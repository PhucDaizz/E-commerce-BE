using Ecommerce.Application.Common.Mappings;
using Ecommerce.Application.DTOS.CartItem;
using Ecommerce.Application.DTOS.Payment;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Application.Repositories.Persistence;
using Ecommerce.Application.Services.Interfaces;
using Ecommerce.Application.Settings;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Enums;
using Microsoft.Extensions.Options;
using System.Data;
using VNPAY.NET;
using VNPAY.NET.Enums;
using VNPAY.NET.Models;

namespace Ecommerce.Application.Services.Impemention
{
    public class PaymentServices : IPaymentServices
    {
        private const double ShippingFee = 30000;
        private const int HoldMinutes = 15;

        private readonly IPaymentRepository _paymentRepository;
        private readonly IDiscountServices _discountServices;
        private readonly IInventoryReservationService _inventoryReservationService;
        private readonly IAuthRepository _authRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVnpay _vnpay;
        private readonly VnpaySettings _vnpaySettings;

        public PaymentServices(IPaymentRepository paymentRepository,
                            IDiscountServices discountServices,
                            IInventoryReservationService inventoryReservationService,
                            IAuthRepository authRepository, IUnitOfWork unitOfWork,
                            IVnpay vnpay, IOptions<VnpaySettings> vnpaySettings)
        {
            _paymentRepository = paymentRepository;
            _discountServices = discountServices;
            _inventoryReservationService = inventoryReservationService;
            _authRepository = authRepository;
            _unitOfWork = unitOfWork;
            _vnpay = vnpay;
            _vnpaySettings = vnpaySettings.Value;
        }

        private static long NewTransactionId()
        {
            // Vừa vặn long (vnp_TxnRef), duy nhất theo mili-giây + random
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 1000 + Random.Shared.Next(0, 1000);
        }

        private static string SanitizeNote(string? note)
        {
            if (string.IsNullOrWhiteSpace(note)) return "Thanh toan don hang";
            var sanitized = note.Replace("|", " ").Replace("\r", " ").Replace("\n", " ").Trim();
            return sanitized.Length > 100 ? sanitized.Substring(0, 100) : sanitized;
        }

        /// <summary>
        /// Tạo đơn nháp Pending + giữ hàng gắn cứng mã giao dịch + sinh URL VNPay,
        /// tất cả trong MỘT transaction serialize để chống oversell và double-submit.
        /// </summary>
        public async Task<PrepareBankingPaymentResult> PrepareBankingPaymentAsync(Guid userId, string? note, int? discountId, string ipAddress)
        {
            var cartItems = (await _unitOfWork.CartItems.GetAllAsync(userId))?.ToList();
            if (cartItems == null || !cartItems.Any())
                return new PrepareBankingPaymentResult { IsSuccess = false, Message = "Cart is empty" };

            var user = await _authRepository.GetInforAsync(userId.ToString());
            if (user == null || string.IsNullOrWhiteSpace(user.PhoneNumber) || string.IsNullOrWhiteSpace(user.Address))
                return new PrepareBankingPaymentResult { IsSuccess = false, Message = "Incomplete profile. Please add phone number and address." };

            var amount = cartItems.Sum(item => item.Quantity * item.Products.Price);
            var finalAmount = amount;
            if (discountId.HasValue)
            {
                var dryRun = await _discountServices.ApplyDiscountAsync(discountId.Value, userId, amount, false);
                if (dryRun < 0)
                    return new PrepareBankingPaymentResult { IsSuccess = false, Message = "Invalid discount code" };
                finalAmount = dryRun;
            }
            finalAmount += ShippingFee;

            var validation = await _inventoryReservationService.CheckAndSuggestCartInventoryAsync(cartItems);
            if (validation.WasAdjusted)
                return new PrepareBankingPaymentResult { IsSuccess = false, Message = "Some items are out of stock.", Validation = validation };

            // Hủy các đơn nháp banking cũ còn Pending để không tồn đọng
            var previousOrders = await _unitOfWork.Orders.GetAllByUserIdAsync(userId);
            foreach (var prev in (previousOrders ?? Enumerable.Empty<Orders>())
                .Where(o => o.Status == (int)OrderStatus.Pending
                    && o.PaymentMethodID == (int)PaymentMethod.VNPAY
                    && !string.IsNullOrEmpty(o.TransactionRef)))
            {
                await _unitOfWork.Orders.UpdateOrderStatus(prev.OrderID, (int)OrderStatus.Cancelled);
                var staleHolds = await _unitOfWork.InventoryReservations.GetByTransactionIdAsync(prev.TransactionRef!);
                if (staleHolds.Any())
                    await _unitOfWork.InventoryReservations.DeleteRangeAsync(staleHolds);
            }
            await _unitOfWork.SaveChangesAsync();

            var txnId = NewTransactionId();
            var txnRef = txnId.ToString();
            var expiresAt = DateTime.UtcNow.AddMinutes(HoldMinutes);

            await _unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                // Re-check trong transaction serialize để chống 2 user cùng hốt size cuối
                if (!await _inventoryReservationService.IsInventoryAvailableAsync(cartItems))
                {
                    await _unitOfWork.RollbackAsync();
                    return new PrepareBankingPaymentResult { IsSuccess = false, Message = "Unable to reserve inventory. Some items may be out of stock." };
                }

                var order = new Orders
                {
                    OrderID = Guid.NewGuid(),
                    UserID = userId,
                    DiscountID = discountId,
                    OrderDate = DateTime.Now,
                    TotalAmount = finalAmount,
                    PaymentMethodID = (int)PaymentMethod.VNPAY,
                    Status = (int)OrderStatus.Pending,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    TransactionRef = txnRef
                };
                await _unitOfWork.Orders.CreateAsync(order);

                var listCart = cartItems.Select(x => x.ToCartItemListDTO());
                await _unitOfWork.OrderDetails.CreateAsync(order.OrderID, listCart);

                var reserved = await _inventoryReservationService.ReserveInventoryAsync(userId, cartItems, txnRef);
                if (!reserved)
                {
                    await _unitOfWork.RollbackAsync();
                    return new PrepareBankingPaymentResult { IsSuccess = false, Message = "Unable to reserve inventory. Some items may be out of stock." };
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                _vnpay.Initialize(_vnpaySettings.TmnCode, _vnpaySettings.HashSecret, _vnpaySettings.BaseUrl, _vnpaySettings.ReturnUrl);
                var request = new PaymentRequest
                {
                    PaymentId = txnId,
                    Money = finalAmount,
                    Description = SanitizeNote(note),
                    IpAddress = ipAddress,
                    BankCode = BankCode.ANY,
                    CreatedDate = DateTime.Now,
                    Currency = Currency.VND,
                    Language = DisplayLanguage.Vietnamese
                };
                var paymentUrl = _vnpay.GetPaymentUrl(request);

                return new PrepareBankingPaymentResult
                {
                    IsSuccess = true,
                    Message = "Payment prepared successfully.",
                    OrderId = order.OrderID,
                    TransactionRef = txnRef,
                    PaymentUrl = paymentUrl,
                    ExpiresAtUtc = expiresAt,
                    TotalAmount = finalAmount
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return new PrepareBankingPaymentResult { IsSuccess = false, Message = $"Could not prepare payment: {ex.Message}" };
            }
        }

        /// <summary>
        /// Xác nhận thanh toán banking từ IPN (gọi được cả từ server VNPay lẫn retry).
        /// Idempotent: giao dịch đã xử lý thì trả success luôn.
        /// </summary>
        public async Task<PaymentProcessResult> ConfirmBankingPaymentAsync(PaymentResult paymentResult)
        {
            var txnRef = paymentResult.PaymentId.ToString();

            if (await _paymentRepository.ExistsByTransactionIdAsync(txnRef))
                return new PaymentProcessResult { IsSuccess = true, Message = "Already processed" };

            var order = await _unitOfWork.Orders.GetByTransactionRefAsync(txnRef);
            if (order == null)
                return new PaymentProcessResult { IsSuccess = false, Message = "Unknown transaction" };

            if (order.Status == (int)OrderStatus.Completed || order.Status == (int)OrderStatus.Confirmed)
                return new PaymentProcessResult { IsSuccess = true, Message = "Already processed" };

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var confirmed = await _inventoryReservationService.ConfirmReservationAsync(order.UserID, txnRef);
                if (!confirmed)
                {
                    // Tiền về trễ sau khi hold hết hạn: thử giữ lại từ tồn hiện tại theo snapshot đơn
                    var details = (await _unitOfWork.OrderDetails.GetListOrderDetailsAsync(order.OrderID))?.ToList()
                        ?? new List<OrderDetails>();
                    var stubs = details.Select(d => new CartItems
                    {
                        UserID = order.UserID,
                        ProductID = d.ProductID,
                        Quantity = d.Quantity,
                        ProductSizeID = d.ProductSizeId
                    }).ToList();

                    if (stubs.Any() && await _inventoryReservationService.IsInventoryAvailableAsync(stubs)
                        && await _inventoryReservationService.ReserveInventoryAsync(order.UserID, stubs, txnRef))
                    {
                        confirmed = await _inventoryReservationService.ConfirmReservationAsync(order.UserID, txnRef);
                    }

                    if (!confirmed)
                    {
                        // Ghi nhận tiền để đối soát, đơn sang Error cho admin hoàn tiền thủ công
                        await _unitOfWork.Payment.CreateAsync(new Payments
                        {
                            OrderID = order.OrderID,
                            UserID = order.UserID,
                            PaymentMethodID = order.PaymentMethodID,
                            PaymentStatus = "Completed",
                            TransactionID = txnRef,
                            AmountPaid = order.TotalAmount,
                            PaymentDate = DateTime.Now,
                        });
                        await _unitOfWork.Orders.UpdateOrderStatus(order.OrderID, (int)OrderStatus.Error);
                        await _unitOfWork.SaveChangesAsync();
                        await _unitOfWork.CommitAsync();
                        return new PaymentProcessResult { IsSuccess = false, Message = "Payment received but items are out of stock. Please contact support for a refund." };
                    }
                }

                if (order.DiscountID.HasValue)
                {
                    var baseAmount = order.TotalAmount - ShippingFee;
                    var discountAmount = await _discountServices.ApplyDiscountAsync(order.DiscountID.Value, order.UserID, baseAmount);
                    if (discountAmount < 0)
                    {
                        await _unitOfWork.RollbackAsync();
                        return new PaymentProcessResult { IsSuccess = false, Message = "Invalid discount code" };
                    }
                }

                var payment = new Payments
                {
                    OrderID = order.OrderID,
                    UserID = order.UserID,
                    PaymentMethodID = order.PaymentMethodID,
                    PaymentStatus = "Completed",
                    TransactionID = txnRef,
                    AmountPaid = order.TotalAmount,
                    PaymentDate = DateTime.Now,
                };

                var user = await _authRepository.GetInforAsync(order.UserID.ToString());
                var shipping = new Shippings
                {
                    ShippingID = Guid.NewGuid(),
                    OrderID = order.OrderID,
                    ShippingMethod = "Standard",
                    ShippingAddress = user?.Address,
                    TrackingNumber = user?.PhoneNumber,
                    ShippingStatus = "1",
                    CreatedAt = DateTime.Now,
                    EstimatedDeliveryDate = DateTime.Now.AddDays(5),
                };

                await _unitOfWork.shipping.CreateAsync(shipping);
                await _unitOfWork.Payment.CreateAsync(payment);
                await _unitOfWork.Orders.UpdateOrderStatus(order.OrderID, (int)OrderStatus.Confirmed);
                await _unitOfWork.CartItems.DeleteAllByUserIDAsync(order.UserID);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return new PaymentProcessResult
                {
                    IsSuccess = true,
                    Message = "Payment processed successfully."
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return new PaymentProcessResult
                {
                    IsSuccess = false,
                    Message = $"Payment failed: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Thanh toán thất bại: giải phóng đúng hold của txn + cancel đơn nháp Pending.
        /// </summary>
        public async Task HandleFailedBankingPaymentAsync(string txnRef)
        {
            var order = await _unitOfWork.Orders.GetByTransactionRefAsync(txnRef);
            if (order != null && order.Status == (int)OrderStatus.Pending)
            {
                await _unitOfWork.Orders.UpdateOrderStatus(order.OrderID, (int)OrderStatus.Cancelled);
            }
            await _inventoryReservationService.ReleaseByTransactionAsync(txnRef);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<BankingPaymentStatusDTO?> GetBankingPaymentStatusAsync(string txnRef, Guid callerUserId)
        {
            var order = await _unitOfWork.Orders.GetByTransactionRefAsync(txnRef);
            if (order == null || order.UserID != callerUserId)
                return null;
            return new BankingPaymentStatusDTO
            {
                OrderId = order.OrderID,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                TransactionRef = txnRef
            };
        }

        public async Task<PaymentProcessResult> processPaymentCOD(Guid userID, int? discountId, int PaymentMethodId = 2)
        {
            // Lấy giỏ hàng
            var cartItems = await _unitOfWork.CartItems.GetAllAsync(userID);
            if (cartItems == null || !cartItems.Any())
            {
                return new PaymentProcessResult
                {
                    IsSuccess = false,
                    Message = "Cart is empty"
                };
            }

            // Tính tổng tiền
            var amount = cartItems.Sum(item => item.Quantity * item.Products.Price);
            var amountFix = amount;

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Áp dụng mã giảm giá (nếu có)
                if (discountId.HasValue)
                {
                    var discountAmount = await _discountServices.ApplyDiscountAsync(discountId.Value, userID, amount);
                    if (discountAmount < 0)
                    {
                        await _unitOfWork.RollbackAsync();
                        return new PaymentProcessResult
                        {
                            IsSuccess = false,
                            Message = "Invalid discount code"
                        };
                    }
                    amountFix = discountAmount;
                }

                // Xác nhận reservation CHƯA GẮN giao dịch (không được ăn hold VNPay đang bay)
                var confirmationSuccess = await _inventoryReservationService.ConfirmReservationAsync(userID, null, true);
                if (!confirmationSuccess)
                {
                    await _inventoryReservationService.ReleaseAllUserReservationsAsync(userID);
                    await _unitOfWork.RollbackAsync();
                    return new PaymentProcessResult { IsSuccess = false, Message = "Unable to secure inventory" };
                }


                // Tạo đơn hàng
                var order = new Orders
                {
                    OrderID = Guid.NewGuid(),
                    UserID = userID,
                    DiscountID = discountId,
                    OrderDate = DateTime.Now,
                    TotalAmount = amountFix + ShippingFee, /* shipping fee */
                    PaymentMethodID = 2,  //COD
                    Status = 0, //Pending
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                // Lưu đơn hàng
                await _unitOfWork.Orders.CreateAsync(order);

                // Tạo thanh toán
                var payment = new Payments
                {
                    OrderID = order.OrderID,
                    UserID = userID,
                    PaymentMethodID = PaymentMethodId,
                    PaymentStatus = "Pending",
                    TransactionID = "COD",
                    AmountPaid = amountFix + ShippingFee,
                    PaymentDate = DateTime.Now,
                };

                // Tạo Shipping
                var user = await _authRepository.GetInforAsync(userID.ToString());
                var shipping = new Shippings
                {
                    ShippingID = Guid.NewGuid(),
                    OrderID = order.OrderID,
                    ShippingMethod = "Standard",
                    ShippingAddress = user.Address,
                    TrackingNumber = user.PhoneNumber,
                    ShippingStatus = "1",
                    CreatedAt = DateTime.Now,
                    EstimatedDeliveryDate = DateTime.Now.AddDays(5),
                };

                // Lưu Shipping
                await _unitOfWork.shipping.CreateAsync(shipping);

                // Thêm chi tiết đơn hàng
                var listCart = cartItems.Select(x => x.ToCartItemListDTO());
                await _unitOfWork.OrderDetails.CreateAsync(order.OrderID, listCart);

                // Xóa giỏ hàng
                await _unitOfWork.CartItems.DeleteAllByUserIDAsync(userID);

                // Lưu thanh toán
                await _unitOfWork.Payment.CreateAsync(payment);

                await _unitOfWork.SaveChangesAsync(); // Save changes to the database
                await _unitOfWork.CommitAsync(); // Commit transaction

                return new PaymentProcessResult
                {
                    IsSuccess = true,
                    Message = "Payment processed successfully."
                };



            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(); // Rollback transaction
                return new PaymentProcessResult
                {
                    IsSuccess = false,
                    Message = $"Payment failed: {ex.Message}"
                };


            }


        }
    }
}

using AutoMapper;
using Ecommerce.Application.DTOS.CartItem;
using Ecommerce.Application.DTOS.Payment;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Application.Repositories.Persistence;
using Ecommerce.Application.Services.Interfaces;
using Ecommerce.Domain.Entities;
using VNPAY.NET.Models;

namespace Ecommerce.Application.Services.Impemention
{
    public class PaymentServices : IPaymentServices
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IDiscountServices _discountServices;
        private readonly IMapper _mapper;
        private readonly IInventoryReservationService _inventoryReservationService;
        private readonly IAuthRepository _authRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentServices(IPaymentRepository paymentRepository,
                            ICartItemRepository cartItemRepository, 
                            IDiscountServices discountServices, 
                            IMapper mapper, IInventoryReservationService inventoryReservationService,
                            IAuthRepository authRepository, IUnitOfWork unitOfWork)
        {
            _paymentRepository = paymentRepository;
            _cartItemRepository = cartItemRepository;
            _discountServices = discountServices;
            _mapper = mapper;
            _inventoryReservationService = inventoryReservationService;
            _authRepository = authRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<PaymentProcessResult> processPaymentTWO(PaymentResult paymentResult, Guid userID, int PaymentMethodId, int? discountId)
        {
            // Lấy giỏ hàng
            var cartItems = await _cartItemRepository.GetAllAsync(userID);
            if (cartItems?.Any() != true)
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

            // Áp dụng mã giảm giá (nếu có)
            if (discountId.HasValue)
            {
                var discountAmount = await _discountServices.ApplyDiscountAsync(discountId.Value, userID, amount);
                if (discountAmount >= 0)
                {
                    amountFix = discountAmount;
                }
            }

            // Tạo đơn hàng
            var order = new Orders
            {
                OrderID = Guid.NewGuid(),
                UserID = userID,
                DiscountID = discountId,
                OrderDate = DateTime.Now,
                TotalAmount = amountFix,
                PaymentMethodID = PaymentMethodId,
                Status = (int)paymentResult.TransactionStatus.Code,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            // Sử dụng TransactionScope nếu không có BeginTransactionAsync
            await _unitOfWork.BeginTransactionAsync();
            {
                try
                {
                    // Lưu đơn hàng
                    await _unitOfWork.Orders.CreateAsync(order);

                    // Thêm chi tiết đơn hàng
                    var listCart = _mapper.Map<IEnumerable<CartItemListDTO>>(cartItems);
                    await _unitOfWork.OrderDetails.CreateAsync(order.OrderID, listCart);

                    // Xóa giỏ hàng
                    await _unitOfWork.CartItems.DeleteAllByUserIDAsync(userID);

                    await _unitOfWork.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu
                    await _unitOfWork.CommitAsync();// Đánh dấu thành công
                    return new PaymentProcessResult
                    {
                        IsSuccess = true,
                        Message = "Payment processed successfully."
                    };
                }
                catch (Exception ex)
                {
                    return new PaymentProcessResult
                    {
                        IsSuccess = false,
                        Message = $"Payment failed: {ex.Message}"
                    };
                }
            }
        }

        public async Task<PaymentProcessResult> processPayment(PaymentResult paymentResult, Guid userID, int PaymentMethodId, int? discountId)
        {
            // Lấy giỏ hàng
            var cartItems = await _cartItemRepository.GetAllAsync(userID);
            if (cartItems == null || !cartItems.Any())
                return new PaymentProcessResult { IsSuccess = false, Message = "Cart is empty" };
            

            // Tính tổng tiền
            var amount = cartItems.Sum(item => item.Quantity * item.Products.Price);
            var amountFix = amount;

            var isExisting = await _paymentRepository.ExistsByTransactionIdAsync(paymentResult.VnpayTransactionId.ToString());  
            if (isExisting)
                return new PaymentProcessResult { IsSuccess = false, Message = "Transaction already exists" };
            
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Áp dụng mã giảm giá (nếu có)
                if (discountId.HasValue)
                {
                    var discountAmount = await _discountServices.ApplyDiscountAsync(discountId.Value, userID, amount);
                    if (discountAmount < 0)
                    {
                        return new PaymentProcessResult { IsSuccess = false, Message = "Invalid discount code" };
                    }
                    amountFix = discountAmount;
                }

                // Xác nhận reservation (chuyển từ reservation sang stock thực tế)
                //var reservationConfirmed = await _inventoryReservationService.ConfirmReservationAsync(userID, paymentResult.VnpayTransactionId.ToString());
                var reservationConfirmed = await _inventoryReservationService.ConfirmReservationAsync(userID, paymentResult.PaymentId.ToString());
                if (!reservationConfirmed)
                {
                    await _unitOfWork.RollbackAsync();
                    return new PaymentProcessResult { IsSuccess = false, Message = "Unable to confirm inventory reservation. Products may no longer be available." };
                }


                // Tạo đơn hàng
                var order = new Orders
                {
                    OrderID = Guid.NewGuid(),
                    UserID = userID,
                    DiscountID = discountId,
                    OrderDate = DateTime.Now,
                    TotalAmount = amountFix + 30000, /* shipping fee */
                    PaymentMethodID = PaymentMethodId,
                    Status = (int)paymentResult.TransactionStatus.Code,
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
                    PaymentStatus = "Completed",
                    TransactionID = paymentResult.VnpayTransactionId.ToString(),
                    AmountPaid = amountFix + 30000,
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
                    /*ActualDeliveryDate = DateTime.Now.AddDays(5),
                    UpdatedAt = DateTime.Now*/
                };

                // Lưu Shipping
                await _unitOfWork.shipping.CreateAsync(shipping);

                // Thêm chi tiết đơn hàng
                var listCart = _mapper.Map<IEnumerable<CartItemListDTO>>(cartItems);
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

        public async Task<PaymentAmountDTO> checkAmount(Guid userId, int? discountId)
        {
            // Get cartItem from database
            var cartItems = await _unitOfWork.CartItems.GetAllAsync(userId);
            if (cartItems == null || !cartItems.Any())
            {
                return new PaymentAmountDTO
                {
                    IsSuccess = false,
                    Message = "Cart is Empty!",
                    FinalAmount = 0
                };
            }

            // Get total amount of cart
            var amount = (float)cartItems.Sum(item => item.Quantity * item.Products.Price);
            var finalAmount = amount;

            // Check discount code if available
            if (discountId.HasValue)
            {
                var discountAmount = await _discountServices.ApplyDiscountAsync(discountId.Value, userId, amount, false);
                if (discountAmount >= 0)
                {
                    finalAmount = (float)discountAmount;
                }
            }
            finalAmount += 30000; // Shipping fee

            _unitOfWork.SaveChangesAsync();

            return new PaymentAmountDTO
            {
                IsSuccess = true,
                Message = "Valid amount",
                FinalAmount = finalAmount
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
                        return new PaymentProcessResult
                        {
                            IsSuccess = false,
                            Message = "Invalid discount code"
                        };
                    }
                    amountFix = discountAmount;
                }

                // Kiểm tra và reserve inventory
                var reservationSuccess = await _inventoryReservationService.ReserveInventoryAsync(userID, cartItems);
                if (!reservationSuccess)
                {
                    await _unitOfWork.RollbackAsync();
                    return new PaymentProcessResult { IsSuccess = false, Message = "Not enough inventory available" };
                }

                // Xác nhận reservation
                var confirmationSuccess = await _inventoryReservationService.ConfirmReservationAsync(userID);
                if (!confirmationSuccess)
                {
                    await _inventoryReservationService.ReleaseReservationAsync(userID);
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
                    TotalAmount = amountFix + 30000, /* shipping fee */
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
                    AmountPaid = amountFix + 30000,
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
                    /*ActualDeliveryDate = DateTime.Now.AddDays(5),
                    UpdatedAt = DateTime.Now*/
                };

                // Lưu Shipping
                await _unitOfWork.shipping.CreateAsync(shipping);

                // Thêm chi tiết đơn hàng
                var listCart = _mapper.Map<IEnumerable<CartItemListDTO>>(cartItems);
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

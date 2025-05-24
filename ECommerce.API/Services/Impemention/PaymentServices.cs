using AutoMapper;
using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.CartItem;
using ECommerce.API.Models.DTO.Payment;
using ECommerce.API.Repositories.Interface;
using ECommerce.API.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using VNPAY.NET.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ECommerce.API.Services.Impemention
{
    public class PaymentServices : IPaymentServices
    {
        private readonly IPaymentRepository paymentRepository;
        private readonly IDiscountRepository discountRepository;
        private readonly ICartItemRepository cartItemRepository;
        private readonly IOrderRepository orderRepository;
        private readonly IDiscountServices discountServices;
        private readonly IOrderDetailRepository orderDetailRepository;
        private readonly IMapper mapper;
        private readonly AppDbContext dbContext;
        private readonly UserManager<ExtendedIdentityUser> userManager;
        private readonly IShippingRepository shippingRepository;
        private readonly IProductSizeRepository productSizeRepository;

        public PaymentServices(IPaymentRepository paymentRepository,IDiscountRepository discountRepository, 
                            ICartItemRepository cartItemRepository, IOrderRepository orderRepository, 
                            IDiscountServices discountServices, IOrderDetailRepository orderDetailRepository,
                            IMapper mapper, AppDbContext dbContext, UserManager<ExtendedIdentityUser> userManager,
                            IShippingRepository shippingRepository, IProductSizeRepository productSizeRepository)
        {
            this.paymentRepository = paymentRepository;
            this.discountRepository = discountRepository;
            this.cartItemRepository = cartItemRepository;
            this.orderRepository = orderRepository;
            this.discountServices = discountServices;
            this.orderDetailRepository = orderDetailRepository;
            this.mapper = mapper;
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.shippingRepository = shippingRepository;
            this.productSizeRepository = productSizeRepository;
        }
        public async Task<PaymentProcessResult> processPaymentTWO(PaymentResult paymentResult, Guid userID, int PaymentMethodId, int? discountId)
        {
            // Lấy giỏ hàng
            var cartItems = await cartItemRepository.GetAllAsync(userID);
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
                var discountAmount = await discountServices.ApplyDiscountAsync(discountId.Value, userID, amount);
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
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    // Lưu đơn hàng
                    await orderRepository.CreateAsync(order);

                    // Thêm chi tiết đơn hàng
                    var listCart = mapper.Map<IEnumerable<CartItemListDTO>>(cartItems);
                    await orderDetailRepository.CreateAsync(order.OrderID, listCart);

                    // Xóa giỏ hàng
                    await cartItemRepository.DeleteAllByUserIDAsync(userID);

                    scope.Complete(); // Đánh dấu thành công
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
            var cartItems = await cartItemRepository.GetAllAsync(userID);
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

            var isExisting = await dbContext.Payments.AnyAsync(x => x.TransactionID == paymentResult.VnpayTransactionId.ToString());
            if (isExisting)
            {
                return new PaymentProcessResult
                {
                    IsSuccess = false,
                    Message = "Transaction already exists"
                };
            }

            await using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                // Áp dụng mã giảm giá (nếu có)
                if (discountId.HasValue)
                {
                    var discountAmount = await discountServices.ApplyDiscountAsync(discountId.Value, userID, amount);
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

                // Kiểm tra và cập nhật tồn kho
                var isStockUpdated = await productSizeRepository.UpdateRangeAsync(cartItems);
                if (!isStockUpdated)
                {
                    await transaction.RollbackAsync(); // Rollback giao dịch nếu lỗi tồn kho
                    return new PaymentProcessResult
                    {
                        IsSuccess = false,
                        Message = "Not found for one or more products"
                    };
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
                await orderRepository.CreateAsync(order);

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
                var user = await userManager.FindByIdAsync(userID.ToString());
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
                await shippingRepository.CreateAsync(shipping);

                // Thêm chi tiết đơn hàng
                var listCart = mapper.Map<IEnumerable<CartItemListDTO>>(cartItems);
                await orderDetailRepository.CreateAsync(order.OrderID, listCart);

                // Xóa giỏ hàng
                await cartItemRepository.DeleteAllByUserIDAsync(userID);

                // Lưu thanh toán
                await paymentRepository.CreateAsync(payment); 

                await transaction.CommitAsync(); // Commit transaction

                return new PaymentProcessResult
                {
                    IsSuccess = true,
                    Message = "Payment processed successfully."
                };


            } catch (Exception ex)
            {
                await transaction.RollbackAsync(); // Rollback transaction
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
            var cartItems = await cartItemRepository.GetAllAsync(userId);
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
                var discountAmount = await discountServices.ApplyDiscountAsync(discountId.Value, userId, amount, false);
                if (discountAmount >= 0)
                {
                    finalAmount = (float)discountAmount;
                }
            }
            finalAmount += 30000; // Shipping fee

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
            var cartItems = await cartItemRepository.GetAllAsync(userID);
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

            await using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                // Áp dụng mã giảm giá (nếu có)
                if (discountId.HasValue)
                {
                    var discountAmount = await discountServices.ApplyDiscountAsync(discountId.Value, userID, amount);
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

                // Kiểm tra và cập nhật tồn kho
                var isStockUpdated = await productSizeRepository.UpdateRangeAsync(cartItems);
                if (!isStockUpdated)
                {
                    await transaction.RollbackAsync(); // Rollback giao dịch nếu lỗi tồn kho
                    return new PaymentProcessResult
                    {
                        IsSuccess = false,
                        Message = "Not found for one or more products"
                    };
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
                await orderRepository.CreateAsync(order);

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
                var user = await userManager.FindByIdAsync(userID.ToString());
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
                await shippingRepository.CreateAsync(shipping);

                // Thêm chi tiết đơn hàng
                var listCart = mapper.Map<IEnumerable<CartItemListDTO>>(cartItems);
                await orderDetailRepository.CreateAsync(order.OrderID, listCart);

                // Xóa giỏ hàng
                await cartItemRepository.DeleteAllByUserIDAsync(userID);

                // Lưu thanh toán
                await paymentRepository.CreateAsync(payment);

                await transaction.CommitAsync(); // Commit transaction

                return new PaymentProcessResult
                {
                    IsSuccess = true,
                    Message = "Payment processed successfully."
                };



            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(); // Rollback transaction
                return new PaymentProcessResult
                {
                    IsSuccess = false,
                    Message = $"Payment failed: {ex.Message}"
                };


            }


        }
    }
}

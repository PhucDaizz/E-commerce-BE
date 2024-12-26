using AutoMapper;
using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.CartItem;
using ECommerce.API.Models.DTO.Payment;
using ECommerce.API.Repositories.Interface;
using ECommerce.API.Services.Interface;
using System.Transactions;
using VNPAY.NET.Models;

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

        public PaymentServices(IPaymentRepository paymentRepository,IDiscountRepository discountRepository, ICartItemRepository cartItemRepository, IOrderRepository orderRepository, IDiscountServices discountServices, IOrderDetailRepository orderDetailRepository,IMapper mapper)
        {
            this.paymentRepository = paymentRepository;
            this.discountRepository = discountRepository;
            this.cartItemRepository = cartItemRepository;
            this.orderRepository = orderRepository;
            this.discountServices = discountServices;
            this.orderDetailRepository = orderDetailRepository;
            this.mapper = mapper;
        }
        public async Task<PaymentProcessResult> processPayment(PaymentResult paymentResult, Guid userID, int PaymentMethodId, int? discountId)
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
    }
}

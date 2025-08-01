using AutoMapper;
using Ecommerce.Application.DTOS.Conversation;
using Ecommerce.Application.DTOS.Discount;
using Ecommerce.Application.DTOS.Shipping;
using Ecommerce.Application.DTOS.User;
using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ecommerce.Application.Common.Mappings
{
    public class AutoMapperProfie : Profile
    {
        public AutoMapperProfie()
        {

            // Category
            CreateMap<CreateCategoryDTO, Categories>().ReverseMap();
            CreateMap<EditCategoryDTO, Categories>().ReverseMap();
            CreateMap<CategoryDTO, Categories>().ReverseMap();

            // Product
            CreateMap<CreateProductDTO, Products>().ReverseMap();
            CreateMap<EditProductDTO, Products>().ReverseMap();
            CreateMap<DetailProductDTO, Products>().ReverseMap();
            CreateMap<ProductDTO, Products>().ReverseMap();
            CreateMap<Products, ListProductDTO>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ProductImages));
            CreateMap<Products, ProductImageCartDTO>().ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ProductImages));
            CreateMap<Products, ListProductAdminDTO>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ProductImages))
                .ForMember(dest => dest.TotalQuantity, opt => opt.MapFrom(src => src.ProductColors.SelectMany(x => x.ProductSizes).Sum(x => x.Stock)));

            // ProductColor
            CreateMap<CreateProductColorDTO, ProductColors>().ReverseMap();
            CreateMap<EditProductColorDTO, ProductColors>().ReverseMap();
            CreateMap<ProductColors, ProductColorDTO>().ForMember(dest => dest.ProductSizes, opt => opt.MapFrom(src => src.ProductSizes));
            CreateMap<ProductColorDTO, ProductColors>();


            // ProductSize
            CreateMap<CreateProductSizeDTO, ProductSizes>().ReverseMap();
            CreateMap<ProductSizeDTO, ProductSizes>().ReverseMap();
            CreateMap<EditProductSizeDTO, ProductSizes>().ReverseMap();

            // ProductImage
            CreateMap<CreateProductImageDTO, ProductImages>().ReverseMap();
            CreateMap<ProductImageDTO, ProductImages>().ReverseMap();

            // CartItem
            CreateMap<CreateCartItemDTO, CartItems>().ReverseMap();
            CreateMap<AddCartItemDTO, CartItems>().ReverseMap();
            CreateMap<EditCartItemDTO, CartItems>().ReverseMap();
            CreateMap<CartItemDTO, CartItems>().ReverseMap();
            CreateMap<CartItems, CartItemListDTO>()
                .ForMember(dest => dest.productDTO, opt => opt.MapFrom(src => src.Products))
                .ForMember(dest => dest.productSizeDTO, opt => opt.MapFrom(src => src.ProductSizes));


            // Discount
            CreateMap<CreateDiscountDTO, Discounts>().ReverseMap();
            CreateMap<DiscountDTO, Discounts>().ReverseMap();
            CreateMap<EditDiscountDTO, Discounts>().ReverseMap();

            // Order
            CreateMap<CreateOrderDTO, Orders>().ReverseMap();
            CreateMap<Orders, OrderDTO>()
                .ForMember(dest => dest.Shipping, opt => opt.MapFrom(src => src.Shippings.OrderBy(x => x.UpdatedAt).LastOrDefault()));
            CreateMap<OrderDetailDTO, Orders>().ReverseMap();
            CreateMap<Orders, GetDetailOrderDTO>()
                   .ForMember(dest => dest.PaymentDTO, opt => opt.MapFrom(src => src.Payments))
                   .ForMember(dest => dest.ShippingDTO, opt => opt.MapFrom(src => src.Shippings.ToList()))
                   .ForMember(dest => dest.GetOrderDetailDTO, opt => opt.MapFrom(src => src.OrderDetails.ToList()));

            // PaymentMethod
            CreateMap<CreatePaymentMethodDTO, PaymentMethods>().ReverseMap();
            CreateMap<PaymentMethodDTO, PaymentMethods>().ReverseMap();

            // ProductReview
            CreateMap<CreateProductReviewDTO, ProductReviews>().ReverseMap();
            CreateMap<ProductReviewDTO, ProductReviews>().ReverseMap();

            // Shipping
            CreateMap<CreateShippingDTO, Shippings>().ReverseMap();
            CreateMap<ShippingDTO, Shippings>().ReverseMap();
            CreateMap<UpdateShippingDTO, Shippings>().ReverseMap();

            //User
            CreateMap<ExtendedIdentityUser, InforDTO>().ReverseMap();

            //OrderDetail
            CreateMap<OrderDetails, GetOrderDetailDTO>().ForMember(dest => dest.ProductDTO, opt => opt.MapFrom(src => src.Products));

            //Payment
            CreateMap<Payments, PaymentDTO>().ReverseMap();

            // Conversation
            CreateMap<Conversations, ListConversationsDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.ClientUser.UserName))
                .ForMember(dest => dest.InitialMessage,
                    opt => opt.MapFrom(src => src.ChatMessages.OrderBy(m => m.SentTimeUtc).Select(x => x.MessageContent).FirstOrDefault()));
            CreateMap<PendingConversationInfo, ListConversationsDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.ClientUserName));

            //ChatMessage
            CreateMap<ChatMessage, ChatMessageDTO>()
                .ForMember(dest => dest.SenderName, opt => opt.Ignore());

        }
    }
}

using AutoMapper;
using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.CartItem;
using ECommerce.API.Models.DTO.Category;
using ECommerce.API.Models.DTO.Discount;
using ECommerce.API.Models.DTO.Order;
using ECommerce.API.Models.DTO.OrderDetail;
using ECommerce.API.Models.DTO.Payment;
using ECommerce.API.Models.DTO.PaymentMethod;
using ECommerce.API.Models.DTO.Product;
using ECommerce.API.Models.DTO.ProductColor;
using ECommerce.API.Models.DTO.ProductImage;
using ECommerce.API.Models.DTO.ProductReview;
using ECommerce.API.Models.DTO.ProductSize;
using ECommerce.API.Models.DTO.Shipping;
using ECommerce.API.Models.DTO.User;

namespace ECommerce.API.Mapping
{
    public class AutoMapperProfie: Profile
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
            CreateMap<Products, ListProductDTO>().ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ProductImages));
            CreateMap<Products, ProductImageCartDTO>().ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ProductImages));

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
            CreateMap<CartItems, CartItemListDTO>().ForMember(dest => dest.productDTO, opt => opt.MapFrom(src => src.Products));


            // Discount
            CreateMap<CreateDiscountDTO, Discounts>().ReverseMap();
            CreateMap<DiscountDTO, Discounts>().ReverseMap();
            CreateMap<EditDiscountDTO, Discounts>().ReverseMap();

            // Order
            CreateMap<CreateOrderDTO, Orders>().ReverseMap();
            CreateMap<OrderDTO, Orders>().ReverseMap();
            CreateMap<OrderDetailDTO, Orders>().ReverseMap();
            CreateMap<Orders, GetDetailOrderDTO>()
                   .ForMember(dest => dest.PaymentDTO, opt => opt.MapFrom(src => src.Payments))
                   .ForMember(dest => dest.ShippingDTO, opt => opt.MapFrom(src => src.Shippings.ToList()))
                   .ForMember(dest => dest.GetOrderDetailDTO, opt => opt.MapFrom(src => src.OrderDetails.ToList()));

            // PaymentMethod
            CreateMap<CreatePaymentMethodDTO,PaymentMethods>().ReverseMap();
            CreateMap<PaymentMethodDTO,PaymentMethods>().ReverseMap();

            // ProductReview
            CreateMap<CreateProductReviewDTO, ProductReviews>().ReverseMap();
            CreateMap<ProductReviewDTO, ProductReviews>().ReverseMap();

            // Shipping
            CreateMap<CreateShippingDTO, Shippings>().ReverseMap();
            CreateMap<ShippingDTO, Shippings>().ReverseMap();

            //User
            CreateMap<ExtendedIdentityUser, InforDTO>().ReverseMap();

            //OrderDetail
            CreateMap<OrderDetails, GetOrderDetailDTO>().ForMember(dest => dest.ProductDTO, opt => opt.MapFrom(src => src.Products));
        
            //Payment
            CreateMap<Payments, PaymentDTO>().ReverseMap();
        }
    }
}

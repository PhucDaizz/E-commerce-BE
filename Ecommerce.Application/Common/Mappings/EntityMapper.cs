using Ecommerce.Application.DTOS.CartItem;
using Ecommerce.Application.DTOS.Category;
using Ecommerce.Application.DTOS.ChatMessage;
using Ecommerce.Application.DTOS.Conversation;
using Ecommerce.Application.DTOS.Discount;
using Ecommerce.Application.DTOS.Order;
using Ecommerce.Application.DTOS.OrderDetail;
using Ecommerce.Application.DTOS.Payment;
using Ecommerce.Application.DTOS.PaymentMethod;
using Ecommerce.Application.DTOS.Product;
using Ecommerce.Application.DTOS.ProductColor;
using Ecommerce.Application.DTOS.ProductImage;
using Ecommerce.Application.DTOS.ProductReview;
using Ecommerce.Application.DTOS.ProductSize;
using Ecommerce.Application.DTOS.Shipping;
using Ecommerce.Application.DTOS.Tag;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Common.Mappings
{
    /// <summary>
    /// Manual mapper thay thế AutoMapper. Không cần package ngoài.
    /// </summary>
    public static class EntityMapper
    {
        // ---------- Category ----------
        public static Categories ToEntity(this CreateCategoryDTO dto)
        {
            if (dto == null) return null!;
            return new Categories
            {
                CategoryName = dto.CategoryName,
                Description = dto.Description
            };
        }

        public static Categories ToEntity(this EditCategoryDTO dto)
        {
            if (dto == null) return null!;
            return new Categories
            {
                CategoryName = dto.CategoryName,
                Description = dto.Description
            };
        }

        public static Categories ToEntity(this CategoryDTO dto)
        {
            if (dto == null) return null!;
            return new Categories
            {
                CategoryID = dto.CategoryID,
                CategoryName = dto.CategoryName,
                ImageURL = dto.ImageURL,
                Description = dto.Description,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt
            };
        }

        public static CategoryDTO ToCategoryDTO(this Categories entity)
        {
            if (entity == null) return null!;
            return new CategoryDTO
            {
                CategoryID = entity.CategoryID,
                CategoryName = entity.CategoryName,
                ImageURL = entity.ImageURL,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        // ---------- Product ----------
        public static Products ToEntity(this CreateProductDTO dto)
        {
            if (dto == null) return null!;
            return new Products
            {
                ProductName = dto.ProductName,
                CategoryID = dto.CategoryID,
                Price = dto.Price,
                Description = dto.Description
            };
        }

        public static Products ToEntity(this EditProductDTO dto)
        {
            if (dto == null) return null!;
            return new Products
            {
                ProductName = dto.ProductName,
                CategoryID = dto.CategoryID,
                Price = dto.Price,
                Description = dto.Description
            };
        }

        public static Products ToEntity(this ProductDTO dto)
        {
            if (dto == null) return null!;
            return new Products
            {
                ProductID = dto.ProductID,
                ProductName = dto.ProductName,
                CategoryID = dto.CategoryID,
                Price = dto.Price,
                Description = dto.Description,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt
            };
        }

        public static ProductDTO ToProductDTO(this Products entity)
        {
            if (entity == null) return null!;
            return new ProductDTO
            {
                ProductID = entity.ProductID,
                ProductName = entity.ProductName,
                CategoryID = entity.CategoryID,
                Price = entity.Price,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        public static ListProductDTO ToListProductDTO(this Products entity)
        {
            if (entity == null) return null!;
            return new ListProductDTO
            {
                ProductID = entity.ProductID,
                ProductName = entity.ProductName,
                CategoryID = entity.CategoryID,
                Price = entity.Price,
                IsPublic = entity.IsPublic,
                Images = entity.ProductImages?.Select(ToProductImageDTO).ToList()
                    ?? Enumerable.Empty<ProductImageDTO>()
            };
        }

        public static ProductImageCartDTO ToProductImageCartDTO(this Products entity)
        {
            if (entity == null) return null!;
            return new ProductImageCartDTO
            {
                ProductID = entity.ProductID,
                ProductName = entity.ProductName,
                CategoryID = entity.CategoryID,
                Price = entity.Price,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                Images = entity.ProductImages?.Select(ToProductImageDTO).ToList()
                    ?? Enumerable.Empty<ProductImageDTO>()
            };
        }

        public static ListProductAdminDTO ToListProductAdminDTO(this Products entity)
        {
            if (entity == null) return null!;
            return new ListProductAdminDTO
            {
                ProductID = entity.ProductID,
                ProductName = entity.ProductName,
                CategoryID = entity.CategoryID,
                Price = entity.Price,
                IsPublic = entity.IsPublic,
                Images = entity.ProductImages?.Select(ToProductImageDTO).ToList()
                    ?? Enumerable.Empty<ProductImageDTO>(),
                TotalQuantity = entity.ProductColors?.SelectMany(x => x.ProductSizes ?? Enumerable.Empty<ProductSizes>()).Sum(x => x.Stock)
            };
        }

        // ---------- ProductColor ----------
        public static ProductColors ToEntity(this CreateProductColorDTO dto)
        {
            if (dto == null) return null!;
            return new ProductColors
            {
                ProductID = dto.ProductID,
                ColorName = dto.ColorName,
                ColorHex = dto.ColorHex
            };
        }

        public static ProductColors ToEntity(this EditProductColorDTO dto)
        {
            if (dto == null) return null!;
            return new ProductColors
            {
                ProductID = dto.ProductID,
                ColorName = dto.ColorName,
                ColorHex = dto.ColorHex
            };
        }

        public static ProductColors ToEntity(this ProductColorDTO dto)
        {
            if (dto == null) return null!;
            return new ProductColors
            {
                ProductColorID = dto.ProductColorID,
                ProductID = dto.ProductID,
                ColorName = dto.ColorName,
                ColorHex = dto.ColorHex,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt
            };
        }

        public static ProductColorDTO ToProductColorDTO(this ProductColors entity)
        {
            if (entity == null) return null!;
            return new ProductColorDTO
            {
                ProductColorID = entity.ProductColorID,
                ProductID = entity.ProductID,
                ColorName = entity.ColorName,
                ColorHex = entity.ColorHex,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                ProductSizes = entity.ProductSizes?.Select(ToProductSizeDTO).ToList()
                    ?? Enumerable.Empty<ProductSizeDTO>()
            };
        }

        // ---------- ProductSize ----------
        public static ProductSizes ToEntity(this CreateProductSizeDTO dto)
        {
            if (dto == null) return null!;
            return new ProductSizes
            {
                ProductColorID = dto.ProductColorID,
                Size = dto.Size,
                Stock = dto.Stock
            };
        }

        public static ProductSizes ToEntity(this EditProductSizeDTO dto)
        {
            if (dto == null) return null!;
            return new ProductSizes
            {
                ProductColorID = dto.ProductColorID,
                Size = dto.Size,
                Stock = dto.Stock
            };
        }

        public static ProductSizes ToEntity(this ProductSizeDTO dto)
        {
            if (dto == null) return null!;
            return new ProductSizes
            {
                ProductSizeID = dto.ProductSizeID,
                ProductColorID = dto.ProductColorID,
                Size = dto.Size,
                Stock = dto.Stock,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt
            };
        }

        public static ProductSizeDTO ToProductSizeDTO(this ProductSizes entity)
        {
            if (entity == null) return null!;
            return new ProductSizeDTO
            {
                ProductSizeID = entity.ProductSizeID,
                ProductColorID = entity.ProductColorID,
                Size = entity.Size,
                Stock = entity.Stock,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        // ---------- ProductImage ----------
        public static ProductImages ToEntity(this ProductImageDTO dto)
        {
            if (dto == null) return null!;
            return new ProductImages
            {
                ImageID = dto.ImageID,
                ProductID = dto.ProductID,
                ImageURL = dto.ImageURL,
                IsPrimary = dto.IsPrimary,
                CreatedAt = dto.CreatedAt
            };
        }

        public static ProductImageDTO ToProductImageDTO(this ProductImages entity)
        {
            if (entity == null) return null!;
            return new ProductImageDTO
            {
                ImageID = entity.ImageID,
                ProductID = entity.ProductID,
                ImageURL = entity.ImageURL,
                IsPrimary = entity.IsPrimary,
                CreatedAt = entity.CreatedAt
            };
        }

        // ---------- CartItem ----------
        public static CartItems ToEntity(this CreateCartItemDTO dto)
        {
            if (dto == null) return null!;
            return new CartItems
            {
                ProductID = dto.ProductID,
                Quantity = dto.Quantity,
                ProductSizeID = dto.ProductSizeID
            };
        }

        public static CartItems ToEntity(this EditCartItemDTO dto)
        {
            if (dto == null) return null!;
            return new CartItems
            {
                CartItemID = dto.CartItemID,
                ProductID = dto.ProductID,
                Quantity = dto.Quantity,
                ProductSizeID = dto.ProductSizeID
            };
        }

        public static CartItems ToEntity(this CartItemDTO dto)
        {
            if (dto == null) return null!;
            return new CartItems
            {
                CartItemID = dto.CartItemID,
                ProductID = dto.ProductID,
                Quantity = dto.Quantity,
                ProductSizeID = dto.ProductSizeID
            };
        }

        public static CartItemDTO ToCartItemDTO(this CartItems entity)
        {
            if (entity == null) return null!;
            return new CartItemDTO
            {
                CartItemID = entity.CartItemID,
                ProductID = entity.ProductID,
                Quantity = entity.Quantity,
                ProductSizeID = entity.ProductSizeID
            };
        }

        public static CartItemListDTO ToCartItemListDTO(this CartItems entity)
        {
            if (entity == null) return null!;
            return new CartItemListDTO
            {
                CartItemID = entity.CartItemID,
                ProductID = entity.ProductID,
                Quantity = entity.Quantity,
                ProductSizeID = entity.ProductSizeID,
                ColorName = entity.ProductSizes?.ProductColors?.ColorName,
                productDTO = entity.Products?.ToProductImageCartDTO()!,
                productSizeDTO = entity.ProductSizes?.ToProductSizeDTO()!
            };
        }

        // ---------- Discount ----------
        public static Discounts ToEntity(this CreateDiscountDTO dto)
        {
            if (dto == null) return null!;
            return new Discounts
            {
                Code = dto.Code,
                Description = dto.Description,
                DiscountType = dto.DiscountType,
                DiscountValue = dto.DiscountValue,
                Quantity = dto.Quantity,
                MaxUsagePerUser = dto.MaxUsagePerUser,
                MinOrderValue = dto.MinOrderValue,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = dto.IsActive
            };
        }

        public static Discounts ToEntity(this EditDiscountDTO dto)
        {
            if (dto == null) return null!;
            return new Discounts
            {
                Code = dto.Code,
                Description = dto.Description,
                DiscountType = dto.DiscountType,
                DiscountValue = dto.DiscountValue,
                Quantity = dto.Quantity,
                MaxUsagePerUser = dto.MaxUsagePerUser,
                MinOrderValue = dto.MinOrderValue,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = dto.IsActive
            };
        }

        public static Discounts ToEntity(this DiscountDTO dto)
        {
            if (dto == null) return null!;
            return new Discounts
            {
                DiscountID = dto.DiscountID,
                Code = dto.Code,
                Description = dto.Description,
                DiscountType = dto.DiscountType,
                DiscountValue = dto.DiscountValue,
                Quantity = dto.Quantity,
                MaxUsagePerUser = dto.MaxUsagePerUser,
                MinOrderValue = dto.MinOrderValue,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = dto.IsActive
            };
        }

        public static DiscountDTO ToDiscountDTO(this Discounts entity)
        {
            if (entity == null) return null!;
            return new DiscountDTO
            {
                DiscountID = entity.DiscountID,
                Code = entity.Code,
                Description = entity.Description,
                DiscountType = entity.DiscountType,
                DiscountValue = entity.DiscountValue,
                Quantity = entity.Quantity,
                MaxUsagePerUser = entity.MaxUsagePerUser,
                MinOrderValue = entity.MinOrderValue,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                IsActive = entity.IsActive
            };
        }

        // ---------- Order ----------
        public static Orders ToEntity(this CreateOrderDTO dto)
        {
            if (dto == null) return null!;
            return new Orders
            {
                UserID = dto.UserID,
                DiscountID = dto.DiscountID == 0 ? null : dto.DiscountID,
                OrderDate = dto.OrderDate,
                TotalAmount = dto.TotalAmount,
                PaymentMethodID = dto.PaymentMethodID,
                Status = dto.Status
            };
        }

        public static Orders ToEntity(this DTOS.Order.OrderDetailDTO dto)
        {
            if (dto == null) return null!;
            return new Orders
            {
                OrderID = dto.OrderID,
                UserID = dto.UserID,
                DiscountID = dto.DiscountID == 0 ? null : dto.DiscountID,
                OrderDate = dto.OrderDate,
                TotalAmount = dto.TotalAmount,
                PaymentMethodID = dto.PaymentMethodID,
                Status = dto.Status,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt
            };
        }

        public static DTOS.Order.OrderDetailDTO ToOrderDetailDTO(this Orders entity)
        {
            if (entity == null) return null!;
            return new DTOS.Order.OrderDetailDTO
            {
                OrderID = entity.OrderID,
                UserID = entity.UserID,
                DiscountID = entity.DiscountID ?? 0,
                OrderDate = entity.OrderDate,
                TotalAmount = entity.TotalAmount,
                PaymentMethodID = entity.PaymentMethodID,
                Status = entity.Status,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        public static OrderDTO ToOrderDTO(this Orders entity)
        {
            if (entity == null) return null!;
            var latestShipping = entity.Shippings?.OrderBy(x => x.UpdatedAt).LastOrDefault();
            return new OrderDTO
            {
                OrderID = entity.OrderID,
                OrderDate = entity.OrderDate,
                TotalAmount = entity.TotalAmount,
                PaymentMethodID = entity.PaymentMethodID,
                Status = entity.Status,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                Shipping = latestShipping?.ToShippingDTO()
            };
        }

        public static GetDetailOrderDTO ToGetDetailOrderDTO(this Orders entity)
        {
            if (entity == null) return null!;
            return new GetDetailOrderDTO
            {
                OrderID = entity.OrderID,
                UserID = entity.UserID,
                DiscountID = entity.DiscountID,
                OrderDate = entity.OrderDate,
                TotalAmount = entity.TotalAmount,
                PaymentMethodID = entity.PaymentMethodID,
                Status = entity.Status,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                PaymentDTO = entity.Payments?.ToPaymentDTO()!,
                ShippingDTO = entity.Shippings?.Select(ToShippingDTO).ToList() ?? new List<ShippingDTO>(),
                GetOrderDetailDTO = entity.OrderDetails?.Select(ToGetOrderDetailDTO).ToList() ?? new List<DTOS.OrderDetail.GetOrderDetailDTO>()
            };
        }

        // ---------- PaymentMethod ----------
        public static PaymentMethods ToEntity(this CreatePaymentMethodDTO dto)
        {
            if (dto == null) return null!;
            return new PaymentMethods
            {
                MethodName = dto.MethodName,
                Description = dto.Description
            };
        }

        public static PaymentMethods ToEntity(this PaymentMethodDTO dto)
        {
            if (dto == null) return null!;
            return new PaymentMethods
            {
                PaymentMethodID = dto.PaymentMethodID,
                MethodName = dto.MethodName,
                Description = dto.Description
            };
        }

        public static PaymentMethodDTO ToPaymentMethodDTO(this PaymentMethods entity)
        {
            if (entity == null) return null!;
            return new PaymentMethodDTO
            {
                PaymentMethodID = entity.PaymentMethodID,
                MethodName = entity.MethodName,
                Description = entity.Description
            };
        }

        // ---------- ProductReview ----------
        public static ProductReviews ToEntity(this CreateProductReviewDTO dto)
        {
            if (dto == null) return null!;
            return new ProductReviews
            {
                ProductID = dto.ProductID,
                Rating = dto.Rating,
                Comment = dto.Comment
            };
        }

        public static ProductReviews ToEntity(this ProductReviewDTO dto)
        {
            if (dto == null) return null!;
            return new ProductReviews
            {
                ReviewID = dto.ReviewID,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt
            };
        }

        public static ProductReviewDTO ToProductReviewDTO(this ProductReviews entity, string? username = null)
        {
            if (entity == null) return null!;
            return new ProductReviewDTO
            {
                ReviewID = entity.ReviewID,
                Rating = entity.Rating,
                Comment = entity.Comment,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                Username = username!
            };
        }

        // ---------- Shipping ----------
        public static Shippings ToEntity(this CreateShippingDTO dto)
        {
            if (dto == null) return null!;
            return new Shippings
            {
                OrderID = dto.OrderID,
                ShippingMethod = dto.ShippingMethod,
                ShippingAddress = dto.ShippingAddress
            };
        }

        public static Shippings ToEntity(this ShippingDTO dto)
        {
            if (dto == null) return null!;
            return new Shippings
            {
                ShippingID = dto.ShippingID,
                OrderID = dto.OrderID,
                ShippingServicesID = dto.ShippingServicesID,
                ShippingFee = dto.ShippingFee,
                ShippingMethod = dto.ShippingMethod,
                ShippingAddress = dto.ShippingAddress,
                TrackingNumber = dto.TrackingNumber,
                ShippingStatus = dto.ShippingStatus,
                EstimatedDeliveryDate = dto.EstimatedDeliveryDate,
                ActualDeliveryDate = dto.ActualDeliveryDate,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt
            };
        }

        public static Shippings ToEntity(this UpdateShippingDTO dto)
        {
            if (dto == null) return null!;
            return new Shippings
            {
                ShippingServicesID = dto.ShippingServicesID,
                ShippingFee = dto.ShippingFee,
                ShippingStatus = dto.ShippingStatus,
                ActualDeliveryDate = dto.ActualDeliveryDate
            };
        }

        public static ShippingDTO ToShippingDTO(this Shippings entity)
        {
            if (entity == null) return null!;
            return new ShippingDTO
            {
                ShippingID = entity.ShippingID,
                OrderID = entity.OrderID,
                ShippingServicesID = entity.ShippingServicesID,
                ShippingFee = entity.ShippingFee,
                ShippingMethod = entity.ShippingMethod ?? string.Empty,
                ShippingAddress = entity.ShippingAddress ?? string.Empty,
                TrackingNumber = entity.TrackingNumber ?? string.Empty,
                ShippingStatus = entity.ShippingStatus ?? string.Empty,
                EstimatedDeliveryDate = entity.EstimatedDeliveryDate ?? default,
                ActualDeliveryDate = entity.ActualDeliveryDate ?? default,
                CreatedAt = entity.CreatedAt ?? default,
                UpdatedAt = entity.UpdatedAt ?? default
            };
        }

        // ---------- OrderDetail ----------
        public static DTOS.OrderDetail.GetOrderDetailDTO ToGetOrderDetailDTO(this OrderDetails entity)
        {
            if (entity == null) return null!;
            return new DTOS.OrderDetail.GetOrderDetailDTO
            {
                OrderDetailID = entity.OrderDetailID,
                OrderID = entity.OrderID,
                ProductID = entity.ProductID,
                Quantity = entity.Quantity,
                UnitPrice = entity.UnitPrice,
                ProductDTO = entity.Products?.ToProductImageCartDTO()!,
                ProductSizeDTO = new GetProductSizeDTO
                {
                    ProductSizeID = entity.ProductSizeId,
                    ColorName = entity.ProductSizes?.ProductColors?.ColorName!,
                    Size = entity.ProductSizes?.Size!
                }
            };
        }

        // ---------- Payment ----------
        public static PaymentDTO ToPaymentDTO(this Payments entity)
        {
            if (entity == null) return null!;
            return new PaymentDTO
            {
                PaymentID = entity.PaymentID,
                PaymentMethodID = entity.PaymentMethodID,
                PaymentStatus = entity.PaymentStatus,
                TransactionID = entity.TransactionID,
                AmountPaid = entity.AmountPaid,
                PaymentDate = entity.PaymentDate,
                PaymentDetails = entity.PaymentDetails
            };
        }

        public static Payments ToEntity(this PaymentDTO dto)
        {
            if (dto == null) return null!;
            return new Payments
            {
                PaymentID = dto.PaymentID,
                PaymentMethodID = dto.PaymentMethodID,
                PaymentStatus = dto.PaymentStatus,
                TransactionID = dto.TransactionID,
                AmountPaid = dto.AmountPaid,
                PaymentDate = dto.PaymentDate,
                PaymentDetails = dto.PaymentDetails
            };
        }

        // ---------- Conversation ----------
        public static ListConversationsDTO ToListConversationsDTO(this PendingConversationInfo src)
        {
            if (src == null) return null!;
            return new ListConversationsDTO
            {
                ConversationId = src.ConversationId,
                ClientUserId = src.ClientUserId,
                UserName = src.ClientUserName,
                StartTimeUtc = src.StartTimeUtc,
                InitialMessage = src.InitialMessage
            };
        }

        public static ListConversationsDTO ToListConversationsDTO(this Conversations src)
        {
            if (src == null) return null!;
            return new ListConversationsDTO
            {
                ConversationId = src.ConversationId,
                ClientUserId = src.ClientUserId,
                UserName = string.Empty,
                StartTimeUtc = src.StartTimeUtc,
                InitialMessage = src.ChatMessages?.OrderBy(m => m.SentTimeUtc).Select(x => x.MessageContent).FirstOrDefault()
            };
        }

        // ---------- ChatMessage ----------
        public static ChatMessageDTO ToChatMessageDTO(this ChatMessage src)
        {
            if (src == null) return null!;
            return new ChatMessageDTO
            {
                ConversationId = src.ConversationId,
                MessageId = src.MessageId,
                SenderUserId = src.SenderUserId,
                MessageContent = src.MessageContent,
                SentTimeUtc = src.SentTimeUtc,
                IsReadByClient = src.IsReadByClient,
                IsReadByAdmin = src.IsReadByAdmin,
                SenderName = null
            };
        }

        // ---------- Tag ----------
        public static Tags ToEntity(this CreateTagDTO dto)
        {
            if (dto == null) return null!;
            return new Tags
            {
                TagName = dto.TagName
            };
        }
    }
}

namespace Ecommerce.Application.DTOS.CartItem
{
    public class MergeCartRequestDTO
    {
        public List<CreateCartItemDTO> Items { get; set; } = new List<CreateCartItemDTO>();
    }
}

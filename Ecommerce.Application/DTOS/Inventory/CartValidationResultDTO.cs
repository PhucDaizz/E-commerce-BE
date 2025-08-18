namespace Ecommerce.Application.DTOS.Inventory
{
    public class CartValidationResultDTO
    {
        public bool WasAdjusted { get; set; }
        public List<ValidatedCartItemDTO> ValidatedItems { get; set; } = new();
        public List<string> Messages { get; set; } = new();
    }
}

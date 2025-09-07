using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.DTOS.Category
{
    public class AddCategoryImageCommand
    {
        public Categories  Category { get; set; }
        public string FileName { get; set; }
        public bool UseCloudStorage { get; set; }
    }
}

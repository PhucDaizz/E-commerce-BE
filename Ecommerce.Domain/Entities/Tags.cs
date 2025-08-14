using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Domain.Entities
{
    public class Tags
    {
        [Key]
        public int TagID { get; set; }
        public string TagName { get; set; } 
        public string Slug { get; set; }

        // Navigation properties
        public ICollection<ProductTags> ProductTags { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.DTOS.Tag
{
    public class CreateTagDTO
    {
        [Required]
        public string TagName { get; set; }
    }
}

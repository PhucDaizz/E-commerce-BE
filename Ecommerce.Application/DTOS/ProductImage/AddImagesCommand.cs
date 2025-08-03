using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOS.ProductImage
{
    public class AddImagesCommand
    {
        public int ProductId { get; set; }
        public List<(Stream Stream, string FileName)> ImageFiles { get; set; } = new();
        public bool UseCloudStorage { get; set; }
    }
}

using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOS.Category
{
    public class UpdateCategoryCommand
    {
        public Categories Categorie { get; set; }
        public string FileName { get; set; }
        public bool UseCloudStorage { get; set; }
        public bool HasNewImage { get; set; }
    }
}

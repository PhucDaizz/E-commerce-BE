using AutoMapper;
using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.Category;
using ECommerce.API.Models.DTO.Product;

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

        }
    }
}

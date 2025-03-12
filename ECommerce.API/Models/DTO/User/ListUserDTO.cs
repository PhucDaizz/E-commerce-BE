namespace ECommerce.API.Models.DTO.User
{
    public class ListUserDTO
    {
        public int CurrentPage { get; set; }
        public int TotalPage { get; set; }
        public int TotalItem { get; set; }
        public IEnumerable<InforDTO> inforDTOs { get; set; }
    }
}

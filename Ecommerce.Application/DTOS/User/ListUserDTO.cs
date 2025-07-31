using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOS.User
{
    public class ListUserDTO
    {
        public int CurrentPage { get; set; }
        public int TotalPage { get; set; }
        public int TotalItem { get; set; }
        public IEnumerable<InforDTO> inforDTOs { get; set; }
    }
}

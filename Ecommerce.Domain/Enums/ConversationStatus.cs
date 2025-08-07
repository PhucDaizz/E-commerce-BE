using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Domain.Enums
{
    public enum ConversationStatus
    {
        Pending,  // Chưa có admin phụ trách
        Active,   // Đang được admin phụ trách
        Closed    // Đã kết thúc
    }
}

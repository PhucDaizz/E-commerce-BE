using Ecommerce.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOS.Conversation
{
    public class ActiveConversationInfoDto
    {
        public Guid ConversationId { get; set; }
        public ConversationStatus Status { get; set; }

        public string? AdminUserId { get; set; }
        public string? AdminUserName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOS.Conversation
{
    public class PendingConversationInfo
    {
        public Guid ConversationId { get; set; }
        public string ClientUserId { get; set; }
        public string ClientUserName { get; set; }
        public DateTime StartTimeUtc { get; set; }
        public string? InitialMessage { get; set; }
    }
}

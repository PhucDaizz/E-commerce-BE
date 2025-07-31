using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Domain.Entities
{
    public class ChatMessage
    {
        public Guid MessageId { get; set; }
        public Guid ConversationId { get; set; }
        public string SenderUserId { get; set; }
        public string MessageContent { get; set; }
        public DateTime SentTimeUtc { get; set; } = DateTime.UtcNow;
        public bool IsReadByClient { get; set; }
        public bool IsReadByAdmin { get; set; }

        // Navigation properties
        public Conversations Conversation { get; set; }
    }
}

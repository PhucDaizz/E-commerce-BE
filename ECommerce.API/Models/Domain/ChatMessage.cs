using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.API.Models.Domain
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

        [ForeignKey("SenderUserId")]
        public ExtendedIdentityUser Sender { get; set; }
    }
}

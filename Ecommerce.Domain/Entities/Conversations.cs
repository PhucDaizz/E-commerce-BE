using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Domain.Enums;

namespace Ecommerce.Domain.Entities
{
    public class Conversations
    {
        public Guid ConversationId { get; set; }
        public string ClientUserId { get; set; }
        public string? AdminUserId { get; set; }
        public ConversationStatus Status { get; set; } = ConversationStatus.Pending;
        public DateTime StartTimeUtc { get; set; } = DateTime.UtcNow;
        public DateTime LastActivityTimeUtc { get; set; }
        public DateTime? EndTimeUtc { get; set; }

        // Navigation properties
        public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
    }
}

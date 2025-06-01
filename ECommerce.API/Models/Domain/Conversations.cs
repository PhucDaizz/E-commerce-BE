using ECommerce.API.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.API.Models.Domain
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
        [ForeignKey("ClientUserId")]
        public  ExtendedIdentityUser ClientUser { get; set; }
        [ForeignKey("AdminUserId")]
        public  ExtendedIdentityUser? AdminUser { get; set; }  

        public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
    }
}

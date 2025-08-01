using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOS.Conversation
{
    public class ConversationDetailsDto
    {
        public Guid ConversationId { get; set; }
        public ConversationStatus Status { get; set; } = ConversationStatus.Pending;
        public DateTime StartTimeUtc { get; set; } = DateTime.UtcNow;
        public DateTime LastActivityTimeUtc { get; set; }
        public DateTime? EndTimeUtc { get; set; }       
        
        public string ClientUserId { get; set; }
        public string ClientUserName { get; set; }
        public string? AdminUserId { get; set; }
        public string? AdminUserName { get; set; }

    }
}

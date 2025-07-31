using Ecommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Identity
{
    public class ExtendedIdentityUser : IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public bool? Gender { get; set; }
        public string? Address { get; set; }

        // Navigation properties
        public ICollection<Conversations> ClientConversations { get; set; } = new List<Conversations>();
        public ICollection<Conversations> AdminAssignedConversations { get; set; } = new List<Conversations>();
        public ICollection<ChatMessage> SentMessages { get; set; } = new List<ChatMessage>();
    }
}

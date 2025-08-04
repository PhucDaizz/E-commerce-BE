using Ecommerce.Application.DTOS.Conversation;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Repositories
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly AppDbContext _dbContext;

        public ConversationRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AcceptChatAsync(Guid conversationId, string adminUserId)
        {
            var conversationToUpdate = await _dbContext.Conversations.FindAsync(conversationId);
            if (conversationToUpdate == null)
            {
                return false;
            }
            conversationToUpdate.AdminUserId = adminUserId;
            conversationToUpdate.Status = ConversationStatus.Active;
            conversationToUpdate.LastActivityTimeUtc = DateTime.UtcNow;

            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<Conversations> AddAsync(Conversations conversation)
        {
            var addedConversation = await _dbContext.Conversations.AddAsync(conversation);
            await _dbContext.SaveChangesAsync();
            return addedConversation.Entity;
        }

        public async Task<bool> CloseChatAsync(Guid conversationId)
        {
            var conversationToUpdate = await _dbContext.Conversations.FindAsync(conversationId);
            if (conversationToUpdate == null)
            {
                return false;
            }

            if (conversationToUpdate.Status == ConversationStatus.Closed)
            {
                return true; 
            }

            conversationToUpdate.Status = ConversationStatus.Closed;
            conversationToUpdate.EndTimeUtc = DateTime.UtcNow;
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public void DeleteRange(IEnumerable<Conversations> conversations)
        {
            _dbContext.Conversations.RemoveRange(conversations);
        }

        public async Task<Conversations?> GetActiveConversationByClientForDisconnectAsync(string userId)
        {
            return await _dbContext.Conversations.FirstOrDefaultAsync(x => x.ClientUserId == userId);
        }

        public async Task<IEnumerable<ConversationDetailsDto>> GetActiveConversationsByAdminAsync(string userId)
        {
            var query = from conv in _dbContext.Conversations
                        join clientUser in _dbContext.ExtendedIdentityUsers
                            on conv.ClientUserId equals clientUser.Id
                        where conv.AdminUserId == userId && conv.Status == ConversationStatus.Active
                        orderby conv.StartTimeUtc descending
                        select new ConversationDetailsDto
                        {
                            ConversationId = conv.ConversationId,
                            Status = conv.Status,
                            StartTimeUtc = conv.StartTimeUtc,
                            LastActivityTimeUtc = conv.LastActivityTimeUtc,
                            EndTimeUtc = conv.EndTimeUtc,
                            ClientUserId = conv.ClientUserId,
                            ClientUserName = clientUser.UserName,
                            AdminUserId = conv.AdminUserId
                        };
            return await query.ToListAsync();
        }

        public async Task<ConversationDetailsDto?> GetByIdAsync(Guid conversationId)
        {
            var query = from conv in _dbContext.Conversations
                        join clientUser in _dbContext.ExtendedIdentityUsers
                            on conv.ClientUserId equals clientUser.Id
                        join adminUser in _dbContext.ExtendedIdentityUsers
                            on conv.AdminUserId equals adminUser.Id into adminGroup
                        from admin in adminGroup.DefaultIfEmpty()
                        where conv.ConversationId == conversationId
                        select new ConversationDetailsDto
                        {
                            ConversationId = conv.ConversationId,
                            Status = conv.Status,
                            StartTimeUtc = conv.StartTimeUtc,
                            LastActivityTimeUtc = conv.LastActivityTimeUtc,
                            EndTimeUtc = conv.EndTimeUtc,
                            ClientUserId = conv.ClientUserId,
                            ClientUserName = clientUser.UserName,
                            AdminUserId = admin != null ? admin.Id : null,
                            AdminUserName = admin != null ? admin.UserName : null
                        };
            return await query.FirstOrDefaultAsync();
        }

        public async Task<ActiveConversationInfoDto> GetConversationPedingOrActiveAsync(string userId)
        {
            var query = from conv in _dbContext.Conversations
                        join adminUser in _dbContext.ExtendedIdentityUsers
                            on conv.AdminUserId equals adminUser.Id into adminGroup
                        from admin in adminGroup.DefaultIfEmpty() 
                        where conv.ClientUserId == userId &&
                              (conv.Status == ConversationStatus.Pending || conv.Status == ConversationStatus.Active)
                        select new ActiveConversationInfoDto
                        {
                            ConversationId = conv.ConversationId,
                            Status = conv.Status,
                            AdminUserId = admin != null ? admin.Id : null,
                            AdminUserName = admin != null ? admin.UserName : "Admin" 
                        };

            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<Conversations>> GetOldClosedConversationsForCleanupAsync(TimeSpan cleanupThreshold, CancellationToken cancellationToken)
        {
            var cutoffDate = DateTime.UtcNow.Subtract(cleanupThreshold);
            return await _dbContext.Conversations
                .Where(c => c.Status == ConversationStatus.Closed && c.EndTimeUtc.HasValue && c.EndTimeUtc < cutoffDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<PendingConversationInfo>?> GetPendingConversationsForAdminAsync()
        {
            var query = from conv in _dbContext.Conversations
                        join clientUser in _dbContext.ExtendedIdentityUsers
                            on conv.ClientUserId equals clientUser.Id
                        where conv.Status == ConversationStatus.Pending
                        orderby conv.StartTimeUtc descending
                        select new PendingConversationInfo
                        {
                            ConversationId = conv.ConversationId,
                            ClientUserId = conv.ClientUserId,
                            ClientUserName = clientUser.UserName, 
                            StartTimeUtc = conv.StartTimeUtc,
                            InitialMessage = (from msg in _dbContext.ChatMessage
                                              where msg.ConversationId == conv.ConversationId
                                              orderby msg.SentTimeUtc
                                              select msg.MessageContent).FirstOrDefault()
                        };

            return await query.ToListAsync();
        }

        public async Task<Conversations?> GetPendingOrActiveConversationByClientAsync(string userId)
        {
            return await _dbContext.Conversations.FirstOrDefaultAsync(c => c.ClientUserId == userId &&
                (c.Status == ConversationStatus.Pending || c.Status == ConversationStatus.Active));
        }

        public async Task<List<Conversations>> GetStalePendingConversationsAsync(TimeSpan cleanupThreshold, CancellationToken cancellationToken)
        {
            var cutoffDate = DateTime.UtcNow.Subtract(cleanupThreshold);
            return await _dbContext.Conversations
                .Where(c => c.Status == ConversationStatus.Pending && c.StartTimeUtc < cutoffDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> UpdateAsync(Conversations conversations)
        {
            var updatedConversation = _dbContext.Conversations.Update(conversations);
            await _dbContext.SaveChangesAsync();
            return updatedConversation != null;
        }

        public async Task<bool> UpdateLastActivityTimeAsync(Guid conversationId)
        {
            var conversationToUpdate = await _dbContext.Conversations.FindAsync(conversationId);
            if (conversationToUpdate == null)
            {
                return false;
            }

            conversationToUpdate.LastActivityTimeUtc = DateTime.UtcNow;
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}

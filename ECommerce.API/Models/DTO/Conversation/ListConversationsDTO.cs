namespace ECommerce.API.Models.DTO.Conversation
{
    public class ListConversationsDTO
    {
        public Guid ConversationId { get; set; }
        public string ClientUserId { get; set; }
        public string UserName { get; set; }
        public DateTime StartTimeUtc { get; set; }

        public string? InitialMessage { get; set; }
    }
}

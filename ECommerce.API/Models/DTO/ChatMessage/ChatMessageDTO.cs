namespace ECommerce.API.Models.DTO.ChatMessage
{
    public class ChatMessageDTO
    {
        public Guid? ConversationId { get; set; }
        public Guid MessageId { get; set; }
        public string SenderUserId { get; set; }
        public string MessageContent { get; set; }
        public DateTime SentTimeUtc { get; set; }
        public bool IsReadByClient { get; set; }
        public bool IsReadByAdmin { get; set; }
        public String? SenderName { get; set; }
    }
}

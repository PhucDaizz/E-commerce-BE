namespace Ecommerce.Infrastructure.Settings
{
    public class EmailSettings
    {
        public const string SectionName = "EmailSettings";

        public string? MailServer { get; set; }
        public int MailPort { get; set; }
        public string? FromEmail { get; set; }
        public string? Password { get; set; }
    }
}

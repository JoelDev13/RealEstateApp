namespace RealEstateApp.WebApp.Models.Chat
{
    public class ChatSummaryViewModel
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string LastMessage { get; set; } = string.Empty;
        public DateTime LastMessageDate { get; set; }
        public int UnreadCount { get; set; }
    }
}
namespace RealEstateApp.Application.Dtos.Messages
{
    public class MessageDto
    {
        public Guid Id { get; set; }
        public int PropertyId { get; set; }
        public string PropertyCode { get; set; } = string.Empty;
        public string SenderId { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime SentDate { get; set; }
        public bool IsFromAgent { get; set; }
        public string TimeAgo => GetTimeAgo(SentDate);

        private string GetTimeAgo(DateTime dateTime)
        {
            var span = DateTime.UtcNow - dateTime;
            
            if (span.TotalMinutes < 1)
                return "Ahora";
            if (span.TotalMinutes < 60)
                return $"Hace {span.Minutes} min";
            if (span.TotalHours < 24)
                return $"Hace {span.Hours} h";
            if (span.TotalDays < 7)
                return $"Hace {span.Days} días";
            
            return dateTime.ToString("dd/MM/yyyy");
        }
    }
}

namespace RealEstateApp.Domain.Entities
{
    public class Message
    {
        public Guid Id { get; set; }
        
        public int PropertyId { get; set; }
        public Property? Property { get; set; }
        
        public string SenderId { get; set; } = string.Empty;
        
        public string ReceiverId { get; set; } = string.Empty;
        
        public string Content { get; set; } = string.Empty;
        
        public DateTime SentDate { get; set; } = DateTime.UtcNow;
    }
}

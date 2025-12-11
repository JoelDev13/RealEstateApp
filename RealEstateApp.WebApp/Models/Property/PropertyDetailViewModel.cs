namespace RealEstateApp.WebApp.Models.Property
{
    public class PropertyDetailViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string PropertyType { get; set; } = string.Empty;
        public string SaleType { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public decimal Size { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsSold { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public List<string> Improvements { get; set; } = new();
        public string AgentName { get; set; } = string.Empty;
        public string AgentPhone { get; set; } = string.Empty;
        public string AgentEmail { get; set; } = string.Empty;
        public string AgentProfilePicture { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public List<ClientChatSummary> ClientChats { get; set; } = new();
        public List<ClientOfferSummary> ClientOffers { get; set; } = new();
    }

    // ViewModel para resumen de chats con clientes
    public class ClientChatSummary
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string LastMessage { get; set; } = string.Empty;
        public DateTime LastMessageDate { get; set; }
        public int UnreadCount { get; set; }
    }

    // ViewModel para resumen de ofertas de clientes
    public class ClientOfferSummary
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public decimal LastOfferAmount { get; set; }
        public DateTime LastOfferDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string StatusText { get; set; } = string.Empty;
        public int TotalOffers { get; set; }
    }
}
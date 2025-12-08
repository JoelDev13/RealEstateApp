namespace RealEstateApp.Application.Dtos.Agents
{
    public class AgentDto
    {
        public string Id { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int PropertyCount { get; set; }
    }

    public class AgentPropertyDto
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
        public DateTime CreatedAt { get; set; }
    }
}

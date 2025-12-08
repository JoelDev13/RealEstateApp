namespace RealEstateApp.Application.Dtos.Property
{
    public class PropertyDetailDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string PropertyTypeName { get; set; } = string.Empty;
        public string SaleTypeName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public double SizeInSquareMeters { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<PropertyImageDto> Images { get; set; } = new();  // Handler para imágenes
        public List<string> Improvements { get; set; } = new();
        public AgentInfoDto Agent { get; set; } = new();
    }

    public class PropertyImageDto
    {
        public int Id { get; set; }
        public byte[]? ImageData { get; set; }  // Handler para imagen
        public string ImageContentType { get; set; } = string.Empty;  // Content-Type
        public bool IsPrimary { get; set; }
    }

    public class AgentInfoDto
    {
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public byte[]? PhotoData { get; set; }  // Handler para foto
        public string PhotoContentType { get; set; } = string.Empty;  // Content-Type
    }
}

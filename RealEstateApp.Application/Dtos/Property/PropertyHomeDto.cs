namespace RealEstateApp.Application.Dtos.Property
{
    public class PropertyHomeDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string PropertyTypeName { get; set; } = string.Empty;
        public byte[]? ImageData { get; set; }  // Handler para imagen
        public string ImageContentType { get; set; } = string.Empty;  // Content-Type
        public string SaleTypeName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public double SizeInSquareMeters { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

namespace RealEstateApp.Domain.Entities
{
    public class Property
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; } = null!;
        public double SizeInSquareMeters { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }

        public int PropertyTypeId { get; set; }
        public int SaleTypeId { get; set; }
        public PropertyType? PropertyType { get; set; }
        public SaleType? SaleType { get; set; }

        public string AgentId { get; set; } = null!;

        public ICollection<Improvement> Improvements { get; set; }
            = new HashSet<Improvement>();

        public ICollection<PropertyImage> Images { get; set; }
            = new HashSet<PropertyImage>();

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}

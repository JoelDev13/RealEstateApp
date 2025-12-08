namespace RealEstateApp.Domain.Entities
{
    public class Improvement
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<Property> Properties { get; set; }
            = new HashSet<Property>();
    }
}

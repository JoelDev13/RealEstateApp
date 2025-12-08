namespace RealEstateApp.Domain.Entities
{
    public class PropertyImprovement
    {
        public int PropertyId { get; set; }
        public int ImprovementId { get; set; }

        public Property Property { get; set; } = null!;
        public Improvement Improvement { get; set; } = null!;
    }
}

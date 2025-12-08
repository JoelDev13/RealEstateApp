namespace RealEstateApp.Domain.Entities
{
    public class PropertyImage
    {
        public int Id { get; set; }
        public string Url { get; set; } = null!;
        public bool IsPrimary { get; set; } = false;

        public int PropertyId { get; set; }
        public Property Property { get; set; } = null!;
    }
}

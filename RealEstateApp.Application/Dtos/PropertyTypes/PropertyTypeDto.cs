namespace RealEstateApp.Application.Dtos.PropertyTypes
{
    public class PropertyTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}

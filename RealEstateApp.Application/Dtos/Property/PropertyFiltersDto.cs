namespace RealEstateApp.Application.Dtos.Property
{
    public class PropertyFiltersDto
    {
        public string? Code { get; set; }
        public int? PropertyTypeId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }
    }
}

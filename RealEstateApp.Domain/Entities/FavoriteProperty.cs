using RealEstateApp.Domain.Common;

namespace RealEstateApp.Domain.Entities
{
    public class FavoriteProperty : BaseEntity
    {
        public string ClientId { get; set; } = null!;
        public int PropertyId { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;

        // Propiedad de navegación para la propiedad
        public Property Property { get; set; } = null!;
    }
}

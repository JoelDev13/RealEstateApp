using RealEstateApp.Domain.Common;
using RealEstateApp.Domain.Enums;

namespace RealEstateApp.Domain.Entities
{
    public class Offer : BaseEntity
    {
        public int PropertyId { get; set; }
        public string ClientId { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime OfferDate { get; set; } = DateTime.UtcNow;
        public OfferStatus Status { get; set; } = OfferStatus.Pendiente;

        // Propiedad de navegacion para la propiedad
        public Property Property { get; set; } = null!;
    }
}

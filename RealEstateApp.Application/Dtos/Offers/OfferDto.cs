using RealEstateApp.Domain.Enums;

namespace RealEstateApp.Application.Dtos.Offers
{
    public class OfferDto
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string ClientId { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime OfferDate { get; set; }
        public OfferStatus Status { get; set; }
        public string StatusText => Status.ToString();
        public string PropertyCode { get; set; } = string.Empty;
        public string PropertyDescription { get; set; } = string.Empty;
    }
}
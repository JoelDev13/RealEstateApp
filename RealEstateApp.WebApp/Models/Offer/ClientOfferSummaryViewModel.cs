namespace RealEstateApp.WebApp.Models.Offer
{
    public class ClientOfferSummaryViewModel
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public decimal LastOfferAmount { get; set; }
        public DateTime LastOfferDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalOffers { get; set; }
    }
}
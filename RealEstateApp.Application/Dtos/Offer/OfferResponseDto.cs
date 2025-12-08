namespace RealEstateApp.Application.Dtos.Offer
{
    public class OfferResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public OfferDto? Offer { get; set; }
    }
}

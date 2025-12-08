using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Application.Dtos.Offer
{
    public class CreateOfferDto
    {
        [Required]
        public int PropertyId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor que 0")]
        public decimal Amount { get; set; }
    }
}

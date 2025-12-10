using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Application.Dtos.Offers
{
    public class CreateOfferDto
    {
        [Required(ErrorMessage = "El monto es requerido")]
        [Range(1, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal Amount { get; set; }
        
        public int PropertyId { get; set; }
        public string ClientId { get; set; } = string.Empty;
    }
}

using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Application.Dtos.Messages
{
    public class CreateMessageDto
    {
        [Required(ErrorMessage = "El mensaje es requerido")]
        [StringLength(1000, ErrorMessage = "El mensaje no puede exceder 1000 caracteres")]
        public string Content { get; set; } = string.Empty;
        
        public int PropertyId { get; set; }
        public string SenderId { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
    }
}

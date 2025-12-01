using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Application.Dtos.Auth
{
    public class ResetPasswordRequestDto
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        
        [Required(ErrorMessage = "La contraseña es requerida")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "La confirmación de contraseña es requerida")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; }
    }
}


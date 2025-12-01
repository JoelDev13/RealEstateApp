using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Application.Dtos.Auth
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "El email o nombre de usuario es requerido")]
        [Display(Name = "Email o Usuario")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Application.Dtos.Auth
{
    public class LoginDto
    {
        [Required(ErrorMessage = "El correo electrónico o nombre de usuario es requerido")]
        public string EmailOrUserName { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}


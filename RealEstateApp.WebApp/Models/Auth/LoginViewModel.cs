using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Application.ViewModels.Auth
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El correo electrónico o nombre de usuario es requerido")]
        [Display(Name = "Correo o Usuario")]
        public string EmailOrUserName { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}


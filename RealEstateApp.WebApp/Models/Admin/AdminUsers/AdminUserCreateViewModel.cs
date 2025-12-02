using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.WebApp.Models.Admin.AdminUsers
{
    public class AdminUserCreateViewModel
    {
        [Required]
        [Display(Name = "Nombre")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Apellido")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Cédula")]
        public string Cedula { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Correo")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Usuario")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.WebApp.Models.Admin.AdminUsers
{
    public class AdminUserEditViewModel
    {
        [Required]
        public string Id { get; set; } = string.Empty;

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

        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña (opcional)")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar nueva contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string? ConfirmPassword { get; set; }
    }
}

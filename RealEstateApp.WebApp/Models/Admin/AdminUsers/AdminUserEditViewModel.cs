using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.WebApp.Models.Admin.AdminUsers
{
    public class AdminUserEditViewModel
    {
        [Required(ErrorMessage = "El identificador del usuario es obligatorio.")]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres.")]
        [Display(Name = "Nombre")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50, ErrorMessage = "El apellido no puede exceder 50 caracteres.")]
        [Display(Name = "Apellido")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "La cédula es obligatoria.")]
        [RegularExpression(@"^\d{3}-\d{7}-\d{1}$",
            ErrorMessage = "La cédula debe tener el formato 000-0000000-0.")]
        [Display(Name = "Cédula")]
        public string Cedula { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder 100 caracteres.")]
        [Display(Name = "Correo")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El usuario es obligatorio.")]
        [StringLength(50, ErrorMessage = "El usuario no puede exceder 50 caracteres.")]
        [Display(Name = "Usuario")]
        public string UserName { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8,
            ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        [Display(Name = "Nueva contraseña (opcional)")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar nueva contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string? ConfirmPassword { get; set; }
    }
}

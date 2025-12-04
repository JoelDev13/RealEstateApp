using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Application.Dtos.Auth
{
    public class UserSaveDto
    {
        public string? Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        
        [Required(ErrorMessage = "La cédula es requerida")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "La cédula debe tener exactamente 11 dígitos")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "La cédula debe contener solo números y tener exactamente 11 dígitos")]
        public string Cedula { get; set; }
        
        public string? PhoneNumber { get; set; }
        public string? ProfilePicture { get; set; }
        public string UserType { get; set; }
    }
}


using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Application.Dtos.Auth
{
    public class ForgotPasswordApiRequestDto
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        public string UserName { get; set; }
    }
}


namespace RealEstateApp.Application.Dtos.Auth
{
    public class ForgotPasswordRequestDto
    {
        public string UserName { get; set; }
        public string? Origin { get; set; }
    }
}


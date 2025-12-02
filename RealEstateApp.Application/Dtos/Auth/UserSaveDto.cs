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
        public string? PhoneNumber { get; set; }
        public string? ProfilePicture { get; set; }
        public string UserType { get; set; }
    }
}


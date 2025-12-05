namespace RealEstateApp.Application.Dtos.DeveloperUsers
{
    public class DeveloperUserUpdateDto
    {
        public string Id { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Cedula { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string? Password { get; set; }
    }
}

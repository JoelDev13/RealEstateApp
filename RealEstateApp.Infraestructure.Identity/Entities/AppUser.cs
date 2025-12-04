using Microsoft.AspNetCore.Identity;

namespace RealEstateApp.Infraestructure.Identity.Entities
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Cedula { get; set; }
        public override string PhoneNumber { get; set; }
        public string? ProfilePicture { get; set; }
        public string UserType { get; set; }
        public bool IsActive { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}

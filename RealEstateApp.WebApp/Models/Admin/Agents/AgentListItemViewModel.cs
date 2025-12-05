namespace RealEstateApp.WebApp.Models.Agents
{
    public class AgentListItemViewModel
    {
        public string Id { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";

        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }

        public int PropertyCount { get; set; }
        public bool IsActive { get; set; }
    }
}

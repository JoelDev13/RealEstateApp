using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.WebApp.Models.Agents
{
    public class AgentDashboardViewModel
    {
        public List<AgentPropertyViewModel> Properties { get; set; } = new();
    }

    public class AgentPropertyViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string PropertyType { get; set; } = string.Empty;
        public string SaleType { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public decimal Size { get; set; }
        public string MainImageUrl { get; set; } = string.Empty;
        public bool IsSold { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class AgentPropertiesViewModel
    {
        public List<AgentPropertyViewModel> Properties { get; set; } = new();
    }

    public class AgentProfileViewModel
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es requerido")]
        [StringLength(50, ErrorMessage = "El apellido no puede exceder 50 caracteres")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es requerido")]
        [Phone(ErrorMessage = "Formato de teléfono inválido")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
        public string PhoneNumber { get; set; } = string.Empty;

        [DataType(DataType.ImageUrl)]
        public IFormFile? ProfileImage { get; set; }
    }
}

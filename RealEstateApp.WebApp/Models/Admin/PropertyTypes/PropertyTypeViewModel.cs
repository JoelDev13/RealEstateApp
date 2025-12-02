using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Web.Models.Admin.PropertyTypes
{
    public class PropertyTypeViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        public string Name { get; set; } = null!;

        [Display(Name = "Descripción")]
        public string? Description { get; set; }

        [Display(Name = "Activo")]
        public bool IsActive { get; set; }
    }
}

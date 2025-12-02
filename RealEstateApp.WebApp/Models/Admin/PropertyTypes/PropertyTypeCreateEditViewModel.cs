using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Web.Models.Admin.PropertyTypes
{
    public class PropertyTypeCreateEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
        [Display(Name = "Nombre")]
        public string Name { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Máximo 500 caracteres")]
        [Display(Name = "Descripción")]
        public string? Description { get; set; }

        [Display(Name = "Activo")]
        public bool IsActive { get; set; } = true;
    }
}

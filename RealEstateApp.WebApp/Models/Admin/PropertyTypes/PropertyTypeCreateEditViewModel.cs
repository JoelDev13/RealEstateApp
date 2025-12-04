using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Web.Models.Admin.PropertyTypes
{
    public class PropertyTypeCreateEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(500, ErrorMessage = "Máximo 500 caracteres")]
        public string Description { get; set; } = null!;
    }
}

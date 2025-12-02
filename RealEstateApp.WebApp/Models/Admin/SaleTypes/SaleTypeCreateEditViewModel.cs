using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Web.Models.Admin.SaleTypes
{
    public class SaleTypeCreateEditViewModel
    {
        public int Id { get; set; }   // 0 = Create, >0 = Edit

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

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.WebApp.Models.Property
{
    public class CreatePropertyViewModel
    {
        [Required(ErrorMessage = "El tipo de propiedad es requerido")]
        public string PropertyTypeId { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de venta es requerido")]
        public string SaleTypeId { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio es requerido")]
        [Range(1, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        [MinLength(10, ErrorMessage = "La descripción debe tener al menos 10 caracteres")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tamaño es requerido")]
        [Range(1, double.MaxValue, ErrorMessage = "El tamaño debe ser mayor a 0")]
        public decimal Size { get; set; }

        [Required(ErrorMessage = "El número de habitaciones es requerido")]
        [Range(1, 20, ErrorMessage = "Debe tener entre 1 y 20 habitaciones")]
        public int Bedrooms { get; set; }

        [Required(ErrorMessage = "El número de baños es requerido")]
        [Range(1, 20, ErrorMessage = "Debe tener entre 1 y 20 baños")]
        public int Bathrooms { get; set; }

        [Required(ErrorMessage = "Debe seleccionar al menos una mejora")]
        public List<string> SelectedImprovementIds { get; set; } = new();

        [Required(ErrorMessage = "Debe subir al menos una imagen")]
        [DataType(DataType.Upload)]
        public List<IFormFile> Images { get; set; } = new();

        public List<SelectListItem> PropertyTypes { get; set; } = new();
        public List<SelectListItem> SaleTypes { get; set; } = new();
        public List<SelectListItem> Improvements { get; set; } = new();
    }
}

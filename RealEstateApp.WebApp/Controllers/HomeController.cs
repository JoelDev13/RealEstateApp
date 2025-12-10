using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Dtos.Property;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.WebApp.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace RealEstateApp.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPropertyService _propertyService;
        private readonly IPropertyTypeService _propertyTypeService;
        private readonly IFavoriteService _favoriteService;
        private readonly IMapper _mapper;

        public HomeController(
            ILogger<HomeController> logger,
            IPropertyService propertyService,
            IPropertyTypeService propertyTypeService,
            IFavoriteService favoriteService,
            IMapper mapper)
        {
            _logger = logger;
            _propertyService = propertyService;
            _propertyTypeService = propertyTypeService;
            _favoriteService = favoriteService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(string searchCode, string propertyType, decimal? minPrice, decimal? maxPrice, int? bedrooms, int? bathrooms)
        {
            var allProperties = await _propertyService.GetAvailablePropertiesAsync();
            
            var properties = allProperties.AsEnumerable();
            
            if (!string.IsNullOrEmpty(searchCode))
            {
                properties = properties.Where(p => p.Code.Contains(searchCode, StringComparison.OrdinalIgnoreCase));
            }
            
            if (!string.IsNullOrEmpty(propertyType))
            {
                properties = properties.Where(p => p.PropertyType != null && p.PropertyType.Name.Contains(propertyType, StringComparison.OrdinalIgnoreCase));
            }
            
            if (minPrice.HasValue)
            {
                properties = properties.Where(p => p.Price >= minPrice.Value);
            }
            
            if (maxPrice.HasValue)
            {
                properties = properties.Where(p => p.Price <= maxPrice.Value);
            }
            
            if (bedrooms.HasValue)
            {
                properties = properties.Where(p => p.Bedrooms >= bedrooms.Value);
            }
            
            if (bathrooms.HasValue)
            {
                properties = properties.Where(p => p.Bathrooms >= bathrooms.Value);
            }
            
            // Verifica si el usuario esta autenticado
            var isAuthenticated = User.Identity?.IsAuthenticated ?? false;
            ViewBag.IsAuthenticated = isAuthenticated;
            
            // Guarda los valores del filtro para mantenerlos en el formulario
            ViewBag.SearchCode = searchCode;
            ViewBag.PropertyType = propertyType;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.Bedrooms = bedrooms;
            ViewBag.Bathrooms = bathrooms;
            
            // Obtiene los  tipos de propiedad para el dropdown
            ViewBag.PropertyTypes = await _propertyTypeService.GetAllAsync();
            
            if (isAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var favorites = await _favoriteService.GetClientFavoritesAsync(userId!);
                ViewBag.FavoritePropertyIds = favorites.Select(f => f.Id).ToList();
                ViewBag.UserName = User.FindFirstValue(ClaimTypes.Name) ?? "Usuario";
            }
            else
            {
                ViewBag.FavoritePropertyIds = new List<int>();
                ViewBag.UserName = null;
            }

            var propertyDtos = _mapper.Map<List<PropertyDto>>(properties.ToList());
            
            return View(propertyDtos);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

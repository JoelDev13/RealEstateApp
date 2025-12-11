using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Dtos.Property;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Domain.Enums;
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

        public async Task<IActionResult> Index([FromQuery] PropertyFiltersDto filters)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole(nameof(Roles.Administrador)))
                    return RedirectToAction("Index", "AdminHome");

                if (User.IsInRole(nameof(Roles.Agente)))
                    return RedirectToAction("Dashboard", "Agent");
            }

            // Propiedades disponibles
            var properties = await _propertyService.GetAvailablePropertiesAsync();
            var query = properties.AsEnumerable();

            // Filtros
            if (!string.IsNullOrWhiteSpace(filters.Code))
                query = query.Where(p => p.Code.Contains(filters.Code, StringComparison.OrdinalIgnoreCase));

            if (filters.PropertyTypeId.HasValue)
                query = query.Where(p => p.PropertyTypeId == filters.PropertyTypeId.Value);

            if (filters.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filters.MinPrice.Value);

            if (filters.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filters.MaxPrice.Value);

            if (filters.Bedrooms.HasValue)
                query = query.Where(p => p.Bedrooms >= filters.Bedrooms.Value);

            if (filters.Bathrooms.HasValue)
                query = query.Where(p => p.Bathrooms >= filters.Bathrooms.Value);

            query = query.OrderByDescending(p => p.CreatedAt);

            ViewBag.PropertyTypes = await _propertyTypeService.GetAllAsync();
            ViewBag.Filters = filters;

            var isAuthenticated = User.Identity?.IsAuthenticated ?? false;
            ViewBag.IsAuthenticated = isAuthenticated;

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

            var propertyDtos = _mapper.Map<List<PropertyDto>>(query.ToList());
            return View(propertyDtos);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Dtos.Property;
using RealEstateApp.Application.Interfaces.Services;
using System.Security.Claims;

namespace RealEstateApp.WebApp.Controllers
{
    [Authorize(Roles = "Cliente")]
    public class ClientPropertyController : Controller
    {
        private readonly IFavoriteService _favoriteService;
        private readonly IPropertyTypeService _propertyTypeService;
        private readonly IMapper _mapper;

        public ClientPropertyController(
            IFavoriteService favoriteService,
            IPropertyTypeService propertyTypeService,
            IMapper mapper)
        {
            _favoriteService = favoriteService;
            _propertyTypeService = propertyTypeService;
            _mapper = mapper;
        }

        public async Task<IActionResult> MyProperties([FromQuery] PropertyFiltersDto filters)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var favoriteProperties = await _favoriteService.GetClientFavoritesAsync(userId);

            var propertyDtos = _mapper.Map<List<PropertyDto>>(favoriteProperties);
            var query = propertyDtos.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(filters.Code))
                query = query.Where(p => p.Code.Contains(filters.Code, StringComparison.OrdinalIgnoreCase));

            if (filters.PropertyTypeId.HasValue)
            {
                var propertyTypes = await _propertyTypeService.GetAllAsync();
                var selectedType = propertyTypes.FirstOrDefault(pt => pt.Id == filters.PropertyTypeId.Value);
                if (selectedType != null)
                {
                    query = query.Where(p => p.PropertyType.Equals(selectedType.Name, StringComparison.OrdinalIgnoreCase));
                }
            }

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
            ViewBag.UserName = User.Identity?.Name ?? "Cliente";

            return View(query.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFavorite(int propertyId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var isFavorite = await _favoriteService.IsPropertyFavoriteAsync(userId, propertyId);

            if (isFavorite)
            {
                await _favoriteService.RemoveFromFavoritesAsync(userId, propertyId);
                TempData["Success"] = "Propiedad removida de favoritos";
            }
            else
            {
                await _favoriteService.AddToFavoritesAsync(userId, propertyId);
                TempData["Success"] = "Propiedad agregada a favoritos";
            }

            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }

            return RedirectToAction("MyProperties");
        }
    }
}
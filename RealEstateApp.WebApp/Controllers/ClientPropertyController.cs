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
        private readonly IPropertyService _propertyService;
        private readonly IMapper _mapper;

        public ClientPropertyController(
            IFavoriteService favoriteService,
            IPropertyService propertyService,
            IMapper mapper)
        {
            _favoriteService = favoriteService;
            _propertyService = propertyService;
            _mapper = mapper;
        }

        // GET: /ClientProperty/MyProperties
        public async Task<IActionResult> MyProperties()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var favorites = await _favoriteService.GetClientFavoritesAsync(userId!);
            
            var propertyDtos = _mapper.Map<List<PropertyDto>>(favorites);

            ViewBag.UserName = User.FindFirstValue(ClaimTypes.Name) ?? "Usuario";

            return View(propertyDtos);
        }

        // POST: /ClientProperty/ToggleFavorite
        [HttpPost]
        public async Task<IActionResult> ToggleFavorite(int propertyId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                var isFavorite = await _favoriteService.IsPropertyFavoriteAsync(userId!, propertyId);
                
                if (isFavorite)
                {
                    await _favoriteService.RemoveFromFavoritesAsync(userId!, propertyId);
                }
                else
                {
                    await _favoriteService.AddToFavoritesAsync(userId!, propertyId);
                }

                // Redirigir de vuelta al Home
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }
    }

    public class ToggleFavoriteRequest
    {
        public int PropertyId { get; set; }
    }
}

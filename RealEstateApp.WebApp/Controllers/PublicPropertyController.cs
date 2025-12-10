using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Interfaces.Services;
using System.Security.Claims;

namespace RealEstateApp.WebApp.Controllers
{
    public class PublicPropertyController : Controller
    {
        private readonly IPropertyService _propertyService;
        private readonly IFavoriteService _favoriteService;
        private readonly IMapper _mapper;
        private readonly IAgentService _agentService;

        public PublicPropertyController(
            IPropertyService propertyService,
            IFavoriteService favoriteService,
            IMapper mapper,
            IAgentService agentService)
        {
            _propertyService = propertyService;
            _favoriteService = favoriteService;
            _mapper = mapper;
            _agentService = agentService;
        }

        // GET: /PublicProperty/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var property = await _propertyService.GetPropertyDetailAsync(id);
            if (property == null)
                return NotFound();

            // Verificar si el usuario está autenticado
            var isAuthenticated = User.Identity?.IsAuthenticated ?? false;
            ViewBag.IsAuthenticated = isAuthenticated;
            ViewBag.IsClient = User.IsInRole("Cliente");

            if (isAuthenticated && User.IsInRole("Cliente"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ViewBag.IsFavorite = await _favoriteService.IsPropertyFavoriteAsync(userId!, id);
            }
            else
            {
                ViewBag.IsFavorite = false;
            }

            // Obtener información del agente
            var agent = await _agentService.GetAgentByIdAsync(property.AgentId);
            ViewBag.AgentName = agent?.FirstName + " " + agent?.LastName ?? "Agente";

            return View(property);
        }

        // POST: /PublicProperty/ToggleFavorite
        [HttpPost]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> ToggleFavorite(int propertyId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var isFavorite = await _favoriteService.IsPropertyFavoriteAsync(userId!, propertyId);
            
            if (isFavorite)
            {
                await _favoriteService.RemoveFromFavoritesAsync(userId!, propertyId);
                TempData["Success"] = "Propiedad removida de favoritos";
            }
            else
            {
                await _favoriteService.AddToFavoritesAsync(userId!, propertyId);
                TempData["Success"] = "Propiedad agregada a favoritos";
            }

            return RedirectToAction("Details", new { id = propertyId });
        }
    }
}

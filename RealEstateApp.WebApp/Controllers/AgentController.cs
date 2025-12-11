using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Dtos.Agent;
using RealEstateApp.Application.Dtos.Property;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Domain.Enums;
using RealEstateApp.WebApp.Models.Agents;
using System.Security.Claims;

namespace RealEstateApp.WebApp.Controllers
{
    [Authorize(Roles = nameof(Roles.Agente))]
    public class AgentController : Controller
    {
        private readonly IPropertyService _propertyService;
        private readonly IAgentService _agentService;
        private readonly IMapper _mapper;
        private readonly IPropertyTypeService _propertyTypeService;


        public AgentController(IPropertyService propertyService, IAgentService agentService, IMapper mapper, IPropertyTypeService propertyTypeService)
        {
            _propertyService = propertyService;
            _agentService = agentService;
            _mapper = mapper;
            _propertyTypeService = propertyTypeService;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var properties = await _propertyService.GetPropertiesByAgentAsync(userId);

            var mappedProperties = properties.Select(p => new AgentPropertyViewModel
            {
                Id = p.Id.ToString(),
                Code = p.Code ?? string.Empty,
                PropertyType = p.PropertyType?.Name ?? "Sin tipo",
                SaleType = p.SaleType?.Name ?? "Sin tipo de venta",
                Price = p.Price,
                Bedrooms = p.Bedrooms,
                Bathrooms = p.Bathrooms,
                Size = (decimal)p.SizeInSquareMeters,
                MainImageUrl = p.Images?.FirstOrDefault(img => img.IsPrimary)?.Url ??
                              p.Images?.FirstOrDefault()?.Url ??
                              string.Empty,
                IsSold = p.IsSold,
                Description = p.Description ?? string.Empty,
                CreatedAt = p.CreatedAt
            }).ToList();

            var viewModel = new AgentDashboardViewModel
            {
                Properties = mappedProperties
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Properties([FromQuery] PropertyFiltersDto filters)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var properties = await _propertyService.GetPropertiesByAgentAsync(userId);

            // ✅ FILTRAR SOLO DISPONIBLES (NO VENDIDAS)
            var query = properties.Where(p => !p.IsSold).AsEnumerable();

            // Aplicar filtros
            if (!string.IsNullOrWhiteSpace(filters.Code))
                query = query.Where(p => p.Code.Contains(filters.Code, StringComparison.OrdinalIgnoreCase));

            if (filters.PropertyTypeId.HasValue && filters.PropertyTypeId.Value > 0)
                query = query.Where(p => p.PropertyTypeId == filters.PropertyTypeId.Value);

            if (filters.MinPrice.HasValue && filters.MinPrice.Value > 0)
                query = query.Where(p => p.Price >= filters.MinPrice.Value);

            if (filters.MaxPrice.HasValue && filters.MaxPrice.Value > 0)
                query = query.Where(p => p.Price <= filters.MaxPrice.Value);

            if (filters.Bedrooms.HasValue && filters.Bedrooms.Value > 0)
                query = query.Where(p => p.Bedrooms >= filters.Bedrooms.Value);

            if (filters.Bathrooms.HasValue && filters.Bathrooms.Value > 0)
                query = query.Where(p => p.Bathrooms >= filters.Bathrooms.Value);

            query = query.OrderByDescending(p => p.CreatedAt);

            ViewBag.PropertyTypes = await _propertyTypeService.GetAllAsync();
            ViewBag.Filters = filters;

            // Mapear PropertyDto a AgentPropertyViewModel
            var mappedProperties = query.Select(p => new AgentPropertyViewModel
            {
                Id = p.Id.ToString(),
                Code = p.Code ?? string.Empty,
                PropertyType = p.PropertyType?.Name ?? "Sin tipo",
                SaleType = p.SaleType?.Name ?? "Sin tipo de venta",
                Price = p.Price,
                Bedrooms = p.Bedrooms,
                Bathrooms = p.Bathrooms,
                Size = (decimal)p.SizeInSquareMeters,
                MainImageUrl = p.Images?.FirstOrDefault(img => img.IsPrimary)?.Url ??
                              p.Images?.FirstOrDefault()?.Url ??
                              string.Empty,
                IsSold = p.IsSold,
                Description = p.Description ?? string.Empty,
                CreatedAt = p.CreatedAt
            }).ToList();

            var viewModel = new AgentPropertiesViewModel
            {
                Properties = mappedProperties
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var profileDto = await _agentService.GetProfileAsync(userId);
            var viewModel = _mapper.Map<AgentProfileViewModel>(profileDto);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(AgentProfileViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                var dto = _mapper.Map<AgentProfileDto>(model);
                var result = await _agentService.UpdateProfileAsync(userId, dto);

                if (result)
                {
                    TempData["SuccessMessage"] = "Perfil actualizado exitosamente";
                    return RedirectToAction(nameof(Dashboard));
                }
                else
                {
                    ModelState.AddModelError("", "Error al actualizar el perfil. Por favor, intente nuevamente.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al guardar el perfil: {ex.Message}");
                return View(model);
            }
        }
    }
}

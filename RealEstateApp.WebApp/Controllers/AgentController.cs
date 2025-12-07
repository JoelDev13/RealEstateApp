using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Application.Dtos.Agent;
using RealEstateApp.Domain.Enums;
using RealEstateApp.WebApp.Models.Agents;
using System.Security.Claims;
using AutoMapper;

namespace RealEstateApp.WebApp.Controllers
{
    [Authorize(Roles = nameof(Roles.Agente))]
    public class AgentController : Controller
    {
        private readonly IPropertyService _propertyService;
        private readonly IAgentService _agentService;
        private readonly IMapper _mapper;

        public AgentController(IPropertyService propertyService, IAgentService agentService, IMapper mapper)
        {
            _propertyService = propertyService;
            _agentService = agentService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var properties = await _propertyService.GetPropertiesByAgentAsync(userId);
            
            var viewModel = new AgentDashboardViewModel
            {
                Properties = properties.Select(p => new AgentPropertyViewModel
                {
                    Id = p.Id.ToString(),
                    Code = p.Code,
                    PropertyType = p.PropertyType?.Name ?? "",
                    SaleType = p.SaleType?.Name ?? "",
                    Price = p.Price,
                    Bedrooms = p.Bedrooms,
                    Bathrooms = p.Bathrooms,
                    Size = (decimal)p.SizeInSquareMeters,
                    MainImageUrl = p.Images.FirstOrDefault()?.Url ?? "/images/default-property.jpg",
                    IsSold = p.IsSold,
                    Description = p.Description
                }).ToList()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Properties()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var properties = await _propertyService.GetAvailablePropertiesByAgentAsync(userId);
            
            var viewModel = new AgentPropertiesViewModel
            {
                Properties = properties.Select(p => new AgentPropertyViewModel
                {
                    Id = p.Id.ToString(),
                    Code = p.Code,
                    PropertyType = p.PropertyType?.Name ?? "",
                    SaleType = p.SaleType?.Name ?? "",
                    Price = p.Price,
                    Bedrooms = p.Bedrooms,
                    Bathrooms = p.Bathrooms,
                    Size = (decimal)p.SizeInSquareMeters,
                    MainImageUrl = p.Images.FirstOrDefault()?.Url ?? "/images/default-property.jpg",
                    IsSold = false,
                    Description = p.Description
                }).ToList()
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

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Domain.Enums;
using RealEstateApp.WebApp.Models.Agents;

namespace RealEstateApp.WebApp.Controllers
{
    [Authorize(Roles = nameof(Roles.Administrador))]
    public class AdminAgentsController : Controller
    {
        private readonly IAgentAdminService _agentAdminService;
        private readonly IMapper _mapper;

        public AdminAgentsController(
            IAgentAdminService agentAdminService,
            IMapper mapper)
        {
            _agentAdminService = agentAdminService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var result = await _agentAdminService.GetAgentsAsync();

            if (!result.Succeeded)
            {
                TempData["Error"] = string.Join(". ", result.Errors ?? new List<string>());
                return View(new List<AgentListItemViewModel>());
            }

            var agents = result.Data ?? new List<RealEstateApp.Application.Dtos.Agents.AgentAdminDto>();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                agents = agents
                    .Where(a =>
                        a.FirstName.ToLower().Contains(term) ||
                        a.LastName.ToLower().Contains(term) ||
                        a.Email.ToLower().Contains(term))
                    .ToList();
            }

            var model = _mapper.Map<List<AgentListItemViewModel>>(agents);
            ViewBag.Search = search;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["Error"] = "Id de agente inválido.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _agentAdminService.ToggleStatusAsync(id);

            if (!result.Succeeded)
            {
                TempData["Error"] = string.Join(". ", result.Errors ?? new List<string>());
            }
            else
            {
                TempData["Success"] = "Estado del agente actualizado correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["Error"] = "Id de agente inválido.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _agentAdminService.GetAgentsAsync();
            if (!result.Succeeded || result.Data == null)
            {
                TempData["Error"] = "No se pudo cargar la información del agente.";
                return RedirectToAction(nameof(Index));
            }

            var agentDto = result.Data.FirstOrDefault(a => a.Id == id);
            if (agentDto == null)
            {
                TempData["Error"] = "El agente indicado no existe.";
                return RedirectToAction(nameof(Index));
            }

            var model = _mapper.Map<AgentListItemViewModel>(agentDto);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["Error"] = "Id de agente inválido.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _agentAdminService.DeleteAgentWithPropertiesAsync(id);

            if (!result.Succeeded)
            {
                TempData["Error"] = string.Join(". ", result.Errors ?? new List<string>());
            }
            else
            {
                TempData["Success"] = "El agente y todas sus propiedades fueron eliminados correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.WebApp.Models;

namespace RealEstateApp.WebApp.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminHomeController : Controller
    {
        private readonly IAdminDashboardService _adminDashboardService;
        private readonly IMapper _mapper;

        public AdminHomeController(
            IAdminDashboardService adminDashboardService,
            IMapper mapper)
        {
            _adminDashboardService = adminDashboardService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var dto = await _adminDashboardService.GetDashboardAsync();
                var viewModel = _mapper.Map<AdminDashboardViewModel>(dto);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar los datos del dashboard. Por favor, intente nuevamente.";

                var viewModel = new AdminDashboardViewModel();
                return View(viewModel);
            }
        }
    }
}
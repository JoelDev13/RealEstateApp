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

        public async Task<IActionResult> Index()
        {
            var dto = await _adminDashboardService.GetDashboardAsync();
            var vm = _mapper.Map<AdminDashboardViewModel>(dto);

            return View(vm);
        }
    }
}

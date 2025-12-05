using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Dtos.DeveloperUsers;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.WebApp.Models.Admin.DeveloperUser;

namespace RealEstateApp.WebApp.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class DeveloperUsersController : Controller
    {
        private readonly IDeveloperUserService _developerService;
        private readonly IMapper _mapper;

        public DeveloperUsersController(IDeveloperUserService developerService, IMapper mapper)
        {
            _developerService = developerService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var developers = await _developerService.GetAllAsync();
            var vm = _mapper.Map<List<DevUserViewModel>>(developers);
            return View(vm);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DevUserCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var dto = _mapper.Map<DeveloperUserCreateDto>(model);
            await _developerService.CreateAsync(dto);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var developer = await _developerService.GetByIdAsync(id);
            if (developer is null) return NotFound();

            var vm = _mapper.Map<DevUserEditViewModel>(developer);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DevUserEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var currentUserId = User?.Identity?.Name ?? string.Empty;

            var dto = _mapper.Map<DeveloperUserUpdateDto>(model);
            await _developerService.UpdateAsync(dto, currentUserId);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string id, bool isActive)
        {
            var currentUserId = User?.Identity?.Name ?? string.Empty;
            await _developerService.SetActiveStatusAsync(id, isActive, currentUserId);

            return RedirectToAction(nameof(Index));
        }
    }
}

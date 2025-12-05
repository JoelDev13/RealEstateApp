using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Dtos.Improvements;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Web.Models.Admin.Improvements;

namespace RealEstateApp.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminImprovementsController : Controller
    {
        private readonly IImprovementService _improvementService;
        private readonly IMapper _mapper;

        public AdminImprovementsController(IImprovementService improvementService, IMapper mapper)
        {
            _improvementService = improvementService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _improvementService.GetAllAsync();
            var vm = _mapper.Map<List<ImprovementViewModel>>(result);

            return View(vm);
        }

        public IActionResult Create()
        {
            var vm = new ImprovementCreateEditViewModel();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ImprovementCreateEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = _mapper.Map<ImprovementDto>(model);
            await _improvementService.CreateAsync(dto);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _improvementService.GetByIdAsync(id);

            if (dto == null)
                return NotFound();

            var vm = _mapper.Map<ImprovementCreateEditViewModel>(dto);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ImprovementCreateEditViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            var dto = _mapper.Map<ImprovementDto>(model);
            await _improvementService.UpdateAsync(dto);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            await _improvementService.ToggleStatusAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _improvementService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

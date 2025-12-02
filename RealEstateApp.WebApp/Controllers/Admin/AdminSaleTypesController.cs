using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Dtos.SaleTypes;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Web.Models.Admin.SaleTypes;

namespace RealEstateApp.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminSaleTypesController : Controller
    {
        private readonly ISaleTypeService _saleTypeService;
        private readonly IMapper _mapper;

        public AdminSaleTypesController(ISaleTypeService saleTypeService, IMapper mapper)
        {
            _saleTypeService = saleTypeService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _saleTypeService.GetAllAsync();
            var vm = _mapper.Map<List<SaleTypeViewModel>>(result);
            return View("Admin/SaleTypes/Index", vm);
        }

        public IActionResult Create()
        {
            return View("Admin/SaleTypes/Create", new SaleTypeCreateEditViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaleTypeCreateEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Admin/SaleTypes/Create", model);

            var dto = _mapper.Map<SaleTypeDto>(model);
            await _saleTypeService.CreateAsync(dto);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _saleTypeService.GetByIdAsync(id);
            if (dto == null)
                return NotFound();

            var vm = _mapper.Map<SaleTypeCreateEditViewModel>(dto);
            return View("Admin/SaleTypes/Edit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SaleTypeCreateEditViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View("Admin/SaleTypes/Edit", model);

            var dto = _mapper.Map<SaleTypeDto>(model);
            await _saleTypeService.UpdateAsync(dto);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            await _saleTypeService.ToggleStatusAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _saleTypeService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Dtos.PropertyTypes;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Web.Models.Admin.PropertyTypes;

namespace RealEstateApp.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminPropertyTypesController : Controller
    {
        private readonly IPropertyTypeService _propertyTypeService;
        private readonly IMapper _mapper;

        public AdminPropertyTypesController(IPropertyTypeService propertyTypeService, IMapper mapper)
        {
            _propertyTypeService = propertyTypeService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _propertyTypeService.GetAllAsync();

            var viewModel = _mapper.Map<List<PropertyTypeViewModel>>(result);

            return View("Admin/PropertyTypes/Index", viewModel);
        }

        public IActionResult Create()
        {
            var vm = new PropertyTypeCreateEditViewModel();
            return View("Admin/PropertyTypes/Create", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PropertyTypeCreateEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Admin/PropertyTypes/Create", model);

            var dto = _mapper.Map<PropertyTypeDto>(model);

            await _propertyTypeService.CreateAsync(dto);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _propertyTypeService.GetByIdAsync(id);

            if (dto == null)
                return NotFound();

            var vm = _mapper.Map<PropertyTypeCreateEditViewModel>(dto);

            return View("Admin/PropertyTypes/Edit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PropertyTypeCreateEditViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View("Admin/PropertyTypes/Edit", model);

            var dto = _mapper.Map<PropertyTypeDto>(model);

            await _propertyTypeService.UpdateAsync(dto);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            await _propertyTypeService.ToggleStatusAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _propertyTypeService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

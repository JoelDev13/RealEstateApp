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
        private readonly IPropertyTypeService _service;
        private readonly IMapper _mapper;

        public AdminPropertyTypesController(IPropertyTypeService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _service.GetAllAsync();
            var vm = _mapper.Map<List<PropertyTypeViewModel>>(list);
            return View(vm);
        }

        public IActionResult Create()
        {
            return View(new PropertyTypeCreateEditViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PropertyTypeCreateEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var dto = _mapper.Map<PropertyTypeDto>(model);
            await _service.CreateAsync(dto);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();

            var vm = _mapper.Map<PropertyTypeCreateEditViewModel>(dto);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PropertyTypeCreateEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var dto = _mapper.Map<PropertyTypeDto>(model);
            await _service.UpdateAsync(dto);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();

            var vm = _mapper.Map<PropertyTypeViewModel>(dto);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

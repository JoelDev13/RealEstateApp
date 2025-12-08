using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Application.Dtos.Property;
using RealEstateApp.Domain.Enums;
using RealEstateApp.WebApp.Models.Property;
using System.Security.Claims;
using AutoMapper;

namespace RealEstateApp.WebApp.Controllers
{
    [Authorize(Roles = nameof(Roles.Agente))]
    public class PropertyController : Controller
    {
        private readonly IPropertyService _propertyService;
        private readonly IPropertyTypeService _propertyTypeService;
        private readonly ISaleTypeService _saleTypeService;
        private readonly IImprovementService _improvementService;
        private readonly IMapper _mapper;

        public PropertyController(
            IPropertyService propertyService,
            IPropertyTypeService propertyTypeService,
            ISaleTypeService saleTypeService,
            IImprovementService improvementService,
            IMapper mapper)
        {
            _propertyService = propertyService;
            _propertyTypeService = propertyTypeService;
            _saleTypeService = saleTypeService;
            _improvementService = improvementService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Create()
        {
            var propertyTypes = await _propertyTypeService.GetAllAsync();
            var saleTypes = await _saleTypeService.GetAllAsync();
            var improvements = await _improvementService.GetAllAsync();

            var viewModel = new CreatePropertyViewModel
            {
                PropertyTypes = propertyTypes.Select(pt => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = pt.Id.ToString(),
                    Text = pt.Name
                }).ToList(),
                SaleTypes = saleTypes.Select(st => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = st.Id.ToString(),
                    Text = st.Name
                }).ToList(),
                Improvements = improvements.Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = i.Id.ToString(),
                    Text = i.Name
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePropertyViewModel model)
        {
            var propertyTypes = await _propertyTypeService.GetAllAsync();
            var saleTypes = await _saleTypeService.GetAllAsync();
            var improvements = await _improvementService.GetAllAsync();

            if (!ModelState.IsValid)
            {
                model.PropertyTypes = propertyTypes.Select(pt => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = pt.Id.ToString(),
                    Text = pt.Name
                }).ToList();
                model.SaleTypes = saleTypes.Select(st => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = st.Id.ToString(),
                    Text = st.Name
                }).ToList();
                model.Improvements = improvements.Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = i.Id.ToString(),
                    Text = i.Name
                }).ToList();

                return View(model);
            }

            if (model.Images == null || !model.Images.Any())
            {
                ModelState.AddModelError("Images", "Debe seleccionar al menos una imagen");
                
                model.PropertyTypes = propertyTypes.Select(pt => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = pt.Id.ToString(),
                    Text = pt.Name
                }).ToList();
                model.SaleTypes = saleTypes.Select(st => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = st.Id.ToString(),
                    Text = st.Name
                }).ToList();
                model.Improvements = improvements.Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = i.Id.ToString(),
                    Text = i.Name
                }).ToList();

                return View(model);
            }

            if (model.Images.Count > 4)
            {
                ModelState.AddModelError("Images", "Puede seleccionar máximo 4 imágenes");
                
                model.PropertyTypes = propertyTypes.Select(pt => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = pt.Id.ToString(),
                    Text = pt.Name
                }).ToList();
                model.SaleTypes = saleTypes.Select(st => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = st.Id.ToString(),
                    Text = st.Name
                }).ToList();
                model.Improvements = improvements.Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = i.Id.ToString(),
                    Text = i.Name
                }).ToList();

                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var dto = _mapper.Map<CreatePropertyDto>(model);
            dto.AgentId = userId;

            await _propertyService.CreatePropertyAsync(dto, userId);

            TempData["Success"] = "Propiedad creada exitosamente";
            return RedirectToAction("Properties", "Agent");
        }

        public async Task<IActionResult> Edit(string id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (property.AgentId != userId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var propertyTypes = await _propertyTypeService.GetAllAsync();
            var saleTypes = await _saleTypeService.GetAllAsync();
            var improvements = await _improvementService.GetAllAsync();

            var viewModel = _mapper.Map<EditPropertyViewModel>(property);
            
            viewModel.PropertyTypes = propertyTypes.Select(pt => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = pt.Id.ToString(),
                Text = pt.Name,
                Selected = pt.Id == property.PropertyTypeId
            }).ToList();
            
            viewModel.SaleTypes = saleTypes.Select(st => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = st.Id.ToString(),
                Text = st.Name,
                Selected = st.Id == property.SaleTypeId
            }).ToList();
            
            viewModel.Improvements = improvements.Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = i.Id.ToString(),
                Text = i.Name,
                Selected = property.Improvements.Any(imp => imp.Id == i.Id)
            }).ToList();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditPropertyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var propertyTypes = await _propertyTypeService.GetAllAsync();
                var saleTypes = await _saleTypeService.GetAllAsync();
                var improvements = await _improvementService.GetAllAsync();

                model.PropertyTypes = propertyTypes.Select(pt => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = pt.Id.ToString(),
                    Text = pt.Name
                }).ToList();
                model.SaleTypes = saleTypes.Select(st => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = st.Id.ToString(),
                    Text = st.Name
                }).ToList();
                model.Improvements = improvements.Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = i.Id.ToString(),
                    Text = i.Name
                }).ToList();

                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var property = await _propertyService.GetPropertyByIdAsync(model.Id);
            
            if (property == null || property.AgentId != userId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var dto = _mapper.Map<UpdatePropertyDto>(model);
            
            await _propertyService.UpdatePropertyAsync(property.Id, dto, userId);

            TempData["Success"] = "Propiedad actualizada exitosamente";
            return RedirectToAction("Properties", "Agent");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (property.AgentId != userId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            await _propertyService.DeletePropertyAsync(property.Id, userId);

            TempData["Success"] = "Propiedad eliminada exitosamente";
            return RedirectToAction("Properties", "Agent");
        }

        public async Task<IActionResult> Details(string id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (property.AgentId != userId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var viewModel = _mapper.Map<PropertyDetailViewModel>(property);
            return View(viewModel);
        }
    }
}

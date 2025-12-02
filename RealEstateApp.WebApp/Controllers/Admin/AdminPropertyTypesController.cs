using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Features.PropertyTypes.Commands.CreatePropertyType;
using RealEstateApp.Application.Features.PropertyTypes.Commands.DeletePropertyType;
using RealEstateApp.Application.Features.PropertyTypes.Commands.TogglePropertyTypeStatus;
using RealEstateApp.Application.Features.PropertyTypes.Commands.UpdatePropertyType;
using RealEstateApp.Application.Features.PropertyTypes.Queries.GetPropertyTypeById;
using RealEstateApp.Application.Features.PropertyTypes.Queries.GetPropertyTypes;
using RealEstateApp.Web.Models.Admin.PropertyTypes;

namespace RealEstateApp.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminPropertyTypesController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public AdminPropertyTypesController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _mediator.Send(new GetPropertyTypesQuery());

            var viewModel = _mapper.Map<List<PropertyTypeViewModel>>(result);

            return View("Admin/PropertyTypes/Index", viewModel);
        }

        public async Task<IActionResult> Create()
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

            var command = _mapper.Map<CreatePropertyTypeCommand>(model);

            await _mediator.Send(command);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _mediator.Send(new GetPropertyTypeByIdQuery { Id = id });

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

            var command = _mapper.Map<UpdatePropertyTypeCommand>(model);

            await _mediator.Send(command);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            await _mediator.Send(new TogglePropertyTypeStatusCommand { Id = id });
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeletePropertyTypeCommand { Id = id });
            return RedirectToAction(nameof(Index));
        }
    }
}

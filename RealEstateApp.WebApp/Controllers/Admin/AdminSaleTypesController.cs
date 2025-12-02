using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Features.SaleTypes.Commands.CreateSaleType;
using RealEstateApp.Application.Features.SaleTypes.Commands.DeleteSaleType;
using RealEstateApp.Application.Features.SaleTypes.Commands.ToggleSaleTypeStatus;
using RealEstateApp.Application.Features.SaleTypes.Commands.UpdateSaleType;
using RealEstateApp.Application.Features.SaleTypes.Queries.GetSaleTypeById;
using RealEstateApp.Application.Features.SaleTypes.Queries.GetSaleTypes;
using RealEstateApp.Web.Models.Admin.SaleTypes;

namespace RealEstateApp.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminSaleTypesController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public AdminSaleTypesController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _mediator.Send(new GetSaleTypesQuery());

            var vm = _mapper.Map<List<SaleTypeViewModel>>(result);

            return View("Admin/SaleTypes/Index", vm);
        }

        public IActionResult Create()
        {
            var vm = new SaleTypeCreateEditViewModel();
            return View("Admin/SaleTypes/Create", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaleTypeCreateEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Admin/SaleTypes/Create", model);

            var command = _mapper.Map<CreateSaleTypeCommand>(model);

            await _mediator.Send(command);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _mediator.Send(new GetSaleTypeByIdQuery { Id = id });

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

            var command = _mapper.Map<UpdateSaleTypeCommand>(model);

            await _mediator.Send(command);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            await _mediator.Send(new ToggleSaleTypeStatusCommand { Id = id });
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteSaleTypeCommand { Id = id });
            return RedirectToAction(nameof(Index));
        }
    }
}

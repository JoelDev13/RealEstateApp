using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Features.Improvements.Commands.CreateImprovement;
using RealEstateApp.Application.Features.Improvements.Commands.DeleteImprovement;
using RealEstateApp.Application.Features.Improvements.Commands.ToggleImprovementStatus;
using RealEstateApp.Application.Features.Improvements.Commands.UpdateImprovement;
using RealEstateApp.Application.Features.Improvements.Queries.GetImprovementById;
using RealEstateApp.Application.Features.Improvements.Queries.GetImprovements;
using RealEstateApp.Web.Models.Admin.Improvements;

namespace RealEstateApp.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminImprovementsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public AdminImprovementsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _mediator.Send(new GetImprovementsQuery());
            var vm = _mapper.Map<List<ImprovementViewModel>>(result);

            return View("Admin/Improvements/Index", vm);
        }

        public IActionResult Create()
        {
            var vm = new ImprovementCreateEditViewModel();
            return View("Admin/Improvements/Create", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ImprovementCreateEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Admin/Improvements/Create", model);

            var command = _mapper.Map<CreateImprovementCommand>(model);
            await _mediator.Send(command);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _mediator.Send(new GetImprovementByIdQuery { Id = id });
            var vm = _mapper.Map<ImprovementCreateEditViewModel>(dto);

            return View("Admin/Improvements/Edit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ImprovementCreateEditViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View("Admin/Improvements/Edit", model);

            var command = _mapper.Map<UpdateImprovementCommand>(model);
            await _mediator.Send(command);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            await _mediator.Send(new ToggleImprovementStatusCommand { Id = id });
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteImprovementCommand { Id = id });
            return RedirectToAction(nameof(Index));
        }
    }
}

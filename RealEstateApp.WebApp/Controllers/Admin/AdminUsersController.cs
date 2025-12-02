using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Dtos.AdminUsers;
using RealEstateApp.Application.Exceptions;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.WebApp.Models.Admin.AdminUsers;
using System.Security.Claims;

namespace RealEstateApp.WebApp.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminUsersController : Controller
    {
        private readonly IAdminUserService _adminUserService;
        private readonly IMapper _mapper;

        public AdminUsersController(
            IAdminUserService adminUserService,
            IMapper mapper)
        {
            _adminUserService = adminUserService;
            _mapper = mapper;
        }

        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public async Task<IActionResult> Index()
        {
            var adminsDto = await _adminUserService.GetAllAsync();
            var viewModel = _mapper.Map<List<AdminUserViewModel>>(adminsDto);

            ViewBag.CurrentUserId = GetCurrentUserId();

            return View(viewModel);
        }

        public IActionResult Create()
        {
            var vm = new AdminUserCreateViewModel();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminUserCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var dto = _mapper.Map<AdminUserCreateDto>(model);
                await _adminUserService.CreateAsync(dto);

                return RedirectToAction(nameof(Index));
            }
            catch (ApiException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Ocurrió un error al crear el administrador.");
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var currentUserId = GetCurrentUserId();

            if (currentUserId == id)
            {
                TempData["ErrorMessage"] = "No puede editar su propio usuario administrador.";
                return RedirectToAction(nameof(Index));
            }

            var adminDto = await _adminUserService.GetByIdAsync(id);
            if (adminDto is null)
                return NotFound();

            var vm = _mapper.Map<AdminUserEditViewModel>(adminDto);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminUserEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var currentUserId = GetCurrentUserId();

            try
            {
                var dto = _mapper.Map<AdminUserUpdateDto>(model);
                await _adminUserService.UpdateAsync(dto, currentUserId!);

                return RedirectToAction(nameof(Index));
            }
            catch (ApiException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Ocurrió un error al editar el administrador.");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string id, bool activate)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var currentUserId = GetCurrentUserId();

            try
            {
                await _adminUserService.SetActiveStatusAsync(id, activate, currentUserId!);
            }
            catch (ApiException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Ocurrió un error al cambiar el estado del administrador.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Dtos.Auth;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Infrastructure.Identity.Entities;
using RealEstateApp.WebApp.Helpers;

namespace RealEstateApp.WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IBaseAccountService _accountService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMapper _mapper;

        public AccountController(
            IBaseAccountService accountService,
            SignInManager<AppUser> signInManager,
            IMapper mapper)
        {
            _accountService = accountService;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Register()
        {
            var model = new UserSaveDto();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserSaveDto model, IFormFile? profilePicture)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Verifica si las contraseñas coinciden
                    if (model.Password != model.ConfirmPassword)
                    {
                        ModelState.AddModelError("ConfirmPassword", "Las contraseñas no coinciden");
                        return View(model);
                    }

                    // Valid el tipo de usuario
                    if (!string.Equals(model.UserType, nameof(Roles.Cliente), StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(model.UserType, nameof(Roles.Agente), StringComparison.OrdinalIgnoreCase))
                    {
                        ModelState.AddModelError("UserType", "Tipo de usuario no válido");
                        return View(model);
                    }

                    // Valida cédula - debe tener exactamente 11 digitos
                    if (string.IsNullOrWhiteSpace(model.Cedula))
                    {
                        ModelState.AddModelError("Cedula", "La cédula es requerida");
                        return View(model);
                    }

                    string cedulaClean = model.Cedula.Replace("-", "").Replace(" ", "").Trim();
                    if (cedulaClean.Length != 11 || !cedulaClean.All(char.IsDigit))
                    {
                        ModelState.AddModelError("Cedula", "La cédula debe tener exactamente 11 dígitos numéricos");
                        return View(model);
                    }

                    model.Cedula = cedulaClean;

                    // Registra el usuario sin imagen primero para obtener el ID
                    var origin = Request.Scheme + "://" + Request.Host.Value;
                    var result = await _accountService.RegisterUser(model, origin);

                    if (result.Succeeded)
                    {
                        // Si se registró correctamente y hay una imagen, subirla
                        if (profilePicture != null && profilePicture.Length > 0 && result.Data != null)
                        {
                            string? imagePath = FileHandler.Upload(profilePicture, result.Data.Id, "avatars");

                            if (!string.IsNullOrEmpty(imagePath))
                            {
                                model.Id = result.Data.Id;
                                model.ProfilePicture = imagePath;
                                model.Password = null;
                                model.ConfirmPassword = null;

                                await _accountService.EditUser(model, origin);
                            }
                        }

                        // Mensaje segun el tipo de usuario
                        if (string.Equals(model.UserType, nameof(Roles.Cliente), StringComparison.OrdinalIgnoreCase))
                        {
                            TempData["SuccessMessage"] = "¡Registro exitoso! Por favor confirma tu correo electrónico para activar tu cuenta";
                        }
                        else if (string.Equals(model.UserType, nameof(Roles.Agente), StringComparison.OrdinalIgnoreCase))
                        {
                            TempData["SuccessMessage"] = "¡Registro de agente exitoso! Tu cuenta está pendiente de aprobación por un administrador";
                        }

                        return RedirectToAction("Login");
                    }
                    else
                    {
                        foreach (var error in result.Errors ?? new List<string>())
                        {
                            ModelState.AddModelError("", error);
                        }
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                // Log del error para debugging
                ModelState.AddModelError("", $"Ocurrió un error durante el registro: {ex.Message}");
                if (ex.InnerException != null)
                {
                    ModelState.AddModelError("", $"Detalle: {ex.InnerException.Message}");
                }
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDto model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _accountService.LoginAsync(model);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    // Redirigir segun el rol del usuario
                    var userDto = result.Data;
                    if (userDto != null)
                    {
                        if (string.Equals(userDto.Role, nameof(Roles.Administrador), StringComparison.OrdinalIgnoreCase))
                        {
                            return RedirectToAction("Index", "AdminHome");
                        }
                        else if (string.Equals(userDto.Role, nameof(Roles.Agente), StringComparison.OrdinalIgnoreCase))
                        {
                            return RedirectToAction("Dashboard", "Agent");
                        }
                        else if (string.Equals(userDto.Role, nameof(Roles.Cliente), StringComparison.OrdinalIgnoreCase))
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors ?? new List<string>())
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Enlace de confirmación inválido";
                return RedirectToAction("Login");
            }

            var result = await _accountService.ConfirmAccountAsync(userId, token);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "¡Correo confirmado exitosamente! Tu cuenta ha sido activada. Ahora puedes iniciar sesión";
            }
            else
            {
                TempData["ErrorMessage"] = "No se pudo activar la cuenta. Por favor intenta nuevamente o contacta soporte";
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestDto model)
        {
            if (ModelState.IsValid)
            {
                var origin = Request.Scheme + "://" + Request.Host.Value;
                var result = await _accountService.ForgotPasswordAsync(model, false);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Se ha enviado un correo con instrucciones para restablecer tu contraseña";
                    return RedirectToAction("Login");
                }
                else
                {
                    foreach (var error in result.Errors ?? new List<string>())
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Enlace de restablecimiento inválido";
                return RedirectToAction("Login");
            }

            var model = new ResetPasswordRequestDto
            {
                UserId = userId,
                Token = token
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequestDto model)
        {
            if (ModelState.IsValid)
            {
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Las contraseñas no coinciden");
                    return View(model);
                }

                var result = await _accountService.ResetPasswordAsync(model);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "¡Contraseña restablecida exitosamente! Ahora puedes iniciar sesión";
                    return RedirectToAction("Login");
                }
                else
                {
                    foreach (var error in result.Errors ?? new List<string>())
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Dtos.Property;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Domain.Enums;
using RealEstateApp.WebApp.Helpers;
using RealEstateApp.WebApp.Models.Property;
using System.Security.Claims;

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
        private readonly IMessageService _messageService;
        private readonly IOfferService _offerService;
        private readonly IUserService _userService;
        public PropertyController(
            IPropertyService propertyService,
            IPropertyTypeService propertyTypeService,
            ISaleTypeService saleTypeService,
            IImprovementService improvementService,
            IMapper mapper,
            IMessageService messageService,
            IOfferService offerService,
            IUserService userService)
        {
            _propertyService = propertyService;
            _propertyTypeService = propertyTypeService;
            _saleTypeService = saleTypeService;
            _improvementService = improvementService;
            _mapper = mapper;
            _messageService = messageService;
            _offerService = offerService;
            _userService = userService;
        }

        public async Task<IActionResult> Create()
        {
            var propertyTypes = await _propertyTypeService.GetAllAsync();
            var saleTypes = await _saleTypeService.GetAllAsync();
            var improvements = await _improvementService.GetAllAsync();

            if (!propertyTypes.Any() || !saleTypes.Any() || !improvements.Any())
            {
                TempData["Error"] = "No se puede crear una propiedad porque faltan tipos de propiedad, tipos de venta o mejoras creadas.";
                return RedirectToAction("Properties", "Agent");
            }

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

            if (!propertyTypes.Any() || !saleTypes.Any() || !improvements.Any())
            {
                TempData["Error"] = "No se puede crear una propiedad porque faltan tipos de propiedad, tipos de venta o mejoras creadas. ";
                return RedirectToAction("Properties", "Agent");
            }

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

            var imageFolderId = Guid.NewGuid().ToString();
            var imagePaths = new List<string>();

            foreach (var file in model.Images.Take(4))
            {
                var path = FileHandler.Upload(file, imageFolderId, "Properties");

                if (string.IsNullOrWhiteSpace(path))
                {
                    ModelState.AddModelError("Images", "Una de las imágenes no es válida o excede el tamaño permitido.");

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

                imagePaths.Add(path);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var dto = _mapper.Map<CreatePropertyDto>(model);
            dto.AgentId = userId;
            dto.ImagePaths = imagePaths;

            await _propertyService.CreatePropertyAsync(dto, userId);

            TempData["Success"] = "Propiedad creada exitosamente";
            return RedirectToAction("Properties", "Agent");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var property = await _propertyService.GetPropertyDetailAsync(id);
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

            if (!propertyTypes.Any() || !saleTypes.Any() || !improvements.Any())
            {
                TempData["Error"] = "No se puede editar la propiedad. Faltan configuraciones del sistema (tipos de propiedad, tipos de venta o mejoras).";
                return RedirectToAction("Properties", "Agent");
            }

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

            viewModel.ExistingImages = property.Images?.Select(img => img.Url).ToList() ?? new List<string>();

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

            // ✅ Parse el ID correctamente
            if (!int.TryParse(model.Id, out int propertyId))
            {
                return BadRequest();
            }

            var property = await _propertyService.GetPropertyDetailAsync(propertyId); // ✅ Usa Detail

            if (property == null || property.AgentId != userId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var dto = _mapper.Map<UpdatePropertyDto>(model);

            await _propertyService.UpdatePropertyAsync(propertyId, dto, userId);

            TempData["Success"] = "Propiedad actualizada exitosamente";
            return RedirectToAction("Properties", "Agent");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var property = await _propertyService.GetPropertyDetailAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (property.AgentId != userId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (property.Images != null && property.Images.Any())
            {
                foreach (var img in property.Images)
                {
                    if (!string.IsNullOrWhiteSpace(img.Url))
                    {
                        FileHandler.DeleteFile(img.Url);
                    }
                }
            }

            await _propertyService.DeletePropertyAsync(id, userId);

            TempData["Success"] = "Propiedad eliminada exitosamente";
            return RedirectToAction("Properties", "Agent");
        }

        public async Task<IActionResult> Details(int id)
        {
            var property = await _propertyService.GetPropertyDetailAsync(id);
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

            try
            {
                var chatPartners = await _messageService.GetAgentChatPartnersAsync(id, userId);
                var clientChats = new List<ClientChatSummary>();

                foreach (var message in chatPartners)
                {
                    var clientName = await _userService.GetUserFullNameAsync(message.SenderId);
                    var clientUser = await _userService.GetUserByIdAsync(message.SenderId);

                    clientChats.Add(new ClientChatSummary
                    {
                        ClientId = message.SenderId,
                        ClientName = clientName,
                        ClientAvatar = clientUser?.ProfilePicture,
                        LastMessage = message.Content,
                        LastMessageDate = message.SentDate,
                        UnreadCount = 0
                    });
                }

                viewModel.ClientChats = clientChats;
            }
            catch
            {
                viewModel.ClientChats = new List<ClientChatSummary>();
            }

            try
            {
                var offers = await _offerService.GetOffersByPropertyAsync(id);
                var clientOffers = new List<ClientOfferSummary>();
                var groupedOffers = offers.GroupBy(o => o.ClientId);

                foreach (var group in groupedOffers)
                {
                    var clientName = await _userService.GetUserFullNameAsync(group.Key);
                    var lastOffer = group.OrderByDescending(o => o.OfferDate).First();

                    clientOffers.Add(new ClientOfferSummary
                    {
                        ClientId = group.Key,
                        ClientName = clientName,
                        LastOfferAmount = lastOffer.Amount,
                        LastOfferDate = lastOffer.OfferDate,
                        Status = lastOffer.Status.ToString(),
                        StatusText = GetOfferStatusText(lastOffer.Status),
                        TotalOffers = group.Count()
                    });
                }

                viewModel.ClientOffers = clientOffers
                    .OrderByDescending(o => o.LastOfferDate)
                    .ToList();
            }
            catch
            {
                viewModel.ClientOffers = new List<ClientOfferSummary>();
            }

            return View(viewModel);
        }

        private string GetOfferStatusText(OfferStatus status)
        {
            return status switch
            {
                OfferStatus.Pendiente => "Pendiente",
                OfferStatus.Aceptada => "Aceptada",
                OfferStatus.Rechazada => "Rechazada",
                _ => "Desconocido"
            };
        }
    }
}
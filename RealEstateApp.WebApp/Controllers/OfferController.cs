using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Dtos.Offers;
using RealEstateApp.Application.Interfaces.Services;
using System.Security.Claims;

namespace RealEstateApp.WebApp.Controllers
{
    [Authorize]
    public class OfferController : Controller
    {
        private readonly IOfferService _offerService;
        private readonly IPropertyService _propertyService;
        private readonly IMapper _mapper;

        public OfferController(
            IOfferService offerService,
            IPropertyService propertyService,
            IMapper mapper)
        {
            _offerService = offerService;
            _propertyService = propertyService;
            _mapper = mapper;
        }

        // GET: /Offer/PropertyOffers
        public async Task<IActionResult> PropertyOffers(int propertyId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Verifica que la propiedad exista
            var property = await _propertyService.GetPropertyByIdAsync(propertyId.ToString());
            if (property == null)
                return NotFound();

            // Obtiene las ofertas del cliente para esta propiedad
            var offers = await _offerService.GetClientOffersForPropertyAsync(userId!, propertyId);
            var offerDtos = _mapper.Map<List<OfferDto>>(offers);

            // Verifica si puede crear una nueva oferta
            var canCreateOffer = await _offerService.CanCreateOfferAsync(userId!, propertyId);

            ViewBag.Property = property;
            ViewBag.CanCreateOffer = canCreateOffer;

            return View(offerDtos);
        }

        // POST: /Offer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOfferDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Por favor ingrese un monto válido";
                return RedirectToAction("PropertyOffers", new { propertyId = dto.PropertyId });
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                dto.ClientId = userId!;

                await _offerService.CreateOfferAsync(dto.ClientId, dto.PropertyId, dto.Amount);

                TempData["Success"] = "Oferta enviada correctamente";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("PropertyOffers", new { propertyId = dto.PropertyId });
        }

        // GET: /Offer/CanCreateOffer
        [HttpGet]
        public async Task<IActionResult> CanCreateOffer(int propertyId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var canCreate = await _offerService.CanCreateOfferAsync(userId!, propertyId);
            
            return Json(new { canCreate });
        }
    }
}

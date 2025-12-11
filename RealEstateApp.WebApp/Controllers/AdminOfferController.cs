using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Dtos.Offers;
using RealEstateApp.Application.Interfaces.Services;
using System.Security.Claims;

namespace RealEstateApp.WebApp.Controllers
{
    [Authorize(Roles = "Agente")]
    public class AdminOfferController : Controller
    {
        private readonly IOfferService _offerService;
        private readonly IPropertyService _propertyService;
        private readonly IMapper _mapper;

        public AdminOfferController(
            IOfferService offerService,
            IPropertyService propertyService,
            IMapper mapper)
        {
            _offerService = offerService;
            _propertyService = propertyService;
            _mapper = mapper;
        }

        // GET: /AdminOffer/PropertyOffers
        public async Task<IActionResult> PropertyOffers(int propertyId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Verifica que el agente es dueño de la propiedad
            if (!await _offerService.ValidateAgentOwnsPropertyAsync(propertyId, userId!))
                return Unauthorized();

            // Consigue la propiedad
            var property = await _propertyService.GetPropertyByIdAsync(propertyId.ToString());
            if (property == null)
                return NotFound();

            // Obtiene los clientes que han hecho ofertas
            var clientIds = await _offerService.GetClientsWithOffersAsync(propertyId);
            ViewBag.Property = property;
            ViewBag.ClientIds = clientIds;

            return View();
        }

        // GET: /AdminOffer/ClientOffers
        public async Task<IActionResult> ClientOffers(int propertyId, string clientId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Verifica que el agente es dueño de la propiedad
            if (!await _offerService.ValidateAgentOwnsPropertyAsync(propertyId, userId!))
                return Unauthorized();

            // Obtiene la propiedad
            var property = await _propertyService.GetPropertyByIdAsync(propertyId.ToString());
            if (property == null)
                return NotFound();

            // Obtiene las ofertas del cliente
            var offers = await _offerService.GetOffersByClientForPropertyAsync(clientId, propertyId);
            var offerDtos = _mapper.Map<List<OfferDto>>(offers);

            ViewBag.Property = property;
            ViewBag.ClientId = clientId;

            return View(offerDtos);
        }

        // POST: /AdminOffer/Accept
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(Guid offerId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _offerService.AcceptOfferAsync(offerId, userId!);

                TempData["Success"] = "Oferta aceptada correctamente. La propiedad ha sido marcada como vendida";

                var offer = await _offerService.GetOfferByIdAsync(offerId);
                return RedirectToAction("PropertyOffers", new { propertyId = offer?.PropertyId ?? 0 });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        // POST: /AdminOffer/Reject
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(Guid offerId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _offerService.RejectOfferAsync(offerId, userId!);

                TempData["Success"] = "Oferta rechazada correctamente";

                var offer = await _offerService.GetOfferByIdAsync(offerId);
                return RedirectToAction("PropertyOffers", new { propertyId = offer?.PropertyId ?? 0 });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Dtos.Offers;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.WebApp.Models.Offer;
using System.Security.Claims;

namespace RealEstateApp.WebApp.Controllers
{
    [Authorize]
    public class OfferController : Controller
    {
        private readonly IOfferService _offerService;
        private readonly IPropertyService _propertyService;
        private readonly IMapper _mapper;
        private readonly IAgentService _agentService;

        public OfferController(
            IOfferService offerService,
            IPropertyService propertyService,
            IMapper mapper,
            IAgentService agentService)
        {
            _offerService = offerService;
            _propertyService = propertyService;
            _mapper = mapper;
            _agentService = agentService;
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

        // GET: /Offer/AgentPropertyOffers - Lista todos los clientes con ofertas
        [Authorize(Roles = "Agente")]
        public async Task<IActionResult> AgentPropertyOffers(int propertyId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var property = await _propertyService.GetPropertyByIdAsync(propertyId.ToString());
                if (property == null)
                {
                    TempData["Error"] = $"La propiedad con ID {propertyId} no existe.";
                    return RedirectToAction("Dashboard", "Agent");
                }

                var isOwner = await _offerService.ValidateAgentOwnsPropertyAsync(propertyId, userId!);
                if (!isOwner)
                {
                    TempData["Error"] = "No tienes permiso para ver las ofertas de esta propiedad. ";
                    return RedirectToAction("Dashboard", "Agent");
                }

                var offers = await _offerService.GetOffersByPropertyAsync(propertyId);

                var clientOffers = new List<ClientOfferSummaryViewModel>();

                if (offers != null && offers.Any())
                {
                    var groupedOffers = offers.GroupBy(o => o.ClientId);

                    foreach (var group in groupedOffers)
                    {
                        var clientId = group.Key;
                        var client = await _agentService.GetClientByIdAsync(clientId);
                        var clientName = client != null
                            ? $"{client.FirstName} {client.LastName}"
                            : $"Cliente {clientId.Substring(0, Math.Min(8, clientId.Length))}";

                        var latestOffer = group.OrderByDescending(o => o.OfferDate).First();

                        clientOffers.Add(new ClientOfferSummaryViewModel
                        {
                            ClientId = clientId,
                            ClientName = clientName,
                            LastOfferAmount = latestOffer.Amount,
                            LastOfferDate = latestOffer.OfferDate,
                            Status = latestOffer.Status.ToString(),
                            TotalOffers = group.Count()
                        });
                    }

                    clientOffers = clientOffers.OrderByDescending(c => c.LastOfferDate).ToList();
                }

                ViewBag.Property = property;
                ViewBag.ClientOffers = clientOffers;

                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR en AgentPropertyOffers: {ex.Message}");
                TempData["Error"] = $"Error inesperado:  {ex.Message}";
                return RedirectToAction("Dashboard", "Agent");
            }
        }

        // GET: /Offer/ClientOffers - Lista las ofertas de un cliente específico
        [Authorize(Roles = "Agente")]
        public async Task<IActionResult> ClientOffers(int propertyId, string clientId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!await _offerService.ValidateAgentOwnsPropertyAsync(propertyId, userId!))
                {
                    TempData["Error"] = "No tienes permiso para ver las ofertas de esta propiedad.";
                    return RedirectToAction("Dashboard", "Agent");
                }

                var property = await _propertyService.GetPropertyByIdAsync(propertyId.ToString());
                if (property == null)
                {
                    TempData["Error"] = $"La propiedad con ID {propertyId} no existe.";
                    return RedirectToAction("Dashboard", "Agent");
                }

                var offers = await _offerService.GetOffersByClientForPropertyAsync(clientId, propertyId);
                var offerDtos = offers != null && offers.Any()
                    ? _mapper.Map<List<OfferDto>>(offers)
                    : new List<OfferDto>();

                // ✅ Obtener el nombre completo del cliente
                var client = await _agentService.GetClientByIdAsync(clientId);
                var clientName = client != null
                    ? $"{client.FirstName} {client.LastName}"
                    : $"Cliente {clientId.Substring(0, Math.Min(8, clientId.Length))}";

                ViewBag.Property = property;
                ViewBag.ClientId = clientId;
                ViewBag.ClientName = clientName;

                return View(offerDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR en ClientOffers: {ex.Message}");
                TempData["Error"] = $"Error inesperado:  {ex.Message}";
                return RedirectToAction("Dashboard", "Agent");
            }
        }
        // POST: /Offer/Accept
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Agente")]
        public async Task<IActionResult> Accept(Guid offerId, int propertyId, string clientId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _offerService.AcceptOfferAsync(offerId, userId!);

                TempData["Success"] = "Oferta aceptada.  La propiedad ha sido marcada como vendida y todas las demás ofertas pendientes han sido rechazadas automáticamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al aceptar la oferta: {ex.Message}";
            }

            return RedirectToAction("ClientOffers", new { propertyId, clientId });
        }

        // POST: /Offer/Reject
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Agente")]
        public async Task<IActionResult> Reject(Guid offerId, int propertyId, string clientId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _offerService.RejectOfferAsync(offerId, userId!);

                TempData["Success"] = "Oferta rechazada exitosamente. ";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al rechazar la oferta: {ex.Message}";
            }

            return RedirectToAction("ClientOffers", new { propertyId, clientId });
        }
    }
}

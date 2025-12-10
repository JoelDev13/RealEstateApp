using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Dtos.Messages;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Domain.Enums;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace RealEstateApp.WebApp.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IMessageService _messageService;
        private readonly IPropertyService _propertyService;
        private readonly IMapper _mapper;
        private readonly IAgentService _agentService;

        public ChatController(
            IMessageService messageService,
            IPropertyService propertyService,
            IMapper mapper,
            IAgentService agentService)
        {
            _messageService = messageService;
            _propertyService = propertyService;
            _mapper = mapper;
            _agentService = agentService;
        }

        // GET: /Chat/PropertyChat
        public async Task<IActionResult> PropertyChat(int propertyId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Verifica que la propiedad exista
            var property = await _propertyService.GetPropertyByIdAsync(propertyId.ToString());
            if (property == null)
                return NotFound();

            // Verifica que la propiedad este disponible
            if (property.Status == PropertyStatus.Vendida)
            {
                TempData["Error"] = "No se puede chatear sobre propiedades vendidas";
                return RedirectToAction("Details", "Property", new { id = propertyId });
            }

            // Obtiene el chat del cliente con el agente
            var agentId = property.AgentId;
            var messages = await _messageService.GetChatBetweenUsersAsync(userId!, agentId, propertyId);
            var messageDtos = _mapper.Map<List<MessageDto>>(messages);

            // Marca quién envió cada mensaje (cliente vs agente)
            foreach (var msg in messageDtos)
            {
                msg.IsFromAgent = msg.SenderId == agentId;
            }

            // Obtiene informacion del agente
            var agent = await _agentService.GetAgentByIdAsync(agentId);

            ViewBag.Property = property;
            ViewBag.AgentId = agentId;
            ViewBag.AgentName = agent?.FirstName + " " + agent?.LastName ?? "Agente";

            return View(messageDtos);
        }

        // POST: /Chat/Send
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(CreateMessageDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Por favor ingrese un mensaje válido";
                return RedirectToAction("PropertyChat", new { propertyId = dto.PropertyId });
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var property = await _propertyService.GetPropertyByIdAsync(dto.PropertyId.ToString());
                
                if (property == null)
                    return NotFound();

                // Determina el receptor (agente o cliente)
                string receiverId;
                if (User.IsInRole("Cliente"))
                {
                    receiverId = property.AgentId;
                    dto.SenderId = userId!;
                }
                else // Agente
                {
                    receiverId = dto.ReceiverId;
                    dto.SenderId = userId!;
                }

                await _messageService.SendMessageAsync(dto.SenderId, receiverId, dto.PropertyId, dto.Content);

                TempData["Success"] = "Mensaje enviado correctamente";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            if (User.IsInRole("Cliente"))
            {
                return RedirectToAction("PropertyChat", new { propertyId = dto.PropertyId });
            }
            else
            {
                return RedirectToAction("AgentChat", new { propertyId = dto.PropertyId, clientId = dto.ReceiverId });
            }
        }

        // GET: /Chat/AgentChat
        [Authorize(Roles = "Agente")]
        public async Task<IActionResult> AgentChat(int propertyId, string clientId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Verifica que el agente es dueño de la propiedad
            if (!await _messageService.ValidateAgentOwnsPropertyAsync(propertyId, userId!))
                return Unauthorized();

            // Verifica que la propiedad exista
            var property = await _propertyService.GetPropertyByIdAsync(propertyId.ToString());
            if (property == null)
                return NotFound();

            // Obtiene el chat con el cliente especifico
            var messages = await _messageService.GetChatBetweenUsersAsync(userId!, clientId, propertyId);
            var messageDtos = _mapper.Map<List<MessageDto>>(messages);

            // Marca quién envió cada mensaje (agente vs cliente)
            foreach (var msg in messageDtos)
            {
                msg.IsFromAgent = msg.SenderId == userId;
            }

            ViewBag.Property = property;
            ViewBag.ClientId = clientId;

            return View(messageDtos);
        }

        // GET: /Chat/PropertyChats
        [Authorize(Roles = "Agente")]
        public async Task<IActionResult> PropertyChats(int propertyId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Verifica que el agente es dueño de la propiedad
            if (!await _messageService.ValidateAgentOwnsPropertyAsync(propertyId, userId!))
                return Unauthorized();

            // Verifica que la propiedad exista
            var property = await _propertyService.GetPropertyByIdAsync(propertyId.ToString());
            if (property == null)
                return NotFound();

            //los clientes con los que ha chateado
            var chatPartners = await _messageService.GetAgentChatPartnersAsync(propertyId, userId!);

            ViewBag.Property = property;

            return View(chatPartners);
        }
    }
}

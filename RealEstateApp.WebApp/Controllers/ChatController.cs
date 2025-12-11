using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Dtos.Messages;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.WebApp.Models.Chat;
using System.Security.Claims;

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

            var property = await _propertyService.GetPropertyByIdAsync(propertyId.ToString());
            if (property == null)
                return NotFound();

            if (property.Status == "Vendida")
            {
                TempData["Error"] = "No se puede chatear sobre propiedades vendidas";
                return RedirectToAction("Details", "PublicProperty", new { id = propertyId });
            }

            var agentId = property.AgentId;
            var messages = await _messageService.GetChatBetweenUsersAsync(userId!, agentId, propertyId);
            var messageDtos = _mapper.Map<List<MessageDto>>(messages);

            foreach (var msg in messageDtos)
            {
                msg.IsFromAgent = msg.SenderId == agentId;
            }

            var agent = await _agentService.GetAgentByIdAsync(agentId);

            ViewBag.Property = property;
            ViewBag.AgentId = agentId;
            ViewBag.AgentName = agent != null ? $"{agent.FirstName} {agent.LastName}" : "Agente";
            ViewBag.AgentProfilePicture = agent?.ProfilePicture;

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

                string receiverId;
                if (User.IsInRole("Cliente"))
                {
                    receiverId = property.AgentId;
                    dto.SenderId = userId!;
                }
                else
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

            if (!await _messageService.ValidateAgentOwnsPropertyAsync(propertyId, userId!))
                return Unauthorized();

            var property = await _propertyService.GetPropertyByIdAsync(propertyId.ToString());
            if (property == null)
                return NotFound();

            var messages = await _messageService.GetChatBetweenUsersAsync(userId!, clientId, propertyId);
            var messageDtos = _mapper.Map<List<MessageDto>>(messages);

            foreach (var msg in messageDtos)
            {
                msg.IsFromAgent = msg.SenderId == userId;
            }

            var client = await _agentService.GetClientByIdAsync(clientId);
            var clientName = client != null
                ? $"{client.FirstName} {client.LastName}"
                : $"Cliente {clientId.Substring(0, Math.Min(8, clientId.Length))}";

            ViewBag.Property = property;
            ViewBag.ClientId = clientId;
            ViewBag.ClientName = clientName;

            return View(messageDtos);
        }

        // GET: /Chat/PropertyChats
        [Authorize(Roles = "Agente")]
        public async Task<IActionResult> PropertyChats(int propertyId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await _messageService.ValidateAgentOwnsPropertyAsync(propertyId, userId!))
                return Unauthorized();

            var property = await _propertyService.GetPropertyByIdAsync(propertyId.ToString());
            if (property == null)
                return NotFound();

            var chatPartners = await _messageService.GetAgentChatPartnersAsync(propertyId, userId!);

            var chatSummaries = new List<ChatSummaryViewModel>();
            foreach (var m in chatPartners)
            {
                var client = await _agentService.GetClientByIdAsync(m.SenderId);
                var clientName = client != null
                    ? $"{client.FirstName} {client.LastName}"
                    : $"Cliente {m.SenderId.Substring(0, Math.Min(8, m.SenderId.Length))}";

                chatSummaries.Add(new ChatSummaryViewModel
                {
                    ClientId = m.SenderId,
                    ClientName = clientName,
                    LastMessage = m.Content,
                    LastMessageDate = m.SentDate,
                    UnreadCount = 0
                });
            }

            ViewBag.Property = property;
            ViewBag.ChatSummaries = chatSummaries;

            return View();
        }
    }
}
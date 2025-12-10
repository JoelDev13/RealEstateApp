using RealEstateApp.Application.Dtos.Messages;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;

namespace RealEstateApp.Application.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IPropertyRepository _propertyRepository;

        public MessageService(
            IMessageRepository messageRepository,
            IPropertyRepository propertyRepository)
        {
            _messageRepository = messageRepository;
            _propertyRepository = propertyRepository;
        }

        public async Task<Message> SendMessageAsync(string senderId, string receiverId, int propertyId, string content)
        {
            // Valida que la propiedad exista
            var property = await _propertyRepository.GetByIdAsync(propertyId);
            if (property == null)
                throw new KeyNotFoundException("Propiedad no encontrada");

            // Valida que se puede enviar mensaje
            if (!await CanSendMessageAsync(senderId, receiverId, propertyId))
                throw new InvalidOperationException("No se puede enviar mensaje a esta propiedad");

            var message = new Message
            {
                PropertyId = propertyId,
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                SentDate = DateTime.UtcNow
            };

            await _messageRepository.AddAsync(message);
            return message;
        }

        public async Task<List<Message>> GetChatBetweenUsersAsync(string userId1, string userId2, int propertyId)
        {
            return await _messageRepository.GetChatBetweenUsersAsync(userId1, userId2, propertyId);
        }

        public async Task<List<Message>> GetPropertyMessagesForUserAsync(string userId, int propertyId)
        {
            return await _messageRepository.GetPropertyMessagesForUserAsync(userId, propertyId);
        }

        public async Task<List<string>> GetUniqueChatPartnersAsync(string userId, int propertyId)
        {
            return await _messageRepository.GetUniqueChatPartnersAsync(userId, propertyId);
        }

        public async Task<List<MessageDto>> GetAgentChatPartnersAsync(int propertyId, string agentId)
        {
            // Valida que el agente es dueño de la propiedad
            if (!await ValidateAgentOwnsPropertyAsync(propertyId, agentId))
                throw new UnauthorizedAccessException("No es el propietario de esta propiedad");

            // Obtiene todos los mensajes de la propiedad
            var messages = await _messageRepository.GetPropertyMessagesAsync(propertyId);
            
            // Agrupa por cliente y obtener el ultimo mensaje de cada conversacion
            var clientGroups = messages
                .Where(m => m.SenderId != agentId) // Mensajes de clientes
                .GroupBy(m => m.SenderId)
                .Select(g => g.OrderByDescending(m => m.SentDate).First())
                .ToList();

            
            var dtos = new List<MessageDto>();
            foreach (var message in clientGroups)
            {
                dtos.Add(new MessageDto
                {
                    Id = message.Id,
                    PropertyId = message.PropertyId,
                    PropertyCode = message.Property?.Code ?? "",
                    SenderId = message.SenderId,
                    SenderName = "Cliente", 
                    ReceiverId = message.ReceiverId,
                    ReceiverName = "Agente",
                    Content = message.Content,
                    SentDate = message.SentDate,
                    IsFromAgent = false
                });
            }

            return dtos.OrderByDescending(m => m.SentDate).ToList();
        }

        public async Task<bool> ValidateAgentOwnsPropertyAsync(int propertyId, string agentId)
        {
            var property = await _propertyRepository.GetByIdAsync(propertyId);
            return property?.AgentId == agentId;
        }

        public async Task<bool> CanSendMessageAsync(string senderId, string receiverId, int propertyId)
        {
            // Valida que la propiedad este disponible (no vendida)
            var property = await _propertyRepository.GetByIdAsync(propertyId);
            if (property == null || property.Status == PropertyStatus.Vendida)
                return false;

            return senderId != receiverId;
        }
    }
}

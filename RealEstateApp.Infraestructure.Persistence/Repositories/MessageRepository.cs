using Microsoft.EntityFrameworkCore;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Infrastructure.Persistence;

namespace RealEstateApp.Infrastructure.Persistence.Repositories
{
    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        private new readonly ApplicationDbContext _context;

        public MessageRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Message>> GetChatBetweenUsersAsync(string userId1, string userId2, int propertyId)
        {
            return await _context.Messages
                .Where(m => m.PropertyId == propertyId && 
                           ((m.SenderId == userId1 && m.ReceiverId == userId2) || 
                            (m.SenderId == userId2 && m.ReceiverId == userId1)))
                .OrderBy(m => m.SentDate)
                .Include(m => m.Property)
                .ToListAsync();
        }

        public async Task<List<Message>> GetPropertyMessagesForUserAsync(string userId, int propertyId)
        {
            return await _context.Messages
                .Where(m => m.PropertyId == propertyId && 
                           (m.SenderId == userId || m.ReceiverId == userId))
                .OrderByDescending(m => m.SentDate)
                .Include(m => m.Property)
                .ToListAsync();
        }

        public async Task<List<Message>> GetPropertyMessagesAsync(int propertyId)
        {
            return await _context.Messages
                .Where(m => m.PropertyId == propertyId)
                .OrderByDescending(m => m.SentDate)
                .Include(m => m.Property)
                .ToListAsync();
        }

        public async Task<List<string>> GetUniqueChatPartnersAsync(string userId, int propertyId)
        {
            var partners = await _context.Messages
                .Where(m => m.PropertyId == propertyId && 
                           (m.SenderId == userId || m.ReceiverId == userId))
                .Select(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Distinct()
                .ToListAsync();

            return partners;
        }

        public async Task<List<string>> GetClientsWithMessagesAsync(int propertyId)
        {
            // Obtiene todos los usuarios que han enviado mensajes a esta propiedad
            var clients = await _context.Messages
                .Where(m => m.PropertyId == propertyId)
                .Select(m => m.SenderId)
                .Distinct()
                .ToListAsync();

            return clients;
        }
    }
}

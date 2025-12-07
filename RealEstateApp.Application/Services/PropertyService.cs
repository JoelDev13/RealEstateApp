using RealEstateApp.Application.Dtos.Property;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IPropertyTypeRepository _propertyTypeRepository;
        private readonly ISaleTypeRepository _saleTypeRepository;
        private readonly IImprovementRepository _improvementRepository;

        public PropertyService(
            IPropertyRepository propertyRepository,
            IPropertyTypeRepository propertyTypeRepository,
            ISaleTypeRepository saleTypeRepository,
            IImprovementRepository improvementRepository)
        {
            _propertyRepository = propertyRepository;
            _propertyTypeRepository = propertyTypeRepository;
            _saleTypeRepository = saleTypeRepository;
            _improvementRepository = improvementRepository;
        }

        public async Task<List<Property>> GetPropertiesByAgentAsync(string agentId)
        {
            return await _propertyRepository.GetByAgentAsync(agentId);
        }

        public async Task<List<Property>> GetAvailablePropertiesByAgentAsync(string agentId)
        {
            return await _propertyRepository.GetAvailableByAgentAsync(agentId);
        }

        public async Task<Property?> GetPropertyByIdAsync(string id)
        {
            if (int.TryParse(id, out int propertyId))
            {
                return await _propertyRepository.GetByIdAsync(propertyId);
            }
            return null;
        }

        public async Task<bool> DeletePropertyAsync(string id)
        {
            if (int.TryParse(id, out int propertyId))
            {
                return await _propertyRepository.DeleteAsync(propertyId);
            }
            return false;
        }

        public async Task<Property> CreatePropertyAsync(CreatePropertyDto dto)
        {
            // Validar que existan los tipos
            if (!int.TryParse(dto.PropertyTypeId, out int propertyTypeId))
            {
                throw new InvalidOperationException("ID de tipo de propiedad inválido");
            }
            if (!int.TryParse(dto.SaleTypeId, out int saleTypeId))
            {
                throw new InvalidOperationException("ID de tipo de venta inválido");
            }

            var propertyType = await _propertyTypeRepository.GetByIdAsync(propertyTypeId);
            var saleType = await _saleTypeRepository.GetByIdAsync(saleTypeId);

            if (propertyType == null || saleType == null)
            {
                throw new InvalidOperationException("No hay tipo de propiedades o tipo de ventas creadas");
            }

            // Valida que existan las mejoras
            var improvements = await _improvementRepository.GetByIdsAsync(dto.ImprovementIds);
            if (!improvements.Any())
            {
                throw new InvalidOperationException("No hay mejoras creadas");
            }

            var property = new Property
            {
                Code = await GenerateUniquePropertyCodeAsync(),
                PropertyTypeId = propertyTypeId,
                SaleTypeId = saleTypeId,
                Price = dto.Price,
                Description = dto.Description,
                SizeInSquareMeters = (double)dto.Size,
                Bedrooms = dto.Bedrooms,
                Bathrooms = dto.Bathrooms,
                AgentId = dto.AgentId,
                IsSold = false,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Improvements = improvements.ToHashSet()
            };

            return await _propertyRepository.AddAsync(property);
        }

        public async Task<Property> UpdatePropertyAsync(UpdatePropertyDto dto)
        {
            if (!int.TryParse(dto.Id, out int propertyId))
            {
                throw new KeyNotFoundException("ID de propiedad inválido");
            }

            var property = await _propertyRepository.GetByIdAsync(propertyId);
            if (property == null)
            {
                throw new KeyNotFoundException("Propiedad no encontrada");
            }

            // Valida que existan los tipos
            if (!int.TryParse(dto.PropertyTypeId, out int propertyTypeId))
            {
                throw new InvalidOperationException("ID de tipo de propiedad inválido");
            }
            if (!int.TryParse(dto.SaleTypeId, out int saleTypeId))
            {
                throw new InvalidOperationException("ID de tipo de venta inválido");
            }

            var propertyType = await _propertyTypeRepository.GetByIdAsync(propertyTypeId);
            var saleType = await _saleTypeRepository.GetByIdAsync(saleTypeId);

            if (propertyType == null || saleType == null)
            {
                throw new InvalidOperationException("No hay tipo de propiedades o tipo de ventas creadas");
            }

            // Valida que existan las mejoras
            var improvements = await _improvementRepository.GetByIdsAsync(dto.ImprovementIds);
            if (!improvements.Any())
            {
                throw new InvalidOperationException("No hay mejoras creadas");
            }

            property.PropertyTypeId = propertyTypeId;
            property.SaleTypeId = saleTypeId;
            property.Price = dto.Price;
            property.Description = dto.Description;
            property.SizeInSquareMeters = (double)dto.Size;
            property.Bedrooms = dto.Bedrooms;
            property.Bathrooms = dto.Bathrooms;
            property.UpdatedAt = DateTime.UtcNow;

            // Actualiza mejoras
            property.Improvements = improvements.ToHashSet();

            return await _propertyRepository.UpdateAsync(property);
        }

        public async Task<string> GenerateUniquePropertyCodeAsync()
        {
            string code;
            bool exists;
            do
            {
                code = GenerateRandomCode(6);
                exists = await _propertyRepository.CodeExistsAsync(code);
            } while (exists);

            return code;
        }

        private string GenerateRandomCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}

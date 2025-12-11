using AutoMapper;
using RealEstateApp.Application.Dtos.Property;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;

namespace RealEstateApp.Application.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IPropertyTypeRepository _propertyTypeRepository;
        private readonly ISaleTypeRepository _saleTypeRepository;
        private readonly IImprovementRepository _improvementRepository;
        private readonly IOfferRepository _offerRepository;
        private readonly IMapper _mapper;
        public PropertyService(
            IPropertyRepository propertyRepository,
            IPropertyTypeRepository propertyTypeRepository,
            ISaleTypeRepository saleTypeRepository,
            IImprovementRepository improvementRepository,
            IOfferRepository offerRepository,
            IMapper mapper)
        {
            _propertyRepository = propertyRepository;
            _propertyTypeRepository = propertyTypeRepository;
            _saleTypeRepository = saleTypeRepository;
            _improvementRepository = improvementRepository;
            _offerRepository = offerRepository;
            _mapper = mapper;
        }

        public async Task<List<Property>> GetPropertiesByAgentAsync(string agentId)
        {
            return await _propertyRepository.GetByAgentAsync(agentId);
        }

        public async Task<List<Property>> GetAvailablePropertiesByAgentAsync(string agentId)
        {
            return await _propertyRepository.GetAvailableByAgentAsync(agentId);
        }

        public async Task<List<Property>> GetSoldPropertiesByAgentAsync(string agentId)
        {
            var allProperties = await _propertyRepository.GetByAgentAsync(agentId);
            return allProperties.Where(p => p.Status == PropertyStatus.Vendida).ToList();
        }

        public async Task<PropertyDto?> GetPropertyByIdAsync(string id)
        {
            if (int.TryParse(id, out int propertyId))
            {
                var property = await _propertyRepository.GetByIdAsync(propertyId);

                if (property == null)
                    return null;

                return _mapper.Map<PropertyDto>(property);
            }
            return null;
        }

        public async Task<Property> CreatePropertyAsync(CreatePropertyDto dto, string agentId)
        {
            if (!int.TryParse(dto.PropertyTypeId, out int propertyTypeId))
            {
                throw new InvalidOperationException("ID de tipo de propiedad inválido");
            }
            if (!int.TryParse(dto.SaleTypeId, out int saleTypeId))
            {
                throw new InvalidOperationException("ID de tipo de venta inválido");
            }

            var improvements = await _improvementRepository.GetByIdsAsync(dto.ImprovementIds);

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
                AgentId = agentId,
                Status = PropertyStatus.Disponible,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Improvements = improvements.ToHashSet()
            };

            if (dto.ImagePaths != null && dto.ImagePaths.Any())
            {
                var images = new List<PropertyImage>();
                for (int i = 0; i < dto.ImagePaths.Count; i++)
                {
                    images.Add(new PropertyImage
                    {
                        Url = dto.ImagePaths[i],
                        IsPrimary = i == 0,
                        Property = property
                    });
                }
                property.Images = images;
            }

            return await _propertyRepository.AddAsync(property);
        }
        public async Task<Property> UpdatePropertyAsync(int propertyId, UpdatePropertyDto dto, string agentId)
        {
            // Validate ownership
            if (!await IsPropertyOwnedByAgentAsync(propertyId, agentId))
                throw new UnauthorizedAccessException("You don't own this property");

            var property = await _propertyRepository.GetByIdAsync(propertyId);
            if (property == null)
            {
                throw new KeyNotFoundException("Propiedad no encontrada");
            }

            // Convertir IDs a números
            if (!int.TryParse(dto.PropertyTypeId, out int propertyTypeId))
            {
                throw new InvalidOperationException("ID de tipo de propiedad inválido");
            }
            if (!int.TryParse(dto.SaleTypeId, out int saleTypeId))
            {
                throw new InvalidOperationException("ID de tipo de venta inválido");
            }

            // Obtener mejoras
            var improvements = await _improvementRepository.GetByIdsAsync(dto.ImprovementIds);

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

        public async Task<bool> DeletePropertyAsync(int propertyId, string agentId)
        {
            // Validate ownership
            if (!await IsPropertyOwnedByAgentAsync(propertyId, agentId))
                throw new UnauthorizedAccessException("You don't own this property");

            var property = await _propertyRepository.GetByIdAsync(propertyId);
            if (property == null)
                return false;

            // Delete related offers
            var offers = await _offerRepository.GetByPropertyAsync(propertyId);
            foreach (var offer in offers)
            {
                await _offerRepository.DeleteAsync(offer);
            }

            // Delete property (cascade will handle images and improvements)
            return await _propertyRepository.DeleteAsync(propertyId);
        }

        public async Task<Property> GetPropertyDetailAsync(int propertyId)
        {
            return await _propertyRepository.GetByIdAsync(propertyId)
                ?? throw new KeyNotFoundException("Property not found");
        }

        public async Task<bool> IsPropertyOwnedByAgentAsync(int propertyId, string agentId)
        {
            var property = await _propertyRepository.GetByIdAsync(propertyId);
            return property?.AgentId == agentId;
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

        public async Task<List<Property>> GetAvailablePropertiesAsync()
        {
            return await _propertyRepository.GetAvailablePropertiesAsync();
        }

        public async Task<List<Property>> GetPropertiesByIdsAsync(List<int> propertyIds)
        {
            return await _propertyRepository.GetByIdsAsync(propertyIds);
        }

        public async Task<IEnumerable<Property>> GetAgentPropertiesAsync(string agentId)
        {
            return await _propertyRepository.GetAvailableByAgentAsync(agentId);
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

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Application.Dtos.PropertyTypes;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Services
{
    public class PropertyTypeService : IPropertyTypeService
    {
        private readonly IPropertyTypeRepository _propertyTypeRepository;
        private readonly IMapper _mapper;

        public PropertyTypeService(IPropertyTypeRepository propertyTypeRepository, IMapper mapper)
        {
            _propertyTypeRepository = propertyTypeRepository;
            _mapper = mapper;
        }

        public async Task<List<PropertyTypeDto>> GetAllAsync()
        {
            var entities = await _propertyTypeRepository
                .Query()
                .OrderBy(pt => pt.Name)
                .ToListAsync();

            return _mapper.Map<List<PropertyTypeDto>>(entities);
        }

        public async Task<PropertyTypeDto?> GetByIdAsync(int id)
        {
            var entity = await _propertyTypeRepository.GetByIdAsync(id);
            return entity is null ? null : _mapper.Map<PropertyTypeDto>(entity);
        }

        public async Task<int> CreateAsync(PropertyTypeDto dto)
        {
            var entity = _mapper.Map<PropertyType>(dto);
            entity.CreatedAt = DateTime.UtcNow;

            await _propertyTypeRepository.AddAsync(entity);
            return entity.Id;
        }

        public async Task UpdateAsync(PropertyTypeDto dto)
        {
            var entity = await _propertyTypeRepository.GetByIdAsync(dto.Id);

            if (entity == null)
                throw new KeyNotFoundException($"PropertyType con Id {dto.Id} no existe.");

            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.IsActive = dto.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _propertyTypeRepository.UpdateAsync(entity);
        }

        public async Task ToggleStatusAsync(int id)
        {
            var entity = await _propertyTypeRepository.GetByIdAsync(id);

            if (entity == null)
                throw new KeyNotFoundException($"PropertyType con Id {id} no existe.");

            entity.IsActive = !entity.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _propertyTypeRepository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _propertyTypeRepository.GetByIdAsync(id);

            if (entity == null)
                throw new KeyNotFoundException($"PropertyType con Id {id} no existe.");

            await _propertyTypeRepository.DeleteAsync(entity);
        }
    }
}

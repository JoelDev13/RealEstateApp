using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Application.Dtos.SaleTypes;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Services
{
    public class SaleTypeService : ISaleTypeService
    {
        private readonly ISaleTypeRepository _saleTypeRepository;
        private readonly IMapper _mapper;

        public SaleTypeService(ISaleTypeRepository saleTypeRepository, IMapper mapper)
        {
            _saleTypeRepository = saleTypeRepository;
            _mapper = mapper;
        }

        public async Task<List<SaleTypeDto>> GetAllAsync()
        {
            var entities = await _saleTypeRepository
                .Query()
                .OrderBy(st => st.Name)
                .ToListAsync();

            return _mapper.Map<List<SaleTypeDto>>(entities);
        }

        public async Task<SaleTypeDto?> GetByIdAsync(int id)
        {
            var entity = await _saleTypeRepository.GetByIdAsync(id);
            return entity is null ? null : _mapper.Map<SaleTypeDto>(entity);
        }

        public async Task<int> CreateAsync(SaleTypeDto dto)
        {
            var entity = _mapper.Map<SaleType>(dto);
            entity.CreatedAt = DateTime.UtcNow;

            await _saleTypeRepository.AddAsync(entity);
            return entity.Id;
        }

        public async Task UpdateAsync(SaleTypeDto dto)
        {
            var entity = await _saleTypeRepository.GetByIdAsync(dto.Id);

            if (entity == null)
                throw new KeyNotFoundException($"SaleType con Id {dto.Id} no existe.");

            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.IsActive = dto.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _saleTypeRepository.UpdateAsync(entity);
        }

        public async Task ToggleStatusAsync(int id)
        {
            var entity = await _saleTypeRepository.GetByIdAsync(id);

            if (entity == null)
                throw new KeyNotFoundException($"SaleType con Id {id} no existe.");

            entity.IsActive = !entity.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _saleTypeRepository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _saleTypeRepository.GetByIdAsync(id);

            if (entity == null)
                throw new KeyNotFoundException($"SaleType con Id {id} no existe.");

            await _saleTypeRepository.DeleteAsync(entity);
        }
    }
}

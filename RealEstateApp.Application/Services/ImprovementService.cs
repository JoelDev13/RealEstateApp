using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Application.Dtos.Improvements;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Services
{
    public class ImprovementService : IImprovementService
    {
        private readonly IImprovementRepository _improvementRepository;
        private readonly IMapper _mapper;

        public ImprovementService(IImprovementRepository improvementRepository, IMapper mapper)
        {
            _improvementRepository = improvementRepository;
            _mapper = mapper;
        }

        public async Task<List<ImprovementDto>> GetAllAsync()
        {
            var entities = await _improvementRepository
                .Query()
                .OrderBy(i => i.Name)
                .ToListAsync();

            return _mapper.Map<List<ImprovementDto>>(entities);
        }

        public async Task<ImprovementDto?> GetByIdAsync(int id)
        {
            var entity = await _improvementRepository.GetByIdAsync(id);
            return entity is null ? null : _mapper.Map<ImprovementDto>(entity);
        }

        public async Task<int> CreateAsync(ImprovementDto dto)
        {
            var entity = _mapper.Map<Improvement>(dto);
            entity.CreatedAt = DateTime.UtcNow;

            await _improvementRepository.AddAsync(entity);
            return entity.Id;
        }

        public async Task UpdateAsync(ImprovementDto dto)
        {
            var entity = await _improvementRepository.GetByIdAsync(dto.Id);

            if (entity == null)
                throw new KeyNotFoundException($"Improvement con Id {dto.Id} no existe.");

            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.IsActive = dto.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _improvementRepository.UpdateAsync(entity);
        }

        public async Task ToggleStatusAsync(int id)
        {
            var entity = await _improvementRepository.GetByIdAsync(id);

            if (entity == null)
                throw new KeyNotFoundException($"Improvement con Id {id} no existe.");

            entity.IsActive = !entity.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _improvementRepository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _improvementRepository.GetByIdAsync(id);

            if (entity == null)
                throw new KeyNotFoundException($"Improvement con Id {id} no existe.");

            await _improvementRepository.DeleteAsync(entity);
        }
    }
}

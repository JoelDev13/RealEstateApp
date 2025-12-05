using MediatR;
using RealEstateApp.Application.Dtos.Improvements;
using RealEstateApp.Application.Interfaces.Repositories;

public class UpdateImprovementCommandHandler
    : IRequestHandler<UpdateImprovementCommand, ImprovementDto>
{
    private readonly IImprovementRepository _improvementRepository;

    public UpdateImprovementCommandHandler(IImprovementRepository improvementRepository)
    {
        _improvementRepository = improvementRepository;
    }

    public async Task<ImprovementDto> Handle(UpdateImprovementCommand request, CancellationToken cancellationToken)
    {
        var entity = await _improvementRepository.GetByIdAsync(request.Id);

        if (entity == null)
            throw new KeyNotFoundException($"Improvement con Id {request.Id} no existe.");

        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.IsActive = request.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        await _improvementRepository.UpdateAsync(entity);

        return new ImprovementDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            IsActive = entity.IsActive
        };
    }
}

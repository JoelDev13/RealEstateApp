using MediatR;
using RealEstateApp.Application.Dtos.SaleTypes;
using RealEstateApp.Application.Interfaces.Repositories;

public class UpdateSaleTypeCommandHandler
    : IRequestHandler<UpdateSaleTypeCommand, SaleTypeDto>
{
    private readonly ISaleTypeRepository _saleTypeRepository;

    public UpdateSaleTypeCommandHandler(ISaleTypeRepository saleTypeRepository)
    {
        _saleTypeRepository = saleTypeRepository;
    }

    public async Task<SaleTypeDto> Handle(UpdateSaleTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _saleTypeRepository.GetByIdAsync(request.Id);

        if (entity == null)
            throw new KeyNotFoundException($"SaleType con Id {request.Id} no existe.");

        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.IsActive = request.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        await _saleTypeRepository.UpdateAsync(entity);

        return new SaleTypeDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            IsActive = entity.IsActive
        };
    }
}

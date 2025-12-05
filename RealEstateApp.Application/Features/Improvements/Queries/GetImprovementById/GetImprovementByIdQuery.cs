using MediatR;
using RealEstateApp.Application.Dtos.Improvements;

namespace RealEstateApp.Application.Features.Improvements.Queries.GetImprovementById
{
    public sealed record GetImprovementByIdQuery(int Id) : IRequest<ImprovementDto>;
}

using MediatR;
using RealEstateApp.Application.Dtos.Improvements;

namespace RealEstateApp.Application.Features.Improvements.Queries.GetImprovements
{
    public class GetImprovementsQuery : IRequest<List<ImprovementDto>>
    {
    }
}

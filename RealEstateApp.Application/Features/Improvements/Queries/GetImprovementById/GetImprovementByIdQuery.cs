using MediatR;
using RealEstateApp.Application.Dtos.Improvements;

namespace RealEstateApp.Application.Features.Improvements.Queries.GetImprovementById
{
    public class GetImprovementByIdQuery : IRequest<ImprovementDto>
    {
        public int Id { get; set; }
    }
}

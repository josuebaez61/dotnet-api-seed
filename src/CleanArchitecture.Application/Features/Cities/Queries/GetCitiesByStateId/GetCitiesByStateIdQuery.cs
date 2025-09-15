using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Cities.Queries.GetCitiesByStateId
{
    public class GetCitiesByStateIdQuery : IRequest<List<CityDto>>
    {
        public Guid StateId { get; set; }

        public GetCitiesByStateIdQuery(Guid stateId)
        {
            StateId = stateId;
        }
    }
}

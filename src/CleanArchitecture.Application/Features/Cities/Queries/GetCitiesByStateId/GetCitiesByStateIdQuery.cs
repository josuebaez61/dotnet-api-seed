using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Cities.Queries.GetCitiesByStateId
{
  public class GetCitiesByStateIdQuery : IRequest<List<CityDto>>
  {
    public int StateId { get; set; }

    public GetCitiesByStateIdQuery(int stateId)
    {
      StateId = stateId;
    }
  }
}

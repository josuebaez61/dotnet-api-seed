using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Cities.Queries.GetAllCities
{
    public class GetAllCitiesQuery : IRequest<List<CityDto>>
    {
    }
}

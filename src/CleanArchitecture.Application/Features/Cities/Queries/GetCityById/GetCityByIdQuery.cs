using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Cities.Queries.GetCityById
{
    public class GetCityByIdQuery : IRequest<CityDto?>
    {
        public Guid Id { get; set; }

        public GetCityByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}

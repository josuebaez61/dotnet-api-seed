using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Cities.Queries.GetCityById
{
    public class GetCityByIdQuery : IRequest<CityDto?>
    {
        public int Id { get; set; }

        public GetCityByIdQuery(int id)
        {
            Id = id;
        }
    }
}

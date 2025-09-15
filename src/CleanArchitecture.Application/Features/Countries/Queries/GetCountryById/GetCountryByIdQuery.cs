using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Countries.Queries.GetCountryById
{
    public class GetCountryByIdQuery : IRequest<CountryDto?>
    {
        public Guid Id { get; set; }

        public GetCountryByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}

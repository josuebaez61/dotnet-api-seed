using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Countries.Queries.GetAllCountries
{
    public class GetAllCountriesQuery : IRequest<List<CountryDto>>
    {
        // No parameters needed for getting all countries
    }
}

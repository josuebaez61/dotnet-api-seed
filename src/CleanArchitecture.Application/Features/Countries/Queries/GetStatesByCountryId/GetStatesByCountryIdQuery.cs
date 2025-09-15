using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Countries.Queries.GetStatesByCountryId
{
    public class GetStatesByCountryIdQuery : IRequest<List<StateDto>>
    {
        public int CountryId { get; set; }

        public GetStatesByCountryIdQuery(int countryId)
        {
            CountryId = countryId;
        }
    }
}

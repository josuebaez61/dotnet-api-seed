using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Countries.Queries.GetStatesByCountryId
{
    public class GetStatesByCountryIdQuery : IRequest<List<StateDto>>
    {
        public Guid CountryId { get; set; }

        public GetStatesByCountryIdQuery(Guid countryId)
        {
            CountryId = countryId;
        }
    }
}

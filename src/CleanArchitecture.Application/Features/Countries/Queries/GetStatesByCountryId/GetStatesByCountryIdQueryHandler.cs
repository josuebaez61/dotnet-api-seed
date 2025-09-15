using AutoMapper;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Features.Countries.Queries.GetStatesByCountryId
{
    public class GetStatesByCountryIdQueryHandler : IRequestHandler<GetStatesByCountryIdQuery, List<StateDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetStatesByCountryIdQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<StateDto>> Handle(GetStatesByCountryIdQuery request, CancellationToken cancellationToken)
        {
            var states = await _context.States
                .Where(s => s.CountryId == request.CountryId)
                .OrderBy(s => s.Name)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<StateDto>>(states);
        }
    }
}

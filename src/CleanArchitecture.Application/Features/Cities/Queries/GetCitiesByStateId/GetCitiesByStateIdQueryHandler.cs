using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Features.Cities.Queries.GetCitiesByStateId
{
    public class GetCitiesByStateIdQueryHandler : IRequestHandler<GetCitiesByStateIdQuery, List<CityDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetCitiesByStateIdQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<CityDto>> Handle(GetCitiesByStateIdQuery request, CancellationToken cancellationToken)
        {
            var cities = await _context.Cities
                .Where(c => c.StateId == request.StateId)
                .Include(c => c.State)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<CityDto>>(cities);
        }
    }
}

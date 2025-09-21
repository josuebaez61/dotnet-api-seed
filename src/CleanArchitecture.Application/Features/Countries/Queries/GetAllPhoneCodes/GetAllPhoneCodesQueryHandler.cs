using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Features.Countries.Queries.GetAllPhoneCodes
{
  /// <summary>
  /// Handler for getting all phone codes for country selection
  /// </summary>
  public class GetAllPhoneCodesQueryHandler : IRequestHandler<GetAllPhoneCodesQuery, List<PhoneCodeDto>>
  {
    private readonly IApplicationDbContext _context;

    public GetAllPhoneCodesQueryHandler(IApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<List<PhoneCodeDto>> Handle(GetAllPhoneCodesQuery request, CancellationToken cancellationToken)
    {
      var phoneCodes = await _context.Countries
          .Where(c => !string.IsNullOrEmpty(c.Phonecode) && !string.IsNullOrEmpty(c.Iso2))
          .OrderBy(c => c.Name)
          .Select(c => new PhoneCodeDto
          {
            Id = c.Id,
            Name = c.Name,
            Iso2 = c.Iso2!,
            PhoneCode = c.Phonecode!,
            Emoji = "🏳️" // Default emoji since Country entity doesn't have Emoji property
          })
          .ToListAsync(cancellationToken);

      return phoneCodes;
    }
  }
}

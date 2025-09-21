using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Countries.Queries.GetAllPhoneCodes
{
  /// <summary>
  /// Query to get all phone codes for country selection
  /// </summary>
  public class GetAllPhoneCodesQuery : IRequest<List<PhoneCodeDto>>
  {
  }
}

using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Features.Countries.Queries.GetAllCountries;
using CleanArchitecture.Application.Features.Countries.Queries.GetAllPhoneCodes;
using CleanArchitecture.Application.Features.Countries.Queries.GetCountryById;
using CleanArchitecture.Application.Features.Countries.Queries.GetStatesByCountryId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class CountriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CountriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all countries
        /// </summary>
        /// <returns>List of all countries</returns>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<CountryDto>>>> GetAllCountries()
        {
            var query = new GetAllCountriesQuery();
            var result = await _mediator.Send(query);

            return Ok(ApiResponse<List<CountryDto>>.SuccessResponse(result));
        }

        /// <summary>
        /// Get a country by ID
        /// </summary>
        /// <param name="id">Country ID</param>
        /// <returns>Country details</returns>
        [HttpGet("id/{id}")]
        public async Task<ActionResult<ApiResponse<CountryDto>>> GetCountryById(int id)
        {
            var query = new GetCountryByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound(ApiResponse<CountryDto>.ErrorResponse("Country not found"));
            }

            return Ok(ApiResponse<CountryDto>.SuccessResponse(result));
        }

        /// <summary>
        /// Get all states for a specific country
        /// </summary>
        /// <param name="countryId">Country ID</param>
        /// <returns>List of states for the country</returns>
        [HttpGet("{countryId}/states")]
        public async Task<ActionResult<ApiResponse<List<StateDto>>>> GetStatesByCountryId(int countryId)
        {
            var query = new GetStatesByCountryIdQuery(countryId);
            var result = await _mediator.Send(query);

            return Ok(ApiResponse<List<StateDto>>.SuccessResponse(result));
        }

        /// <summary>
        /// Get all phone codes for country selection in phone number inputs
        /// </summary>
        /// <returns>List of phone codes with country information</returns>
        [HttpGet("phone-codes")]
        public async Task<ActionResult<ApiResponse<List<PhoneCodeDto>>>> GetAllPhoneCodes()
        {
            var query = new GetAllPhoneCodesQuery();
            var result = await _mediator.Send(query);

            return Ok(ApiResponse<List<PhoneCodeDto>>.SuccessResponse(result));
        }
    }
}

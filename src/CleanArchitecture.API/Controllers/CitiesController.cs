using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Features.Cities.Queries.GetAllCities;
using CleanArchitecture.Application.Features.Cities.Queries.GetCitiesByStateId;
using CleanArchitecture.Application.Features.Cities.Queries.GetCityById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class CitiesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CitiesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all cities
        /// </summary>
        /// <returns>List of all cities</returns>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<CityDto>>>> GetAllCities()
        {
            var query = new GetAllCitiesQuery();
            var result = await _mediator.Send(query);

            return Ok(ApiResponse<List<CityDto>>.SuccessResponse(result));
        }

        /// <summary>
        /// Get a city by ID
        /// </summary>
        /// <param name="id">City ID</param>
        /// <returns>City details</returns>
        [HttpGet("id/{id}")]
        public async Task<ActionResult<ApiResponse<CityDto>>> GetCityById(int id)
        {
            var query = new GetCityByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound(ApiResponse<CityDto>.ErrorResponse("City not found"));
            }

            return Ok(ApiResponse<CityDto>.SuccessResponse(result));
        }

        /// <summary>
        /// Get all cities for a specific state
        /// </summary>
        /// <param name="stateId">State ID</param>
        /// <returns>List of cities for the state</returns>
        [HttpGet("{stateId}/cities")]
        public async Task<ActionResult<ApiResponse<List<CityDto>>>> GetCitiesByStateId(int stateId)
        {
            var query = new GetCitiesByStateIdQuery(stateId);
            var result = await _mediator.Send(query);

            return Ok(ApiResponse<List<CityDto>>.SuccessResponse(result));
        }
    }
}

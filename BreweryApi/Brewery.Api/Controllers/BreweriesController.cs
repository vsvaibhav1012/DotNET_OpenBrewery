using Brewery.Application.DTOs;
using Brewery.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Brewery.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class BreweriesController : ControllerBase
    {
        private readonly IBreweryService _breweryService;

        public BreweriesController(IBreweryService breweryService)
        {
            _breweryService = breweryService;
        }

        /// <summary>
        /// Get breweries with optional filtering, sorting, and pagination
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<BreweryDto>>>> GetBreweries(
            [FromQuery] BrewerySearchRequest request)
        {
            var result = await _breweryService.GetBreweriesAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get brewery by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<BreweryDto>>> GetBrewery(string id)
        {
            var result = await _breweryService.GetBreweryByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Get autocomplete suggestions for brewery names
        /// </summary>
        [HttpGet("autocomplete")]
        public async Task<ActionResult<ApiResponse<IEnumerable<string>>>> GetAutocomplete(
            [FromQuery] string searchTerm)
        {
            var result = await _breweryService.GetAutocompleteAsync(searchTerm);
            return Ok(result);
        }
    }
}
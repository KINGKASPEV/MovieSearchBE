using Microsoft.AspNetCore.Mvc;
using MovieSearch.Application.ServiceImplementation;
using MovieSearch.Domain.Entities;

namespace MovieSearch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly MovieService _movieService;

        public MovieController(MovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet("search/{title}")]
        public async Task<ActionResult<IEnumerable<MovieResult>>> SearchMovie(string title)
        {
            var searchResults = await _movieService.SearchMovieByTitleAsync(title);

            if (searchResults != null && searchResults.Any())
            {
                // Your existing code to save search history...
                return Ok(searchResults);
            }
            else
            {
                return NotFound("No results found");
            }
        }

        [HttpGet("history")]
        public ActionResult<IEnumerable<string>> GetSearchHistory()
        {
            // Your existing code to get search history...
            return Ok(SearchHistory);
        }

        [HttpGet("{imdbId}")]
        public async Task<ActionResult<MovieDetail>> GetMovieDetails(string imdbId)
        {
            var movieDetails = await _movieService.GetMovieDetailsAsync(imdbId);

            if (movieDetails != null)
            {
                return Ok(movieDetails);
            }
            else
            {
                return NotFound("Movie details not found");
            }
        }
    }

}
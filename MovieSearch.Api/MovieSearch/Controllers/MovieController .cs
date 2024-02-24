using Microsoft.AspNetCore.Mvc;
using MovieSearch.Application.ServiceImplementation;
using MovieSearch.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieSearch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly MovieService _movieService;

        // Assuming you want to store search history in the controller
        private static List<string> SearchHistory = new List<string>();

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
                // Save search history
                SearchHistory.Insert(0, title);
                if (SearchHistory.Count > 5)
                    SearchHistory.RemoveAt(5);

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

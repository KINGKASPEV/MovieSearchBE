using Microsoft.AspNetCore.Mvc;
using MovieSearch.Application.Interface;
using MovieSearch.Application.ServiceImplementation;
using MovieSearch.Domain.Entities;

namespace MovieSearch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;

        // Assuming you want to store search history in the controller
        private static List<string> SearchHistory = new List<string>();

        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet("search/{title}")]
        public async Task<ActionResult<IEnumerable<MovieResult>>> SearchMovie(string title)
        {
            var searchResults = await _movieService.SearchMovieByTitleAsync(title);

            if (searchResults != null && searchResults.Data != null && searchResults.Data.Any())
            {
                SearchHistory.Insert(0, title);
                if (SearchHistory.Count > 5)
                    SearchHistory.RemoveAt(5);

                return Ok(searchResults.Data);
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

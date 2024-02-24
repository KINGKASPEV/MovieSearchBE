using Microsoft.Extensions.Options;
using MovieSearch.Application.Interface;
using MovieSearch.Domain;
using MovieSearch.Domain.Entities;
using Newtonsoft.Json;

namespace MovieSearch.Application.ServiceImplementation
{
    public class MovieService : IMovieService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AppSettings _appSettings;

        public MovieService(IHttpClientFactory httpClientFactory, IOptions<AppSettings> appSettings)
        {
            _httpClientFactory = httpClientFactory;
            _appSettings = appSettings?.Value;
        }

        public async Task<ApiResponse<IEnumerable<MovieResult>>> SearchMovieByTitleAsync(string title)
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient();
                string apiUrl = $"http://www.omdbapi.com/?apikey={_appSettings.OMDBApiKey}&s={title}";

                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<SearchResult>(content);

                    if (result?.Search != null && result.Search.Any())
                    {
                        return ApiResponse<IEnumerable<MovieResult>>.Success(result.Search, "Search successful", (int)response.StatusCode);
                    }
                }

                return ApiResponse<IEnumerable<MovieResult>>.Failed(false, "No results found", (int)response.StatusCode, new List<string>());
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<MovieResult>>.Failed(false, $"An error occurred: {ex.Message}", 500, new List<string> { ex.StackTrace });
            }
        }

        public async Task<ApiResponse<MovieDetail>> GetMovieDetailsAsync(string imdbId)
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient();
                string apiUrl = $"http://www.omdbapi.com/?apikey={_appSettings.OMDBApiKey}&i={imdbId}";

                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<MovieDetail>(content);

                    return ApiResponse<MovieDetail>.Success(result, "Details fetched successfully", (int)response.StatusCode);
                }

                return ApiResponse<MovieDetail>.Failed(false, "Movie details not found", (int)response.StatusCode, new List<string>());
            }
            catch (Exception ex)
            {
                return ApiResponse<MovieDetail>.Failed(false, $"An error occurred: {ex.Message}", 500, new List<string> { ex.StackTrace });
            }
        }
    }
}

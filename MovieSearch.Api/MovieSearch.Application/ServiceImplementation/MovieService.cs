using Microsoft.Extensions.Options;
using MovieSearch.Application.Interface;
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
            _appSettings = appSettings.Value;
        }

        public async Task<IEnumerable<MovieResult>> SearchMovieByTitleAsync(string title)
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
                    return result.Search;
                }
            }

            return null;
        }

        public async Task<MovieDetail> GetMovieDetailsAsync(string imdbId)
        {
            HttpClient client = _httpClientFactory.CreateClient();
            string apiUrl = $"http://www.omdbapi.com/?apikey={_appSettings.OMDBApiKey}&i={imdbId}";

            HttpResponseMessage response = await client.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<MovieDetail>(content);

                return result;
            }

            return null;
        }
    }

}
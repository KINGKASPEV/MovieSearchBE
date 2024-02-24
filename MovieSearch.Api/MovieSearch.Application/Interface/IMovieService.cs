using MovieSearch.Domain;
using MovieSearch.Domain.Entities;

namespace MovieSearch.Application.Interface
{
    public interface IMovieService
    {
        Task<ApiResponse<IEnumerable<MovieResult>>> SearchMovieByTitleAsync(string title);
        Task<ApiResponse<MovieDetail>> GetMovieDetailsAsync(string imdbId);
    }
}

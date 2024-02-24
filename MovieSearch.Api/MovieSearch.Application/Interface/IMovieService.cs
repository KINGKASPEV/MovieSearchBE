using MovieSearch.Domain.Entities;

namespace MovieSearch.Application.Interface
{
    public interface IMovieService
    {
        Task<IEnumerable<MovieResult>> SearchMovieByTitleAsync(string title);
        Task<MovieDetail> GetMovieDetailsAsync(string imdbId);
    }
}

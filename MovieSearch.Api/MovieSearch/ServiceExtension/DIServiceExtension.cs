using MovieSearch.Application.Interface;
using MovieSearch.Application.ServiceImplementation;
using MovieSearch.Domain.Entities;

namespace MovieSearch.ServiceExtension
{
    public static class DIServiceExtension
    {
        public static void AddDependencies(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IMovieService, MovieService>();
            services.Configure<AppSettings>(config.GetSection("AppSettings"));
        }
    }
}

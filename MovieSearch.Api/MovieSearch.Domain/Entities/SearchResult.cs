namespace MovieSearch.Domain.Entities
{
    public class SearchResult
    {
        public List<MovieResult> Search { get; set; }
        public string TotalResults { get; set; }
        public bool Response { get; set; }
    }
}
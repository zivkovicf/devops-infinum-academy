namespace IA.DevOps.Movies.Data.Db.SeedData.Models
{
    internal class MovieCSV
    {
        public string Title { get; set; } = default!;
        public int ReleasedYear { get; set; }
        public string Genre { get; set; } = default!;
        public string Overview { get; set; } = default!;
        public string PosterLink { get; set; } = default!;
        public double IMDBRating { get; set; } = default!;
    }
}

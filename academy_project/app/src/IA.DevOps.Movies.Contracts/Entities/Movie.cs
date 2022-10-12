namespace IA.DevOps.Movies.Contracts.Entities
{
    public class Movie : Base
    {
        public string Title { get; set; } = default!;
        public int ReleasedYear { get; set; }
        public string Genre { get; set; } = default!;
        public string Overview { get; set; } = default!;
        public double Rating { get; set; } = default!;
    }
}

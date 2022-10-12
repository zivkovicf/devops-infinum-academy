namespace IA.DevOps.Movies.Contracts.Forms
{
    public class MovieForm
    {
        public Guid? Id { get; set; }
        public string Title { get; set; } = default!;
        public int ReleasedYear { get; set; }
        public double Rating { get; set; }
        public string Genre { get; set; } = default!;
        public Stream? Image { get; set; } = default!;
        public string? ImageUrl { get; set; } = default!;
        public string Overview { get; set; } = default!;
    }
}

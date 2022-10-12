using IA.DevOps.Movies.Contracts.Forms;
using IA.DevOps.Movies.Contracts.Services;
using Microsoft.AspNetCore.Components;

namespace IA.DevOps.Movies.Web.Pages.Movies
{
    public partial class Edit
    {
        [Parameter]
        public Guid Id { get; set; }

        [Inject]
        private IMovieService _movieService { get; set; } = default!;
        [Inject]
        private NavigationManager _navigationManager { get; set; } = default!;

        private MovieForm _form { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            var movie = await _movieService.Get(Id);

            _form = new MovieForm
            {
                Id = Id,
                Genre = movie.Genre,
                ImageUrl = movie.ImageUrl,
                Overview = movie.Overview,
                ReleasedYear = movie.ReleasedYear,
                Rating = movie.Rating,
                Title = movie.Title
            };
        }

        protected async Task Update()
        {
            await _movieService.Update(_form);
            _navigationManager.NavigateTo("/movies", forceLoad: true);
        }
    }
}

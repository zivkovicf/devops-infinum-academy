using IA.DevOps.Movies.Contracts.Forms;
using IA.DevOps.Movies.Contracts.Services;
using Microsoft.AspNetCore.Components;

namespace IA.DevOps.Movies.Web.Pages.Movies
{
    public partial class Create
    {
        [Inject]
        private IMovieService _movieService { get; set; } = default!;
        [Inject]
        private NavigationManager _navigationManager { get; set; } = default!;

        private MovieForm _form { get; set; } = new();

        protected async Task Save()
        {
            await _movieService.Create(_form);
            _navigationManager.NavigateTo("/movies", forceLoad: true);
        }
    }
}

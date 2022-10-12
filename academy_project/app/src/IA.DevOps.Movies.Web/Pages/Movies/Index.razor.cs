using IA.DevOps.Movies.Contracts.DTOs;
using IA.DevOps.Movies.Contracts.Services;
using Microsoft.AspNetCore.Components;

namespace IA.DevOps.Movies.Web.Pages.Movies
{
    public partial class Index
    {
        [Inject]
        private IMovieService _movieService { get; set; } = default!;

        private List<MovieDTO> _movies { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            _movies = await _movieService.GetAll();
        }
    }
}

using IA.DevOps.Movies.Contracts.Services;
using Microsoft.AspNetCore.Mvc;

namespace IA.DevOps.Movies.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var movies = await _movieService.GetAll();

            return Ok(movies);
        }
    }
}

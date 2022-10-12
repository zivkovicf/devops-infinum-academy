using IA.DevOps.Movies.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace IA.DevOps.Movies.Web.Controllers
{
    [Route("api/loki-superhero")]
    [ApiController]
    public class LokiSuperheroController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get(int? power)
        {
            new CPULoadGenerator().Run(duration: power);

            return Ok(power);
        }
    }
}

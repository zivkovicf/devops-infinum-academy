using IA.DevOps.Movies.Contracts.Services;
using Microsoft.AspNetCore.Mvc;

namespace IA.DevOps.Movies.Web.Controllers
{
    [Route("api/health-check")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        private readonly IHealthCheckService _healthCheckService;

        public HealthCheckController(IHealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var stats = await _healthCheckService.GetCurrentStats();

            return Ok(stats);
        }
    }
}

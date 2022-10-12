using IA.DevOps.Movies.Contracts.DTOs;

namespace IA.DevOps.Movies.Contracts.Services
{
    public interface IHealthCheckService
    {
        Task<SystemInformationDTO> GetCurrentStats();
    }
}

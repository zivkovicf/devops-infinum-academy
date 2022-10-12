using IA.DevOps.Movies.Contracts.DTOs;
using IA.DevOps.Movies.Contracts.Forms;

namespace IA.DevOps.Movies.Contracts.Services
{
    public interface IMovieService
    {
        Task<MovieDTO> Get(Guid id);
        Task<List<MovieDTO>> GetAll();
        Task<MovieDTO> Create(MovieForm movieForm);
        Task<MovieDTO> Update(MovieForm movieForm);
    }
}

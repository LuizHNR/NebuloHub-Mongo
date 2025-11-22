using Infrastructure.Repositories;
using NebuloMongo.Application.DTOs.Request;
using NebuloMongo.Application.DTOs.Response;
using NebuloMongo.Domain.Entities;

namespace NebuloMongo.Application.UseCase
{
    public class StartupUseCase
    {
        private readonly IRepository<Startup> _repository;

        public StartupUseCase(IRepository<Startup> repository)
        {
            _repository = repository;
        }

        public async Task<ResponseStartupDto> CreateStartupAsync(RequestStartupDto request)
        {
            var startup = Startup.Create(
                request.CNPJ,
                request.Video,
                request.Name,
                request.Descricao,
                request.Site,
                request.DataCriacao,
                request.Habilidades,
                request.IdUser
            );

            await _repository.AddAsync(startup);

            return ResponseStartupDto.FromEntity(startup);
        }

        public async Task<List<ResponseStartupDto>> GetAllStartupsAsync(int page, int pageSize)
        {
            var startups = await _repository.GetAllAsync();

            var paged = startups
               .Skip((page - 1) * pageSize)
               .Select(u => ResponseStartupDto.FromEntity(u))
               .Take(pageSize)
               .Select(u => new ResponseStartupDto
               {
                   Id = u.Id,
                   Video = u.Video,
                   CNPJ = u.CNPJ,
                   Name = u.Name
                   
               })
               .ToList();

            return paged;
        }



        public async Task<ResponseStartupDto?> GetByIdAsync(string id)
        {
            var startup = await _repository.GetByIdAsync(id);
            return startup == null ? null : ResponseStartupDto.FromEntity(startup);
        }

        public async Task<ResponseStartupDto?> UpdateStartupAsync(string id, RequestStartupDto request)
        {
            var startup = await _repository.GetByIdAsync(id);
            if (startup == null) return null;

            startup.Atualizar(
                request.CNPJ,
                request.Video,
                request.Name,
                request.Descricao,
                request.Site,
                request.DataCriacao,
                request.Habilidades,
                request.IdUser
            );

            await _repository.UpdateAsync(id, startup);
            return ResponseStartupDto.FromEntity(startup);
        }

        public async Task<bool> DeleteStartupAsync(string id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            await _repository.DeleteAsync(id);
            return true;
        }
    }
}

using Infrastructure.Repositories;
using NebuloMongo.Application.DTOs.Request;
using NebuloMongo.Application.DTOs.Response;
using NebuloMongo.Domain.Entities;

namespace NebuloMongo.Application.UseCase
{
    public class UserUseCase
    {
        private readonly IRepository<User> _repository;

        public UserUseCase(IRepository<User> repository)
        {
            _repository = repository;
        }

        public async Task<ResponseUserDto> CreateUserAsync(RequestUserDto request)
        {
            var user = User.Create(
                request.CPF,
                request.Name,
                request.Email,
                request.Password,
                request.Role,
                request.Telefone
            );

            await _repository.AddAsync(user);

            return ResponseUserDto.FromEntity(user);
        }

        public async Task<List<ResponseUserDto>> GetAllUsersAsync(int page, int pageSize)
        {
            var users = await _repository.GetAllAsync();

            var paged = users
               .Skip((page - 1) * pageSize)
               .Select(u => ResponseUserDto.FromEntity(u))
               .Take(pageSize)
               .Select(u => new ResponseUserDto
               {
                   Id = u.Id,
                   CPF = u.CPF,
                   Name = u.Name,
                   Email = u.Email
               })
               .ToList();

            return paged;
        }



        public async Task<ResponseUserDto?> GetByIdAsync(string id)
        {
            var user = await _repository.GetByIdAsync(id);
            return user == null ? null : ResponseUserDto.FromEntity(user);
        }

        public async Task<ResponseUserDto?> UpdateUserAsync(string id, RequestUserDto request)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null) return null;

            user.Atualizar(
                request.Name,
                request.Email,
                request.Password,
                request.Role,
                request.Telefone
            );

            await _repository.UpdateAsync(id, user);
            return ResponseUserDto.FromEntity(user);
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            await _repository.DeleteAsync(id);
            return true;
        }
    }
}

using Infrastructure.Repositories;
using Microsoft.IdentityModel.Tokens;
using NebuloMongo.Application.DTOs.Request;
using NebuloMongo.Application.DTOs.Response;
using NebuloMongo.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NebuloMongo.Application.UseCase
{
    public class AuthUseCase
    {
        private readonly IRepository<User> _repository;
        private readonly IConfiguration _configuration;

        public AuthUseCase(IRepository<User> repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        public async Task<ResponseAuthDto?> LoginAsync(RequestLoginDto request)
        {
            var users = await _repository.GetAllAsync();
            var user = users.FirstOrDefault(u => u.Email == request.Email);

            if (user == null)
                return null;

            if (user.Password != request.Password)
                return null;

            var token = GenerateToken(user);

            return new ResponseAuthDto
            {
                Token = token,
                Email = user.Email,
                Role = user.Role.ToString(),
                UserId = user.Id
            };
        }

        private string GenerateToken(User user)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

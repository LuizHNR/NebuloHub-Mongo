using Microsoft.AspNetCore.Mvc;
using NebuloMongo.Application.DTOs.Request;
using NebuloMongo.Application.UseCase;

namespace NebuloMongo.API.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthUseCase _authUseCase;

        public AuthController(AuthUseCase authUseCase)
        {
            _authUseCase = authUseCase;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] RequestLoginDto request)
        {
            var response = await _authUseCase.LoginAsync(request);

            if (response == null)
                return Unauthorized("E-mail ou senha inválidos.");

            return Ok(response);
        }
    }
}

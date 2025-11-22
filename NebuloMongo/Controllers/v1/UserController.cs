using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using NebuloMongo.Application.DTOs.Request;
using NebuloMongo.Application.UseCase;

namespace NebuloMongo.API.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserUseCase _useCase;

        public UserController(UserUseCase useCase)
        {
            _useCase = useCase;
        }


        /// <summary>
        /// Retorna todos os Users.
        /// </summary>
        /// <param name="page">Número da página (default = 1)</param>
        /// <param name="pageSize">Quantidade de itens por página (default = 10)</param>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var users = await _useCase.GetAllUsersAsync(page, pageSize);

                var result = users.Select(d => new
                {
                    d.CPF,
                    d.Name,
                    d.Email,
                    links = new
                    {
                        self = Url.Action(nameof(GetById), new { id = d.Id })
                    }
                });

                return Ok(new
                {
                    page,
                    pageSize,
                    totalItems = users.Count(),
                    items = result
                });

            }
            catch (MongoException ex)
            {
                return BadRequest(new { erro = "Erro no MongoDB: " + ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }


        }





        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _useCase.GetByIdAsync(id);
            if (user == null)
                return NotFound("Usuário não encontrado.");

            return Ok(user);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RequestUserDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _useCase.CreateUserAsync(request);

            return CreatedAtAction(nameof(GetById), new { id = created.Id, version = "1" }, created);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] RequestUserDto request)
        {
            var updated = await _useCase.UpdateUserAsync(id, request);
            if (updated == null)
                return NotFound("Usuário não encontrado.");

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _useCase.DeleteUserAsync(id);
            if (!deleted)
                return NotFound("Usuário não encontrado.");

            return NoContent();
        }
    }
}

using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using NebuloMongo.Application.DTOs.Request;
using NebuloMongo.Application.DTOs.Response;
using NebuloMongo.Application.UseCase;
using NebuloMongo.Application.Validators;
using System.Net;

namespace NebuloMongo.API.Controllers.v2
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    [ApiController]
    [Tags("CRUD User")]
    public class UserController : ControllerBase
    {
        private readonly UserUseCase _useCase;
        private readonly RequestUserValidator _validationUser;

        // ILogger
        private readonly ILogger<UserController> _logger;


        public UserController(UserUseCase useCase, RequestUserValidator validationUser,
        ILogger<UserController> logger)
        {
            _useCase = useCase;
            _validationUser = validationUser;
            _logger = logger;
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
                _logger.LogInformation("Iniciando busca de todos os users...");

                var users = await _useCase.GetAllUsersAsync(page, pageSize);

                _logger.LogInformation("Busca de usuários concluída. {count} registros encontrados.", users.Count());

                var result = users.Select(d => new
                {
                    d.Id,
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
                _logger.LogError(ex, "Erro inesperado.");
                return StatusCode(500, new { erro = ex.Message });
            }


        }



        /// <summary>
        /// Retorna um User pelo ID.
        /// </summary>
        /// <param name="id">id do registro</param>
        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseUserDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetById(string id)
        {

            try
            {
                _logger.LogInformation("Buscando user com id {id}", id);

                var user = await _useCase.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User {id} não encontrado.", id);
                    return NotFound("Usuário não encontrado.");

                }

                _logger.LogInformation("User {id} encontrado com sucesso.", id);
                return Ok(user);

            }
            catch (MongoException ex)
            {
                return BadRequest(new { erro = "Erro no MongoDB: " + ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado.");
                return StatusCode(500, new { erro = ex.Message });
            }
        }



        /// <summary>
        /// Cria um novo User.
        /// </summary>
        /// <param name="request">Payload para criação</param>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(ResponseUserDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> PostUser([FromBody] RequestUserDto request)
        {

            try
            {
                // Valida entrada
                _validationUser.ValidateAndThrow(request);

                var created = await _useCase.CreateUserAsync(request);

                return CreatedAtAction(nameof(GetById), new { id = created.Id, version = "2" }, created);

            }
            catch (ValidationException ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
            catch (MongoException ex)
            {
                return BadRequest(new { erro = "Erro no MongoDB: " + ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado.");
                return StatusCode(500, new { erro = ex.Message });
            }

        }




        /// <summary>
        /// Atualiza um Usuario existente.
        /// </summary>
        /// <param name="id">ID do registro</param>
        /// <param name="request">Payload para atualização</param>
        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PutUser(string id, [FromBody] RequestUserDto request)
        {

            try
            {
                _logger.LogInformation("Atualizando user {id}", id);

                var updated = await _useCase.UpdateUserAsync(id, request);

                if (!updated)
                    return NotFound("Usuário não encontrado.");

                return Ok(updated);

            }
            catch (ValidationException ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
            catch (MongoException ex)
            {
                return BadRequest(new { erro = "Erro no MongoDB: " + ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado.");
                return StatusCode(500, new { erro = ex.Message });
            }

        }





        /// <summary>
        /// Deleta um User existente.
        /// </summary>
        /// <param name="id">ID do registro</param>
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteUser(string id)
        {

            try
            {
                var deleted = await _useCase.DeleteUserAsync(id);
                if (!deleted)
                    return NotFound("Usuário não encontrado.");

                return Ok(new { mensagem = "Usuário deletado com sucesso." });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao deletar user.");
                return StatusCode(500, new { erro = ex.Message });
            }

        }
    }
}

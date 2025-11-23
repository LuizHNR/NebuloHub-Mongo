using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using NebuloMongo.Application.DTOs.Request;
using NebuloMongo.Application.DTOs.Response;
using NebuloMongo.Application.UseCase;
using NebuloMongo.Application.Validators;
using System.Net;

namespace NebuloMongo.Controllers.v2
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    [ApiController]
    [Tags("CRUD Startup")]
    public class StartupController : ControllerBase
    {
        private readonly StartupUseCase _useCase;
        private readonly RequestStartupValidator _validationStartup;

        // ILogger
        private readonly ILogger<StartupController> _logger;


        public StartupController(StartupUseCase useCase, RequestStartupValidator validationStartup,
        ILogger<StartupController> logger)
        {
            _useCase = useCase;
            _validationStartup = validationStartup;
            _logger = logger;
        }



        /// <summary>
        /// Retorna todos os Startups.
        /// </summary>
        /// <param name="page">Número da página (default = 1)</param>
        /// <param name="pageSize">Quantidade de itens por página (default = 10)</param>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Iniciando busca de todos os startups...");

                var startups = await _useCase.GetAllStartupsAsync(page, pageSize);

                _logger.LogInformation("Busca de usuários concluída. {count} registros encontrados.", startups.Count());

                var result = startups.Select(d => new
                {
                    d.Id,
                    d.Video,
                    d.CNPJ,
                    d.Name,
                    links = new
                    {
                        self = Url.Action(nameof(GetById), new { id = d.Id })
                    }
                });

                return Ok(new
                {
                    page,
                    pageSize,
                    totalItems = startups.Count(),
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
        /// Retorna um Startup pelo ID.
        /// </summary>
        /// <param name="id">id do registro</param>
        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseStartupDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetById(string id)
        {

            try
            {
                _logger.LogInformation("Buscando startup com id {id}", id);

                var startup = await _useCase.GetByIdAsync(id);
                if (startup == null)
                {
                    _logger.LogWarning("Startup {id} não encontrado.", id);
                    return NotFound("Usuário não encontrado.");

                }

                _logger.LogInformation("Startup {id} encontrado com sucesso.", id);
                return Ok(startup);

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
        /// Cria um novo Startup.
        /// </summary>
        /// <param name="request">Payload para criação</param>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(ResponseStartupDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> PostStartup([FromBody] RequestStartupDto request)
        {

            try
            {
                // Valida entrada
                _validationStartup.ValidateAndThrow(request);

                var created = await _useCase.CreateStartupAsync(request);

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
        public async Task<IActionResult> PutStartup(string id, [FromBody] RequestStartupDto request)
        {

            try
            {
                _logger.LogInformation("Atualizando startup {id}", id);

                var updated = await _useCase.UpdateStartupAsync(id, request);

                if (updated == null)
                    return NotFound("Startup não encontrada.");

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
        /// Deleta um Startup existente.
        /// </summary>
        /// <param name="id">ID do registro</param>
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteStartup(string id)
        {

            try
            {
                var deleted = await _useCase.DeleteStartupAsync(id);
                if (!deleted)
                    return NotFound("Usuário não encontrado.");

                return Ok(new { mensagem = "Usuário deletado com sucesso." });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao deletar startup.");
                return StatusCode(500, new { erro = ex.Message });
            }

        }
    }
}

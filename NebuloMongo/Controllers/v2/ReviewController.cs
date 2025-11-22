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
    [Tags("CRUD Review")]
    public class ReviewController : ControllerBase
    {
        private readonly ReviewUseCase _useCase;
        private readonly RequestReviewValidator _validationReview;

        // ILogger
        private readonly ILogger<ReviewController> _logger;


        public ReviewController(ReviewUseCase useCase, RequestReviewValidator validationReview,
        ILogger<ReviewController> logger)
        {
            _useCase = useCase;
            _validationReview = validationReview;
            _logger = logger;
        }



        /// <summary>
        /// Retorna todos os Reviews.
        /// </summary>
        /// <param name="page">Número da página (default = 1)</param>
        /// <param name="pageSize">Quantidade de itens por página (default = 10)</param>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Iniciando busca de todos os reviews...");

                var reviews = await _useCase.GetAllReviewsAsync(page, pageSize);

                _logger.LogInformation("Busca de usuários concluída. {count} registros encontrados.", reviews.Count());

                var result = reviews.Select(d => new
                {
                    d.Id,
                    d.Rating,
                    d.UserId,
                    links = new
                    {
                        self = Url.Action(nameof(GetById), new { id = d.Id })
                    }
                });

                return Ok(new
                {
                    page,
                    pageSize,
                    totalItems = reviews.Count(),
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
        /// Retorna um Review pelo ID.
        /// </summary>
        /// <param name="id">id do registro</param>
        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseReviewDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetById(string id)
        {

            try
            {
                _logger.LogInformation("Buscando review com id {id}", id);

                var review = await _useCase.GetByIdAsync(id);
                if (review == null)
                {
                    _logger.LogWarning("Review {id} não encontrado.", id);
                    return NotFound("Usuário não encontrado.");

                }

                _logger.LogInformation("Review {id} encontrado com sucesso.", id);
                return Ok(review);

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
        /// Cria um novo Review.
        /// </summary>
        /// <param name="request">Payload para criação</param>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(ResponseReviewDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> PostReview([FromBody] RequestReviewDto request)
        {

            try
            {
                // Valida entrada
                _validationReview.ValidateAndThrow(request);

                var created = await _useCase.CreateReviewAsync(request);

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
        public async Task<IActionResult> PutReview(string id, [FromBody] RequestReviewDto request)
        {

            try
            {
                _logger.LogInformation("Atualizando review {id}", id);

                var updated = await _useCase.UpdateReviewAsync(id, request);

                if (updated == null)
                    return NotFound("Review não encontrada.");

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
        /// Deleta um Review existente.
        /// </summary>
        /// <param name="id">ID do registro</param>
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteReview(string id)
        {

            try
            {
                var deleted = await _useCase.DeleteReviewAsync(id);
                if (!deleted)
                    return NotFound("Usuário não encontrado.");

                return Ok(new { mensagem = "Usuário deletado com sucesso." });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao deletar review.");
                return StatusCode(500, new { erro = ex.Message });
            }

        }
    }
}

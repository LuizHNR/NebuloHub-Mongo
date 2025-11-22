using Infrastructure.Repositories;
using NebuloMongo.Application.DTOs.Request;
using NebuloMongo.Application.DTOs.Response;
using NebuloMongo.Domain.Entities;

namespace NebuloMongo.Application.UseCase
{
    public class ReviewUseCase
    {
        private readonly IRepository<Review> _repository;

        public ReviewUseCase(IRepository<Review> repository)
        {
            _repository = repository;
        }

        public async Task<ResponseReviewDto> CreateReviewAsync(RequestReviewDto request)
        {
            var review = Review.Create(
                request.Rating,
                request.UserId,
                request.Comment,
                request.DataCriacao,
                request.StartupId
            );

            await _repository.AddAsync(review);

            return ResponseReviewDto.FromEntity(review);
        }

        public async Task<List<ResponseReviewDto>> GetAllReviewsAsync(int page, int pageSize)
        {
            var reviews = await _repository.GetAllAsync();

            var paged = reviews
               .Skip((page - 1) * pageSize)
               .Select(u => ResponseReviewDto.FromEntity(u))
               .Take(pageSize)
               .Select(u => new ResponseReviewDto
               {
                   Id = u.Id,
                   Rating = u.Rating,
                   UserId = u.UserId

               })
               .ToList();

            return paged;
        }



        public async Task<ResponseReviewDto?> GetByIdAsync(string id)
        {
            var review = await _repository.GetByIdAsync(id);
            return review == null ? null : ResponseReviewDto.FromEntity(review);
        }

        public async Task<ResponseReviewDto?> UpdateReviewAsync(string id, RequestReviewDto request)
        {
            var review = await _repository.GetByIdAsync(id);
            if (review == null) return null;

            review.Atualizar(
                request.Rating,
                request.UserId,
                request.Comment,
                request.DataCriacao,
                request.StartupId
            );

            await _repository.UpdateAsync(id, review);
            return ResponseReviewDto.FromEntity(review);
        }

        public async Task<bool> DeleteReviewAsync(string id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            await _repository.DeleteAsync(id);
            return true;
        }
    }
}

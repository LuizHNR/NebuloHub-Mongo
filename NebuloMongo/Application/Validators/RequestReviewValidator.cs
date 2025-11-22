using FluentValidation;
using NebuloMongo.Application.DTOs.Request;

namespace NebuloMongo.Application.Validators
{
    public class RequestReviewValidator : AbstractValidator<RequestReviewDto>
    {
        public RequestReviewValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("Criador é obrigatório.");

            RuleFor(x => x.DataCriacao)
                .NotEmpty().WithMessage("Data de criacao é obrigatória.");

            RuleFor(x => x.StartupId)
                .NotEmpty().WithMessage("Startup é obrigatória.");

        }
    }
}

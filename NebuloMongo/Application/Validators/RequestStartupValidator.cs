using FluentValidation;
using NebuloMongo.Application.DTOs.Request;

namespace NebuloMongo.Application.Validators
{
    public class RequestStartupValidator : AbstractValidator<RequestStartupDto>
    {
        public RequestStartupValidator()
        {
            RuleFor(x => x.CNPJ)
                .NotEmpty().WithMessage("CNPJ é obrigatório.")
                .Length(14).WithMessage("CNPJ deve ter 14 dígitos.")
                .MinimumLength(14).WithMessage("CNPJ deve ter no minimo 1141 caracteres.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Nome é obrigatório.")
                .MaximumLength(250).WithMessage("Nome deve ter no máximo 250 caracteres.");

            RuleFor(x => x.Descricao)
                .NotEmpty().WithMessage("Descricao é obrigatório.");

            RuleFor(x => x.DataCriacao)
                .NotEmpty().WithMessage("Data de criacao é obrigatória.");

            RuleFor(x => x.IdUser)
                .NotEmpty().WithMessage("Criador é obrigatória.");

        }
    }
}

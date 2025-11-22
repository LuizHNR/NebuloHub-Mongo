using FluentValidation;
using NebuloMongo.Application.DTOs.Request;

namespace NebuloMongo.Application.Validators
{
    public class RequestUserValidator : AbstractValidator<RequestUserDto>
    {
        public RequestUserValidator()
        {
            RuleFor(x => x.CPF)
                .NotEmpty().WithMessage("CPF é obrigatório.")
                .Length(11).WithMessage("CPF deve ter 11 dígitos.")
                .MinimumLength(11).WithMessage("Nome deve ter no minimo 11 caracteres.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Nome é obrigatório.")
                .MaximumLength(250).WithMessage("Nome deve ter no máximo 250 caracteres.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-mail é obrigatório.")
                .MaximumLength(255).WithMessage("Senha deve ter no máximo 255 caracteres.")
                .EmailAddress().WithMessage("E-mail inválido.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Senha é obrigatória.")
                .MaximumLength(255).WithMessage("Senha deve ter no máximo 255 caracteres.")
                .Must(s =>
                    s.Any(char.IsUpper) &&                       // Pelo menos uma letra maiúscula
                    s.Any(ch => !char.IsLetterOrDigit(ch)) &&    // Pelo menos um caractere especial
                    s.Length > 8)                                // Maior que 8 caracteres
                .WithMessage("Senha deve conter pelo menos uma letra maiúscula, um caractere especial e ter mais de 8 caracteres.");


            RuleFor(x => x.Role)
                .IsInEnum().WithMessage("Role inválida.");

            RuleFor(x => x.Telefone)
                .GreaterThan(0).When(x => x.Telefone.HasValue)
                .WithMessage("Telefone inválido.");
        }
    }
}

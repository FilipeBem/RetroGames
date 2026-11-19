using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequestApiModel>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Informe o e-mail.")
            .MaximumLength(256).WithMessage("O e-mail deve ter no máximo 256 caracteres.")
            .EmailAddress().WithMessage("E-mail inválido.");
        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("Informe a senha.");
    }
}

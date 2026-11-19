using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Filters;

/// <summary>
/// Filtro de ação: roda depois do model binding e antes da action. Procura o IValidator do tipo de cada
/// argumento; com erro, responde 400 (código 3) com os erros por campo e a action não roda.
/// </summary>
public class FluentValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext contexto, ActionExecutionDelegate proximo)
    {
        foreach (var argumento in contexto.ActionArguments.Values)
        {
            if (argumento is null)
            {
                contexto.Result = new BadRequestObjectResult(new RetroGamesResponseApiModel
                {
                    ResponseCode = ResponseCodes.MODEL_NULL.Code,
                    ErrorMessage = ResponseCodes.MODEL_NULL.DefaultMessage,
                });
                return;
            }

            var tipoValidator = typeof(IValidator<>).MakeGenericType(argumento.GetType());
            if (contexto.HttpContext.RequestServices.GetService(tipoValidator) is not IValidator validator)
            {
                continue; // argumento sem validator (por exemplo, o id da rota)
            }

            var resultado = await validator.ValidateAsync(new ValidationContext<object>(argumento));
            if (!resultado.IsValid)
            {
                var erros = resultado.Errors
                    .GroupBy(e => NomeCampo(e.PropertyName))
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).Distinct().ToArray());

                contexto.Result = new BadRequestObjectResult(
                    new RetroGamesResponseApiModel<Dictionary<string, string[]>>(erros)
                    {
                        ResponseCode = ResponseCodes.MODEL_STATE_INVALIDO.Code,
                        ErrorMessage = ResponseCodes.MODEL_STATE_INVALIDO.DefaultMessage,
                    });
                return;
            }
        }

        await proximo();
    }

    /// <summary>"AnoLancamento" vira "anoLancamento" (camelCase, como no JSON).</summary>
    private static string NomeCampo(string propriedade) =>
        string.IsNullOrEmpty(propriedade) ? "corpo" : char.ToLowerInvariant(propriedade[0]) + propriedade[1..];
}

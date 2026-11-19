using Microsoft.AspNetCore.Mvc;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Configuration;

public static class ModelStateConfig
{
    /// <summary>
    /// Resposta do [ApiController] quando o corpo não chega a virar objeto (JSON malformado, tipo errado,
    /// parâmetro de query inválido): 400, código 3, no envelope. Sem detalhes técnicos do parser.
    /// </summary>
    public static IActionResult CriarResposta(ActionContext contexto)
    {
        var erros = contexto.ModelState
            .Where(entrada => entrada.Value is { Errors.Count: > 0 })
            .ToDictionary(
                entrada => NomeCampo(entrada.Key),
                _ => new[] { "Valor inválido." });

        return new BadRequestObjectResult(new RetroGamesResponseApiModel<Dictionary<string, string[]>>(erros)
        {
            ResponseCode = ResponseCodes.MODEL_STATE_INVALIDO.Code,
            ErrorMessage = ResponseCodes.MODEL_STATE_INVALIDO.DefaultMessage,
        });
    }

    private static string NomeCampo(string chave)
    {
        var campo = chave.TrimStart('$', '.');
        return campo.Length == 0 ? "corpo" : char.ToLowerInvariant(campo[0]) + campo[1..];
    }
}

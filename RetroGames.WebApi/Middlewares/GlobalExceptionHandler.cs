using Microsoft.AspNetCore.Diagnostics;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Middlewares;

/// <summary>Único ponto que transforma exceções em respostas HTTP no formato do envelope.</summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private const string MensagemGenerica = "Ocorreu um erro inesperado. Tente novamente mais tarde.";

    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        RetroGamesResponseApiModel resposta;

        if (exception is BusinessRuleException regra)
        {
            httpContext.Response.StatusCode = (int)regra.Erro.Status;
            resposta = new RetroGamesResponseApiModel
            {
                ResponseCode = regra.Erro.Code,
                ErrorMessage = regra.Message,
            };
        }
        else
        {
            // O erro completo vai só para o log; o cliente recebe uma mensagem genérica.
            _logger.LogError(exception, "Erro não tratado em {Metodo} {Caminho}",
                httpContext.Request.Method, httpContext.Request.Path);

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            resposta = new RetroGamesResponseApiModel
            {
                ResponseCode = ResponseCodes.ERRO,
                ErrorMessage = MensagemGenerica,
            };
        }

        await httpContext.Response.WriteAsJsonAsync(resposta, cancellationToken);
        return true;
    }
}

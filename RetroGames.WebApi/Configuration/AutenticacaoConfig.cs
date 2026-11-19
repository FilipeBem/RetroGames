using Microsoft.AspNetCore.Authentication.Cookies;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Configuration;

public static class AutenticacaoConfig
{
    public const string NomeCookie = "retrogames.sessao";

    public static IServiceCollection AddAutenticacaoConfig(this IServiceCollection services)
    {
        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(opcoes =>
            {
                opcoes.Cookie.Name = NomeCookie;
                opcoes.Cookie.HttpOnly = true;                    // o JavaScript não lê o cookie (RS01)
                opcoes.Cookie.SameSite = SameSiteMode.Strict;     // não vai em requisições de outros sites (defesa contra CSRF)
                opcoes.ExpireTimeSpan = TimeSpan.FromHours(2);
                opcoes.SlidingExpiration = true;                  // renova enquanto há uso

                // Sem redirecionar para uma página de login: 401 e 403 no envelope.
                opcoes.Events.OnRedirectToLogin = contexto =>
                    ResponderAsync(contexto.Response, StatusCodes.Status401Unauthorized, ResponseCodes.NAO_AUTENTICADO);
                opcoes.Events.OnRedirectToAccessDenied = contexto =>
                    ResponderAsync(contexto.Response, StatusCodes.Status403Forbidden, ResponseCodes.ACESSO_NEGADO);
            });

        return services;
    }

    private static Task ResponderAsync(HttpResponse resposta, int status, (int Code, string DefaultMessage) codigo)
    {
        resposta.StatusCode = status;
        return resposta.WriteAsJsonAsync(new RetroGamesResponseApiModel
        {
            ResponseCode = codigo.Code,
            ErrorMessage = codigo.DefaultMessage,
        });
    }
}

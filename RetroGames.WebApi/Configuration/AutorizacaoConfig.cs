using Microsoft.AspNetCore.Authorization;
using RetroGames.Model.Constantes;

namespace RetroGames.WebApi.Configuration;

public static class AutorizacaoConfig
{
    public static IServiceCollection AddAutorizacaoConfig(this IServiceCollection services)
    {
        services.AddAuthorization(opcoes =>
        {
            // API segura por padrão: toda rota exige login, exceto as marcadas com [AllowAnonymous].
            opcoes.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            opcoes.AddPolicy(Politicas.Administrador, politica => politica
                .RequireAuthenticatedUser()
                .RequireRole(Perfis.Administrador));
        });

        return services;
    }
}

using Microsoft.AspNetCore.Identity;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;
using RetroGames.WebApi.Services;

namespace RetroGames.WebApi.Configuration;

public static class InjecaoDependenciaConfig
{
    /// <summary>Registra o RepositoryConfig (uma única vez), cada Repository e o RepositorySet.</summary>
    public static IServiceCollection AddRepositorios(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton(new RepositoryConfig { ConnectionString = connectionString });

        services.AddScoped<IPlataformaRepository, PlataformaRepository>();
        services.AddScoped<IJogoRepository, JogoRepository>();
        services.AddScoped<IJogoGeneroRepository, JogoGeneroRepository>();
        services.AddScoped<IGeneroRepository, GeneroRepository>();
        services.AddScoped<ICondicaoJogoRepository, CondicaoJogoRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IRepositorySet, RepositorySet>();

        return services;
    }

    /// <summary>Registra cada Service com a sua interface (um por requisição).</summary>
    public static IServiceCollection AddServicosDeNegocio(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

        // Quem chama: o Service usa a interface; a WebApi a implementa lendo o cookie (HttpContext).
        services.AddHttpContextAccessor();
        services.AddScoped<IUsuarioAtualService, UsuarioAtualService>();

        services.AddScoped<IPlataformaService, PlataformaService>();
        services.AddScoped<IJogoService, JogoService>();
        services.AddScoped<IGeneroService, GeneroService>();
        services.AddScoped<ICondicaoJogoService, CondicaoJogoService>();
        services.AddScoped<IAutenticacaoService, AutenticacaoService>();

        return services;
    }
}

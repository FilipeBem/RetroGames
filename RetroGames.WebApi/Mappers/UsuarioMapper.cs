using System.Security.Claims;
using RetroGames.Model.Model;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Extensions;

namespace RetroGames.WebApi.Mappers;

public static class UsuarioMapper
{
    public static UsuarioLogadoResponseApiModel ParaResponse(this Usuario usuario) => new()
    {
        Id = usuario.Id,
        Nome = usuario.Nome,
        Email = usuario.Email,
        Perfil = ClaimsPrincipalExtensions.NomeDoPerfil(usuario.IdPerfil),
    };

    /// <summary>Monta a resposta do GET eu a partir das claims da sessão (sem consultar o banco).</summary>
    public static UsuarioLogadoResponseApiModel ParaResponse(this ClaimsPrincipal principal) => new()
    {
        Id = principal.ObterIdUsuario() ?? 0,
        Nome = principal.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
        Email = principal.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
        Perfil = principal.FindFirstValue(ClaimTypes.Role) ?? string.Empty,
    };
}

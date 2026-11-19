using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;

namespace RetroGames.WebApi.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>Monta as claims do usuário (id, nome, e-mail e perfil) que vão, criptografadas, no cookie.</summary>
    public static ClaimsPrincipal ParaClaimsPrincipal(this Usuario usuario)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Email, usuario.Email),
            new(ClaimTypes.Role, NomeDoPerfil(usuario.IdPerfil)),
        };

        var identidade = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return new ClaimsPrincipal(identidade);
    }

    /// <summary>Lê o id do usuário logado, ou nulo se não houver sessão.</summary>
    public static int? ObterIdUsuario(this ClaimsPrincipal principal)
    {
        var valor = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(valor, out var id) ? id : null;
    }

    public static string NomeDoPerfil(int idPerfil) =>
        idPerfil == PerfilIds.Administrador ? Perfis.Administrador : Perfis.Cliente;
}

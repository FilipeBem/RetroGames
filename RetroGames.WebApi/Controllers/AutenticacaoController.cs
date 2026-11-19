using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Extensions;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutenticacaoController : ControllerBase
{
    private readonly IAutenticacaoService _service;

    public AutenticacaoController(IAutenticacaoService service)
    {
        _service = service;
    }

    /// <summary>Autentica e cria a sessão (cookie retrogames.sessao).</summary>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>>> Login(
        LoginRequestApiModel request)
    {
        var usuario = await _service.AutenticarAsync(request.Email, request.Senha);
        if (usuario is null)
        {
            return Unauthorized(new RetroGamesResponseApiModel
            {
                ResponseCode = ResponseCodes.CREDENCIAIS_INVALIDAS.Code,
                ErrorMessage = ResponseCodes.CREDENCIAIS_INVALIDAS.DefaultMessage,
            });
        }

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, usuario.ParaClaimsPrincipal());
        return Ok(new RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>(usuario.ParaResponse()));
    }

    /// <summary>Encerra a sessão. Exige login (política padrão).</summary>
    [HttpPost("logout")]
    public async Task<ActionResult<RetroGamesResponseApiModel>> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new RetroGamesResponseApiModel());
    }

    /// <summary>Dados do usuário logado. Exige login (política padrão).</summary>
    [HttpGet("eu")]
    public ActionResult<RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>> Eu()
    {
        return Ok(new RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>(User.ParaResponse()));
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JogoController : ControllerBase
{
    private readonly IJogoService _service;

    public JogoController(IJogoService service)
    {
        _service = service;
    }

    /// <summary>Catálogo paginado, com busca e filtros por plataforma e gênero.</summary>
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<RetroGamesResponseApiModel<PaginaResponseApiModel<ListarJogoResponseApiModel>>>> Listar(
        [FromQuery] ListarJogoRequestApiModel request)
    {
        var pagina = await _service.ListarAsync(request.ParaFiltro());
        return Ok(new RetroGamesResponseApiModel<PaginaResponseApiModel<ListarJogoResponseApiModel>>(pagina.ParaResponse()));
    }

    /// <summary>Detalhe de um jogo.</summary>
    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>>> ListarPorId(int id)
    {
        var detalhe = await _service.BuscarPorIdAsync(id);
        return Ok(new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>(detalhe.ParaPorIdResponse()));
    }

    /// <summary>Inclui um jogo com os seus gêneros. Responde 201 com o cabeçalho Location.</summary>
    [Authorize(Policy = Politicas.Administrador)]
    [HttpPost]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>>> Inserir(
        InserirJogoRequestApiModel request)
    {
        var criado = await _service.InserirAsync(request.ParaModel(), request.IdsGeneros);
        return CreatedAtAction(nameof(ListarPorId), new { id = criado.Jogo.Id },
            new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>(criado.ParaPorIdResponse()));
    }

    /// <summary>Altera um jogo (recurso completo, com "ativo"). O id vem da rota.</summary>
    [Authorize(Policy = Politicas.Administrador)]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>>> Alterar(
        int id, AlterarJogoRequestApiModel request)
    {
        var alterado = await _service.AlterarAsync(request.ParaModel(id), request.IdsGeneros);
        return Ok(new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>(alterado.ParaPorIdResponse()));
    }

    /// <summary>Exclusão lógica (RN03): o jogo é desativado, nunca apagado. Responde 200 com o envelope.</summary>
    [Authorize(Policy = Politicas.Administrador)]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel>> Desativar(int id)
    {
        await _service.DesativarAsync(id);
        return Ok(new RetroGamesResponseApiModel());
    }
}

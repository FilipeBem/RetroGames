using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlataformaController : ControllerBase
{
    private readonly IPlataformaService _service;

    public PlataformaController(IPlataformaService service)
    {
        _service = service;
    }

    /// <summary>Lista as plataformas.</summary>
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<RetroGamesResponseApiModel<List<ListarPlataformaResponseApiModel>>>> Listar()
    {
        var plataformas = await _service.ListarAsync();
        var dados = plataformas.Select(p => p.ParaResponse()).ToList();
        return Ok(new RetroGamesResponseApiModel<List<ListarPlataformaResponseApiModel>>(dados));
    }

    /// <summary>Consulta uma plataforma pelo id.</summary>
    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>>> ListarPorId(int id)
    {
        var plataforma = await _service.BuscarPorIdAsync(id);
        return Ok(new RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>(plataforma.ParaResponse()));
    }

    /// <summary>Inclui uma plataforma. Responde 201 com o cabeçalho Location.</summary>
    [Authorize(Policy = Politicas.Administrador)]
    [HttpPost]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>>> Inserir(
        InserirPlataformaRequestApiModel request)
    {
        var criada = await _service.InserirAsync(request.ParaModel());
        return CreatedAtAction(nameof(ListarPorId), new { id = criada.Id },
            new RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>(criada.ParaResponse()));
    }

    /// <summary>Altera uma plataforma (recurso completo). O id vem da rota.</summary>
    [Authorize(Policy = Politicas.Administrador)]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>>> Alterar(
        int id, AlterarPlataformaRequestApiModel request)
    {
        var alterada = await _service.AlterarAsync(request.ParaModel(id));
        return Ok(new RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>(alterada.ParaResponse()));
    }

    /// <summary>Exclui uma plataforma sem jogos vinculados. Responde 200 com o envelope.</summary>
    [Authorize(Policy = Politicas.Administrador)]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel>> Excluir(int id)
    {
        await _service.ExcluirAsync(id);
        return Ok(new RetroGamesResponseApiModel());
    }
}

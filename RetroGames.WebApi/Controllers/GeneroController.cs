using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GeneroController : ControllerBase
{
    private readonly IGeneroService _service;

    public GeneroController(IGeneroService service)
    {
        _service = service;
    }

    /// <summary>Lista os gêneros.</summary>
    [HttpGet]
    public async Task<ActionResult<RetroGamesResponseApiModel<List<ListarGeneroResponseApiModel>>>> Listar()
    {
        var generos = await _service.ListarAsync();
        var dados = generos.Select(g => g.ParaResponse()).ToList();
        return Ok(new RetroGamesResponseApiModel<List<ListarGeneroResponseApiModel>>(dados));
    }
}

namespace RetroGames.WebApi.ApiModels;

/// <summary>Parâmetros da listagem de jogos, lidos da query string (?busca=zelda&amp;idPlataforma=2&amp;pagina=1).</summary>
public class ListarJogoRequestApiModel
{
    public string? Busca { get; set; }
    public int? IdPlataforma { get; set; }
    public int? IdGenero { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 12;
}

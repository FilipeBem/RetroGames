namespace RetroGames.Services;

/// <summary>Filtro e paginação da listagem de jogos.</summary>
public class FiltroJogoServiceModel
{
    public string? Busca { get; set; }
    public int? IdPlataforma { get; set; }
    public int? IdGenero { get; set; }

    /// <summary>Pedido para incluir jogos inativos. Só vale para o administrador (RN04).</summary>
    public bool IncluirInativos { get; set; }

    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 12;
}

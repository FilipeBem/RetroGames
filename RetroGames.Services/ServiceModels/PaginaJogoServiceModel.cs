namespace RetroGames.Services;

/// <summary>Uma página de jogos com o total encontrado (antes da paginação).</summary>
public class PaginaJogoServiceModel
{
    public IReadOnlyList<JogoDetalheServiceModel> Itens { get; set; } = new List<JogoDetalheServiceModel>();

    /// <summary>Página efetivamente usada (já corrigida, se o filtro veio fora da faixa).</summary>
    public int Pagina { get; set; }

    /// <summary>Tamanho de página efetivamente usado.</summary>
    public int TamanhoPagina { get; set; }

    /// <summary>Total de jogos encontrados com o filtro.</summary>
    public int Total { get; set; }
}

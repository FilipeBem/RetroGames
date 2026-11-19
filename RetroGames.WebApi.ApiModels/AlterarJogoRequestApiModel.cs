namespace RetroGames.WebApi.ApiModels;

/// <summary>Entrada da alteração de jogo (PUT: recurso completo). O id vem da rota. Ativo permite reativar o jogo.</summary>
public class AlterarJogoRequestApiModel
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int AnoLancamento { get; set; }
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
    public int IdPlataforma { get; set; }
    public int IdCondicaoJogo { get; set; }

    /// <summary>Ids dos gêneros do jogo (ao menos um).</summary>
    public List<int> IdsGeneros { get; set; } = new();

    public bool Ativo { get; set; }
}

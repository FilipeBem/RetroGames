using RetroGames.Model.Model;

namespace RetroGames.Services;

/// <summary>Um jogo combinado com os dados de outras tabelas (montado com LINQ no JogoService).</summary>
public class JogoDetalheServiceModel
{
    /// <summary>O jogo, como vem do Repository.</summary>
    public Jogo Jogo { get; set; } = new();

    /// <summary>Nome da plataforma (tb_plataforma).</summary>
    public string NomePlataforma { get; set; } = string.Empty;

    /// <summary>Descrição da condição (tb_condicao_jogo).</summary>
    public string DescricaoCondicao { get; set; } = string.Empty;

    /// <summary>Gêneros do jogo (tb_genero via tb_jogo_genero), ordenados por nome.</summary>
    public IReadOnlyList<Genero> Generos { get; set; } = new List<Genero>();
}

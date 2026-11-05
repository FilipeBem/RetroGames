namespace RetroGames.Model.Model;

/// <summary>Tabela tb_jogo_genero.</summary>
public class JogoGenero
{
    /// <summary>Coluna co_jogo (PK composta, FK para tb_jogo).</summary>
    public int IdJogo { get; set; }

    /// <summary>Coluna co_genero (PK composta, FK para tb_genero).</summary>
    public int IdGenero { get; set; }
}

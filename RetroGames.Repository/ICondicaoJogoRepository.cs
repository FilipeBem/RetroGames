using RetroGames.Model.Model;

namespace RetroGames.Repository;

/// <summary>Leitura da tabela tb_condicao_jogo.</summary>
public interface ICondicaoJogoRepository
{
    /// <summary>Todas as condições de jogo, ordenadas pela chave.</summary>
    Task<IReadOnlyList<CondicaoJogo>> ListarAsync();
}

using RetroGames.Model.Model;

namespace RetroGames.Repository;

/// <summary>Todo o SQL da tabela tb_jogo.</summary>
public interface IJogoRepository
{
    /// <summary>
    /// Jogos de uma página e o total encontrado, com o mesmo filtro. Cada filtro só vale quando informado:
    /// parte do nome, plataforma e lista de ids (lista vazia não encontra nenhum jogo).
    /// Inativos só entram com <paramref name="incluirInativos"/>. Ordenação por nome.
    /// <paramref name="pagina"/> começa em 1; o Service garante valores válidos.
    /// </summary>
    Task<(IReadOnlyList<Jogo> Itens, int Total)> ListarAsync(
        string? busca,
        int? idPlataforma,
        IReadOnlyCollection<int>? idsJogos,
        bool incluirInativos,
        int pagina,
        int tamanhoPagina);

    /// <summary>O jogo com o id informado, ou nulo se não existir.</summary>
    Task<Jogo?> BuscarPorIdAsync(int id);

    /// <summary>Grava um novo jogo e devolve o id gerado pelo banco.</summary>
    Task<int> InserirAsync(Jogo jogo);

    /// <summary>Atualiza nome, descrição, ano, preço, estoque, plataforma e condição.</summary>
    Task AlterarAsync(Jogo jogo);

    /// <summary>Marca o jogo como inativo (st_ativo = 0). Jogo nunca é apagado.</summary>
    Task DesativarAsync(int id);
}

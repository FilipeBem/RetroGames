using RetroGames.Model.Model;

namespace RetroGames.Services;

/// <summary>Regras de negócio de jogos (RN03, RN05).</summary>
public interface IJogoService
{
    Task<PaginaJogoServiceModel> ListarAsync(FiltroJogoServiceModel filtro);

    /// <exception cref="BusinessRuleException">JogoNaoEncontrado.</exception>
    Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id);

    /// <exception cref="BusinessRuleException">PlataformaNaoEncontrada, CondicaoNaoEncontrada ou GeneroNaoEncontrado.</exception>
    Task<JogoDetalheServiceModel> InserirAsync(Jogo jogo, IReadOnlyCollection<int> idsGeneros);

    /// <exception cref="BusinessRuleException">JogoNaoEncontrado, PlataformaNaoEncontrada, CondicaoNaoEncontrada ou GeneroNaoEncontrado.</exception>
    Task<JogoDetalheServiceModel> AlterarAsync(Jogo jogo, IReadOnlyCollection<int> idsGeneros);

    /// <exception cref="BusinessRuleException">JogoNaoEncontrado.</exception>
    Task DesativarAsync(int id);
}

using RetroGames.Model.Model;

namespace RetroGames.Services;

/// <summary>Regras de negócio de plataformas (RN01 e RN02).</summary>
public interface IPlataformaService
{
    Task<IReadOnlyList<Plataforma>> ListarAsync();

    /// <exception cref="BusinessRuleException">PlataformaNaoEncontrada.</exception>
    Task<Plataforma> BuscarPorIdAsync(int id);

    /// <exception cref="BusinessRuleException">PlataformaNomeDuplicado.</exception>
    Task<Plataforma> InserirAsync(Plataforma plataforma);

    /// <exception cref="BusinessRuleException">PlataformaNaoEncontrada ou PlataformaNomeDuplicado.</exception>
    Task<Plataforma> AlterarAsync(Plataforma plataforma);

    /// <exception cref="BusinessRuleException">PlataformaNaoEncontrada ou PlataformaPossuiJogos.</exception>
    Task ExcluirAsync(int id);
}

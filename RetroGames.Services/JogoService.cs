using System.Transactions;
using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class JogoService : IJogoService
{
    private const int TamanhoPaginaPadrao = 12;
    private const int TamanhoPaginaMaximo = 100;

    private readonly IRepositorySet _repositorio;
    private readonly IUsuarioAtualService _usuarioAtual;

    public JogoService(IRepositorySet repositorio, IUsuarioAtualService usuarioAtual)
    {
        _repositorio = repositorio;
        _usuarioAtual = usuarioAtual;
    }

    public async Task<PaginaJogoServiceModel> ListarAsync(FiltroJogoServiceModel filtro)
    {
        // O Repository espera página >= 1 e tamanho > 0: o Service corrige valores fora da faixa.
        var pagina = Math.Max(filtro.Pagina, 1);
        var tamanhoPagina = filtro.TamanhoPagina < 1
            ? TamanhoPaginaPadrao
            : Math.Min(filtro.TamanhoPagina, TamanhoPaginaMaximo);

        // Filtro por gênero: resolvido aqui, com LINQ. O resultado segue para o SQL como lista de ids,
        // o que mantém a contagem e a paginação no banco.
        IReadOnlyCollection<int>? idsJogos = null;
        if (filtro.IdGenero.HasValue)
        {
            var vinculos = await _repositorio.JogoGenero.ListarTodosAsync();
            idsJogos = vinculos
                .Where(v => v.IdGenero == filtro.IdGenero.Value)
                .Select(v => v.IdJogo)
                .Distinct()
                .ToList();
        }

        // RN04: jogos inativos só aparecem para o administrador, mesmo que o visitante peça incluirInativos.
        var incluirInativos = filtro.IncluirInativos && _usuarioAtual.EhAdministrador;

        var (jogos, total) = await _repositorio.Jogo.ListarAsync(
            filtro.Busca, filtro.IdPlataforma, idsJogos, incluirInativos, pagina, tamanhoPagina);

        return new PaginaJogoServiceModel
        {
            Itens = await MontarDetalhesAsync(jogos),
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina,
            Total = total,
        };
    }

    public async Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id)
    {
        var jogo = await _repositorio.Jogo.BuscarPorIdAsync(id);

        // RN04: para quem não é administrador, jogo inativo é tratado como inexistente (404).
        if (jogo is null || (!jogo.Ativo && !_usuarioAtual.EhAdministrador))
        {
            throw new BusinessRuleException(BusinessErrors.JogoNaoEncontrado);
        }

        var detalhes = await MontarDetalhesAsync(new List<Jogo> { jogo });
        return detalhes[0];
    }

    public async Task<JogoDetalheServiceModel> InserirAsync(Jogo jogo, IReadOnlyCollection<int> idsGeneros)
    {
        var generos = idsGeneros.Distinct().ToList();
        await ValidarReferenciasAsync(jogo, generos);

        jogo.Nome = jogo.Nome.Trim();
        jogo.Ativo = true;
        jogo.DataCadastro = DateTime.UtcNow;

        int idJogo;
        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout,
        };
        using (var transactionScope = new TransactionScope(TransactionScopeOption.Required,
                   transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
        {
            idJogo = await _repositorio.Jogo.InserirAsync(jogo);
            await _repositorio.JogoGenero.InserirAsync(idJogo, generos);
            transactionScope.Complete();
        }

        return await BuscarPorIdAsync(idJogo);
    }

    public async Task<JogoDetalheServiceModel> AlterarAsync(Jogo jogo, IReadOnlyCollection<int> idsGeneros)
    {
        if (await _repositorio.Jogo.BuscarPorIdAsync(jogo.Id) is null)
        {
            throw new BusinessRuleException(BusinessErrors.JogoNaoEncontrado);
        }

        var generos = idsGeneros.Distinct().ToList();
        await ValidarReferenciasAsync(jogo, generos);

        jogo.Nome = jogo.Nome.Trim();

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout,
        };
        using (var transactionScope = new TransactionScope(TransactionScopeOption.Required,
                   transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
        {
            await _repositorio.Jogo.AlterarAsync(jogo);
            await _repositorio.JogoGenero.RemoverPorJogoAsync(jogo.Id);
            await _repositorio.JogoGenero.InserirAsync(jogo.Id, generos);
            transactionScope.Complete();
        }

        return await BuscarPorIdAsync(jogo.Id);
    }

    public async Task DesativarAsync(int id)
    {
        if (await _repositorio.Jogo.BuscarPorIdAsync(id) is null)
        {
            throw new BusinessRuleException(BusinessErrors.JogoNaoEncontrado);
        }

        // RN03: exclusão lógica. O jogo nunca é apagado.
        await _repositorio.Jogo.DesativarAsync(id);
    }

    /// <summary>RN05: plataforma, condição e ao menos um gênero precisam existir.</summary>
    private async Task ValidarReferenciasAsync(Jogo jogo, IReadOnlyCollection<int> idsGeneros)
    {
        if (await _repositorio.Plataforma.BuscarPorIdAsync(jogo.IdPlataforma) is null)
        {
            throw new BusinessRuleException(BusinessErrors.PlataformaNaoEncontrada);
        }

        var condicoes = await _repositorio.CondicaoJogo.ListarAsync();
        if (!condicoes.Any(c => c.Id == jogo.IdCondicaoJogo))
        {
            throw new BusinessRuleException(BusinessErrors.CondicaoNaoEncontrada);
        }

        var idsExistentes = (await _repositorio.Genero.ListarAsync()).Select(g => g.Id).ToHashSet();
        if (idsGeneros.Count == 0 || idsGeneros.Any(id => !idsExistentes.Contains(id)))
        {
            throw new BusinessRuleException(BusinessErrors.GeneroNaoEncontrado);
        }
    }

    /// <summary>
    /// Combina, com LINQ, jogos + plataformas + condições + gêneros + vínculos.
    /// Sempre 4 consultas (se houver jogos), qualquer que seja o tamanho da lista: nunca uma por jogo.
    /// </summary>
    private async Task<IReadOnlyList<JogoDetalheServiceModel>> MontarDetalhesAsync(IReadOnlyList<Jogo> jogos)
    {
        if (jogos.Count == 0)
        {
            return new List<JogoDetalheServiceModel>();
        }

        var plataformas = (await _repositorio.Plataforma.ListarAsync()).ToDictionary(p => p.Id);
        var condicoes = (await _repositorio.CondicaoJogo.ListarAsync()).ToDictionary(c => c.Id);
        var generos = (await _repositorio.Genero.ListarAsync()).ToDictionary(g => g.Id);
        var vinculos = await _repositorio.JogoGenero.ListarPorJogosAsync(jogos.Select(j => j.Id).ToList());

        var generosPorJogo = vinculos
            .GroupBy(v => v.IdJogo)
            .ToDictionary(
                grupo => grupo.Key,
                grupo => (IReadOnlyList<Genero>)grupo
                    .Select(v => generos[v.IdGenero])
                    .OrderBy(g => g.Nome)
                    .ToList());

        return jogos
            .Select(jogo => new JogoDetalheServiceModel
            {
                Jogo = jogo,
                NomePlataforma = plataformas[jogo.IdPlataforma].Nome,
                DescricaoCondicao = condicoes[jogo.IdCondicaoJogo].Descricao,
                Generos = generosPorJogo.TryGetValue(jogo.Id, out var lista) ? lista : new List<Genero>(),
            })
            .ToList();
    }
}

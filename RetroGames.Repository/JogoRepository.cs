using MySqlConnector;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class JogoRepository : RepositoryBase, IJogoRepository
{
    private const string Colunas =
        "co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, " +
        "dt_cadastro, co_plataforma, co_condicao_jogo";

    public JogoRepository(RepositoryConfig config) : base(config)
    {
    }

    public async Task<(IReadOnlyList<Jogo> Itens, int Total)> ListarAsync(
        string? busca,
        int? idPlataforma,
        IReadOnlyCollection<int>? idsJogos,
        bool incluirInativos,
        int pagina,
        int tamanhoPagina)
    {
        // Só trechos fixos de SQL são montados aqui; todo valor entra como parâmetro.
        var filtros = new List<string>();
        var parametros = new List<(string Nome, object? Valor)>();

        if (!incluirInativos)
        {
            filtros.Add("st_ativo = TRUE");
        }

        if (!string.IsNullOrWhiteSpace(busca))
        {
            filtros.Add("no_jogo LIKE @busca");
            parametros.Add(("@busca", "%" + EscaparLike(busca.Trim()) + "%"));
        }

        if (idPlataforma.HasValue)
        {
            filtros.Add("co_plataforma = @idPlataforma");
            parametros.Add(("@idPlataforma", idPlataforma.Value));
        }

        if (idsJogos is not null)
        {
            if (idsJogos.Count == 0)
            {
                filtros.Add("1 = 0");
            }
            else
            {
                var nomes = new List<string>();
                var indice = 0;
                foreach (var id in idsJogos)
                {
                    var nome = $"@idJogo{indice++}";
                    nomes.Add(nome);
                    parametros.Add((nome, id));
                }

                filtros.Add($"co_seq_jogo IN ({string.Join(", ", nomes)})");
            }
        }

        var where = filtros.Count > 0 ? " WHERE " + string.Join(" AND ", filtros) : string.Empty;
        var sqlTotal = "SELECT COUNT(*) FROM tb_jogo" + where;
        var sqlPagina = $"SELECT {Colunas} FROM tb_jogo{where} " +
                        "ORDER BY no_jogo, co_seq_jogo LIMIT @limite OFFSET @deslocamento";

        await using var conexao = await AbrirConexaoAsync();

        int total;
        await using (var comandoTotal = CriarComando(conexao, sqlTotal))
        {
            foreach (var (nome, valor) in parametros)
            {
                AdicionarParametro(comandoTotal, nome, valor);
            }

            total = Convert.ToInt32(await comandoTotal.ExecuteScalarAsync());
        }

        var jogos = new List<Jogo>();
        await using (var comandoPagina = CriarComando(conexao, sqlPagina))
        {
            foreach (var (nome, valor) in parametros)
            {
                AdicionarParametro(comandoPagina, nome, valor);
            }

            AdicionarParametro(comandoPagina, "@limite", tamanhoPagina);
            AdicionarParametro(comandoPagina, "@deslocamento", (pagina - 1) * tamanhoPagina);

            await using var leitor = (MySqlDataReader)await comandoPagina.ExecuteReaderAsync();
            while (await leitor.ReadAsync())
            {
                jogos.Add(Mapear(leitor));
            }
        }

        return (jogos, total);
    }

    public async Task<Jogo?> BuscarPorIdAsync(int id)
    {
        var sql = $"SELECT {Colunas} FROM tb_jogo WHERE co_seq_jogo = @id";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", id);
        await using var leitor = (MySqlDataReader)await comando.ExecuteReaderAsync();

        return await leitor.ReadAsync() ? Mapear(leitor) : null;
    }

    public async Task<int> InserirAsync(Jogo jogo)
    {
        const string sql = @"INSERT INTO tb_jogo
                                 (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo,
                                  dt_cadastro, co_plataforma, co_condicao_jogo)
                             VALUES
                                 (@nome, @descricao, @anoLancamento, @preco, @estoque, @ativo,
                                  @dataCadastro, @idPlataforma, @idCondicaoJogo)";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@nome", jogo.Nome);
        AdicionarParametro(comando, "@descricao", jogo.Descricao);
        AdicionarParametro(comando, "@anoLancamento", jogo.AnoLancamento);
        AdicionarParametro(comando, "@preco", jogo.Preco);
        AdicionarParametro(comando, "@estoque", jogo.Estoque);
        AdicionarParametro(comando, "@ativo", jogo.Ativo);
        AdicionarParametro(comando, "@dataCadastro", jogo.DataCadastro);
        AdicionarParametro(comando, "@idPlataforma", jogo.IdPlataforma);
        AdicionarParametro(comando, "@idCondicaoJogo", jogo.IdCondicaoJogo);
        await ExecutarGravacaoAsync(comando);

        return (int)comando.LastInsertedId;
    }

    public async Task AlterarAsync(Jogo jogo)
    {
        const string sql = @"UPDATE tb_jogo
                                SET no_jogo = @nome,
                                    ds_jogo = @descricao,
                                    nu_ano_lancamento = @anoLancamento,
                                    vl_preco = @preco,
                                    qt_estoque = @estoque,
                                    co_plataforma = @idPlataforma,
                                    co_condicao_jogo = @idCondicaoJogo
                              WHERE co_seq_jogo = @id";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", jogo.Id);
        AdicionarParametro(comando, "@nome", jogo.Nome);
        AdicionarParametro(comando, "@descricao", jogo.Descricao);
        AdicionarParametro(comando, "@anoLancamento", jogo.AnoLancamento);
        AdicionarParametro(comando, "@preco", jogo.Preco);
        AdicionarParametro(comando, "@estoque", jogo.Estoque);
        AdicionarParametro(comando, "@idPlataforma", jogo.IdPlataforma);
        AdicionarParametro(comando, "@idCondicaoJogo", jogo.IdCondicaoJogo);
        await ExecutarGravacaoAsync(comando);
    }

    public async Task DesativarAsync(int id)
    {
        const string sql = "UPDATE tb_jogo SET st_ativo = FALSE WHERE co_seq_jogo = @id";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", id);
        await ExecutarGravacaoAsync(comando);
    }

    /// <summary>Escapa \, % e _ para que o texto digitado seja buscado literalmente no LIKE.</summary>
    private static string EscaparLike(string texto) =>
        texto.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");

    private static Jogo Mapear(MySqlDataReader leitor) => new()
    {
        Id = leitor.GetInt32("co_seq_jogo"),
        Nome = leitor.GetString("no_jogo"),
        Descricao = leitor.IsDBNull("ds_jogo") ? null : leitor.GetString("ds_jogo"),
        AnoLancamento = leitor.GetInt32("nu_ano_lancamento"),
        Preco = leitor.GetDecimal("vl_preco"),
        Estoque = leitor.GetInt32("qt_estoque"),
        Ativo = leitor.GetBoolean("st_ativo"),
        DataCadastro = leitor.GetDateTime("dt_cadastro"),
        IdPlataforma = leitor.GetInt32("co_plataforma"),
        IdCondicaoJogo = leitor.GetInt32("co_condicao_jogo"),
    };
}

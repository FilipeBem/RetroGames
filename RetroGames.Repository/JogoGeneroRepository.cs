using MySqlConnector;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class JogoGeneroRepository : RepositoryBase, IJogoGeneroRepository
{
    public JogoGeneroRepository(RepositoryConfig config) : base(config)
    {
    }

    public async Task<IReadOnlyList<JogoGenero>> ListarTodosAsync()
    {
        const string sql = "SELECT co_jogo, co_genero FROM tb_jogo_genero ORDER BY co_jogo, co_genero";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        return await LerVinculosAsync(comando);
    }

    public async Task<IReadOnlyList<JogoGenero>> ListarPorJogosAsync(IReadOnlyCollection<int> idsJogos)
    {
        if (idsJogos.Count == 0)
        {
            return new List<JogoGenero>();
        }

        var nomes = idsJogos.Select((_, indice) => $"@idJogo{indice}").ToList();
        var sql = "SELECT co_jogo, co_genero FROM tb_jogo_genero " +
                  $"WHERE co_jogo IN ({string.Join(", ", nomes)}) ORDER BY co_jogo, co_genero";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        var posicao = 0;
        foreach (var id in idsJogos)
        {
            AdicionarParametro(comando, nomes[posicao++], id);
        }

        return await LerVinculosAsync(comando);
    }

    public async Task InserirAsync(int idJogo, IReadOnlyCollection<int> idsGeneros)
    {
        if (idsGeneros.Count == 0)
        {
            return;
        }

        var linhas = idsGeneros.Select((_, indice) => $"(@idJogo, @idGenero{indice})").ToList();
        var sql = $"INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES {string.Join(", ", linhas)}";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@idJogo", idJogo);

        var posicao = 0;
        foreach (var idGenero in idsGeneros)
        {
            AdicionarParametro(comando, $"@idGenero{posicao++}", idGenero);
        }

        await ExecutarGravacaoAsync(comando);
    }

    public async Task RemoverPorJogoAsync(int idJogo)
    {
        const string sql = "DELETE FROM tb_jogo_genero WHERE co_jogo = @idJogo";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@idJogo", idJogo);
        await ExecutarGravacaoAsync(comando);
    }

    private static async Task<IReadOnlyList<JogoGenero>> LerVinculosAsync(MySqlCommand comando)
    {
        await using var leitor = (MySqlDataReader)await comando.ExecuteReaderAsync();

        var vinculos = new List<JogoGenero>();
        while (await leitor.ReadAsync())
        {
            vinculos.Add(new JogoGenero
            {
                IdJogo = leitor.GetInt32("co_jogo"),
                IdGenero = leitor.GetInt32("co_genero"),
            });
        }

        return vinculos;
    }
}

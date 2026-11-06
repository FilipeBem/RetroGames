using MySqlConnector;

namespace RetroGames.Repository;

/// <summary>
/// Base de todos os repositórios: abre conexão, cria comando, adiciona parâmetro e executa gravação.
/// Não contém SQL de nenhuma tabela e não abre transações (isso é do Service).
/// </summary>
public abstract class RepositoryBase
{
    private readonly RepositoryConfig _config;

    protected RepositoryBase(RepositoryConfig config)
    {
        _config = config;
    }

    /// <summary>Abre e devolve uma conexão. Quem chama deve descartá-la com <c>await using</c>.</summary>
    protected async Task<MySqlConnection> AbrirConexaoAsync()
    {
        var conexao = new MySqlConnection(_config.ConnectionString);
        try
        {
            await conexao.OpenAsync();
        }
        catch
        {
            await conexao.DisposeAsync();
            throw;
        }

        return conexao;
    }

    /// <summary>Cria um comando com o texto SQL. Quem chama deve descartá-lo com <c>await using</c>.</summary>
    protected static MySqlCommand CriarComando(MySqlConnection conexao, string sql)
    {
        var comando = conexao.CreateCommand();
        comando.CommandText = sql;
        return comando;
    }

    /// <summary>Adiciona o parâmetro (@nome e valor). Nulo vira NULL no banco.</summary>
    protected static void AdicionarParametro(MySqlCommand comando, string nome, object? valor)
    {
        comando.Parameters.AddWithValue(nome, valor ?? DBNull.Value);
    }

    /// <summary>
    /// Executa um INSERT, UPDATE ou DELETE e devolve as linhas afetadas.
    /// Único lugar onde o erro 1062 (UNIQUE) é convertido em <see cref="RegistroDuplicadoException"/>.
    /// </summary>
    protected static async Task<int> ExecutarGravacaoAsync(MySqlCommand comando)
    {
        try
        {
            return await comando.ExecuteNonQueryAsync();
        }
        catch (MySqlException ex) when (ex.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
        {
            throw new RegistroDuplicadoException(ex);
        }
    }
}

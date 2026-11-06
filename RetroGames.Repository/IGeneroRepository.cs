using RetroGames.Model.Model;

namespace RetroGames.Repository;

/// <summary>Leitura da tabela tb_genero.</summary>
public interface IGeneroRepository
{
    /// <summary>Todos os gêneros, ordenados por nome.</summary>
    Task<IReadOnlyList<Genero>> ListarAsync();
}

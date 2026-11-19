using RetroGames.Model.Model;

namespace RetroGames.Repository;

/// <summary>Todo o SQL da tabela tb_usuario.</summary>
public interface IUsuarioRepository
{
    /// <summary>O usuário com o e-mail informado, ou nulo se não existir.</summary>
    Task<Usuario?> BuscarPorEmailAsync(string email);

    /// <summary>O usuário com o id informado, ou nulo se não existir.</summary>
    Task<Usuario?> BuscarPorIdAsync(int id);

    /// <summary>Se existe algum usuário com o perfil Administrador.</summary>
    Task<bool> ExisteAdministradorAsync();

    /// <summary>Grava um novo usuário e devolve o id gerado pelo banco.</summary>
    Task<int> InserirAsync(Usuario usuario);
}

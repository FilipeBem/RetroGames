namespace RetroGames.Repository;

/// <summary>Configuração do Repository: apenas a connection string.</summary>
public class RepositoryConfig
{
    /// <summary>Texto que diz onde está o banco e com qual usuário entrar.</summary>
    public string ConnectionString { get; set; } = string.Empty;
}

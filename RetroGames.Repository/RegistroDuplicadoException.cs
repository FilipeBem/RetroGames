namespace RetroGames.Repository;

/// <summary>
/// Lançada pela RepositoryBase quando o MySQL recusa um valor repetido em coluna UNIQUE (erro 1062).
/// O Service converte esta exceção no erro de negócio correspondente.
/// </summary>
public class RegistroDuplicadoException : Exception
{
    public RegistroDuplicadoException(Exception? excecaoInterna = null)
        : base("Já existe um registro com este valor.", excecaoInterna)
    {
    }
}

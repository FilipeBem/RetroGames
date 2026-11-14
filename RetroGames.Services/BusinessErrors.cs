using System.Net;

namespace RetroGames.Services;

/// <summary>Catálogo dos erros de negócio. Cada erro é definido uma única vez, aqui.</summary>
public static class BusinessErrors
{
    /// <summary>Plataforma pedida ou do jogo não existe (RN05).</summary>
    public static readonly BusinessError PlataformaNaoEncontrada =
        new(100, "Plataforma não encontrada.", HttpStatusCode.NotFound);

    /// <summary>Nome de plataforma já usado (RN01).</summary>
    public static readonly BusinessError PlataformaNomeDuplicado =
        new(101, "Já existe uma plataforma com este nome.", HttpStatusCode.Conflict);

    /// <summary>Plataforma com jogos não pode ser excluída (RN02).</summary>
    public static readonly BusinessError PlataformaPossuiJogos =
        new(102, "Não é possível excluir: existem jogos desta plataforma.", HttpStatusCode.Conflict);

    /// <summary>O id do jogo não existe.</summary>
    public static readonly BusinessError JogoNaoEncontrado =
        new(103, "Jogo não encontrado.", HttpStatusCode.NotFound);

    /// <summary>Condição do jogo não existe (RN05).</summary>
    public static readonly BusinessError CondicaoNaoEncontrada =
        new(104, "A condição informada não existe.", HttpStatusCode.NotFound);

    /// <summary>Gênero do jogo não existe (RN05).</summary>
    public static readonly BusinessError GeneroNaoEncontrado =
        new(105, "Um ou mais gêneros informados não existem.", HttpStatusCode.NotFound);
}

namespace RetroGames.WebApi.ApiModels;

/// <summary>
/// Códigos da própria API (0 a 99), sequenciais. Os erros de negócio (a partir de 100) ficam em BusinessErrors.
/// Código novo recebe o próximo número livre.
/// </summary>
public static class ResponseCodes
{
    public const int SUCESSO = 0;

    /// <summary>Exceção não tratada; a mensagem genérica é definida no GlobalExceptionHandler.</summary>
    public const int ERRO = 1;

    public static readonly (int Code, string DefaultMessage) MODEL_NULL = (2, "O modelo veio nulo.");

    public static readonly (int Code, string DefaultMessage) MODEL_STATE_INVALIDO = (3, "Há um erro com o modelo enviado.");

    public static readonly (int Code, string DefaultMessage) NAO_AUTENTICADO = (4, "Faça login para continuar.");

    public static readonly (int Code, string DefaultMessage) ACESSO_NEGADO = (5, "Você não tem permissão para esta operação.");

    public static readonly (int Code, string DefaultMessage) CREDENCIAIS_INVALIDAS = (6, "E-mail ou senha inválidos.");
}

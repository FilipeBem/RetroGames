namespace RetroGames.Services;

/// <summary>Lançada pelo Service quando uma regra de negócio é violada.</summary>
public class BusinessRuleException : Exception
{
    public BusinessRuleException(BusinessError erro) : base(erro.Message)
    {
        Erro = erro;
    }

    /// <summary>O erro do catálogo (código, mensagem e status HTTP).</summary>
    public BusinessError Erro { get; }
}

namespace RetroGames.WebApi.ApiModels;

/// <summary>Envelope de resposta sem dados (usado nos erros: a propriedade data é omitida).</summary>
public class RetroGamesResponseApiModel
{
    public int ResponseCode { get; set; }

    public string? ErrorMessage { get; set; }
}

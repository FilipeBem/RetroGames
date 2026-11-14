namespace RetroGames.WebApi.ApiModels;

/// <summary>Envelope de resposta com dados: { "responseCode", "data", "errorMessage" }.</summary>
public class RetroGamesResponseApiModel<T> : RetroGamesResponseApiModel
{
    public RetroGamesResponseApiModel()
    {
        ResponseCode = ResponseCodes.SUCESSO;
    }

    public RetroGamesResponseApiModel(T data) : this()
    {
        Data = data;
    }

    public T? Data { get; set; }
}

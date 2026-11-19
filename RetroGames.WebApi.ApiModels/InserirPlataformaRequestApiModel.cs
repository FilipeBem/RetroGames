namespace RetroGames.WebApi.ApiModels;

/// <summary>Entrada da inclusão de plataforma: só o que o cliente pode enviar (sem Id).</summary>
public class InserirPlataformaRequestApiModel
{
    public string Nome { get; set; } = string.Empty;
    public string Fabricante { get; set; } = string.Empty;
    public int AnoLancamento { get; set; }
}

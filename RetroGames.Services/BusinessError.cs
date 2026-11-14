using System.Net;

namespace RetroGames.Services;

/// <summary>Um erro de negócio do catálogo: código, mensagem e status HTTP com que a API responde.</summary>
/// <param name="Code">Código do erro (a partir de 100).</param>
/// <param name="Message">Mensagem exibida ao usuário.</param>
/// <param name="Status">Status HTTP (apenas um valor; o Service não conhece HttpContext nem controller).</param>
public sealed record BusinessError(int Code, string Message, HttpStatusCode Status);

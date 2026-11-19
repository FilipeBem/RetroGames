using Microsoft.AspNetCore.Identity;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class AutenticacaoService : IAutenticacaoService
{
    private readonly IRepositorySet _repositorio;
    private readonly IPasswordHasher<Usuario> _hasher;

    public AutenticacaoService(IRepositorySet repositorio, IPasswordHasher<Usuario> hasher)
    {
        _repositorio = repositorio;
        _hasher = hasher;
    }

    public async Task<Usuario?> AutenticarAsync(string email, string senha)
    {
        var usuario = await _repositorio.Usuario.BuscarPorEmailAsync(email.Trim());
        if (usuario is null || !usuario.Ativo)
        {
            return null;
        }

        // A senha nunca é comparada em texto: o hasher gera o hash da senha digitada e compara com o guardado.
        var resultado = _hasher.VerifyHashedPassword(usuario, usuario.SenhaHash, senha);
        return resultado == PasswordVerificationResult.Failed ? null : usuario;
    }

    public async Task GarantirAdministradorInicialAsync(string? email, string? senha)
    {
        if (await _repositorio.Usuario.ExisteAdministradorAsync())
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
        {
            throw new InvalidOperationException(
                "Não existe administrador e o administrador inicial não está configurado. " +
                "Defina AdminInicial__Email e AdminInicial__Senha no arquivo .env.");
        }

        var administrador = new Usuario
        {
            Nome = "Administrador",
            Email = email.Trim(),
            Ativo = true,
            DataCadastro = DateTime.UtcNow,
            IdPerfil = PerfilIds.Administrador,
        };
        administrador.SenhaHash = _hasher.HashPassword(administrador, senha);

        await _repositorio.Usuario.InserirAsync(administrador);
    }
}

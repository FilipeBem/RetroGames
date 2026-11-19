using FluentValidation;
using RetroGames.Services;
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Filters;
using RetroGames.WebApi.Middlewares;

// O .env fica na raiz do repositório: TraversePath() sobe as pastas até achá-lo.
// Precisa rodar ANTES do CreateBuilder, para as variáveis entrarem na configuração.
DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

// ConnectionStrings__Default no .env vira ConnectionStrings:Default na configuração.
var connectionString = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Connection string não encontrada. Crie o arquivo .env na raiz do repositório " +
        "com a linha ConnectionStrings__Default=\"server=localhost;...\" (veja o .env.example).");
}

builder.Services.AddRepositorios(connectionString);
builder.Services.AddServicosDeNegocio();

builder.Services.AddAutenticacaoConfig();
builder.Services.AddAutorizacaoConfig();

builder.Services.AddControllers(opcoes => opcoes.Filters.Add<FluentValidationFilter>())
    .ConfigureApiBehaviorOptions(opcoes => opcoes.InvalidModelStateResponseFactory = ModelStateConfig.CriarResposta);
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddOpenApi();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Administrador inicial: criado só se ainda não houver nenhum. E-mail e senha vêm do .env (nunca do código).
using (var escopo = app.Services.CreateScope())
{
    var autenticacao = escopo.ServiceProvider.GetRequiredService<IAutenticacaoService>();
    await autenticacao.GarantirAdministradorInicialAsync(
        builder.Configuration["AdminInicial:Email"],
        builder.Configuration["AdminInicial:Senha"]);
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    // Sem AllowAnonymous, a política padrão bloquearia o JSON do OpenAPI e o Swagger mostraria "Failed to load API definition".
    app.MapOpenApi().AllowAnonymous();
    app.UseSwaggerUI(opcoes => opcoes.SwaggerEndpoint("/openapi/v1.json", "RetroGames API v1"));
}

// A ordem importa: primeiro quem é o usuário, depois o que ele pode fazer.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

using RetroGames.WebApi.Configuration;
// ETAPA 3: descomente a linha abaixo.
// using RetroGames.WebApi.Middlewares;

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

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// ETAPA 3: descomente as duas linhas abaixo.
// builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
// builder.Services.AddProblemDetails();

var app = builder.Build();

// ETAPA 3: descomente a linha abaixo. Deve ser a PRIMEIRA etapa do pipeline.
// app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(opcoes => opcoes.SwaggerEndpoint("/openapi/v1.json", "RetroGames API v1"));
}

app.MapControllers();

app.Run();

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Backend.Hubs;
using Backend;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options => options.AddDefaultPolicy(o => o.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddSignalR();
builder.Services.AddSingleton<GameManager>(new GameManager());

var app = builder.Build();
app.UseCors();

app.MapGet("/controller/echo", (string message) => message);
app.MapGet("/game/create", (GameManager manager) => {
    Game game = new();
    manager.Games.Add(game);

    return Results.Ok(game.Id);
});

app.MapHub<EchoHub>("/hub/echo");
app.MapHub<GameHub>("/hub/game");

app.Run();
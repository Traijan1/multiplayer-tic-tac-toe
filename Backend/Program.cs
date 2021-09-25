using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Backend.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options => options.AddDefaultPolicy(o => o.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddSignalR();

var app = builder.Build();
app.UseCors();

app.MapGet("/controller/echo", (string message) => message);
app.MapHub<EchoHub>("/hub/echo");

app.Run();
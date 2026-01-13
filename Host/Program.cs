using Pokeguesser.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
var app = builder.Build();

app.MapHub<ConnectionHub>("/connectionHub");

app.Run();

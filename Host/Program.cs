using ShadowDex.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy
            .WithOrigins("http://127.0.0.1:5500")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddSignalR();
var app = builder.Build();

app.UseCors("AllowLocalhost");
app.MapHub<GameHub>("/GameHub");

app.Run();

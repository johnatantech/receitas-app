using Receitas.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();

builder.Services.AddHttpClient<IMealDbClient, MealDbClient>(c =>
{
    c.BaseAddress = new Uri("https://www.themealdb.com/api/json/v1/1/");
});

builder.Services.AddHttpClient<ITradutor, MyMemoryTradutor>(c =>
{
    c.BaseAddress = new Uri("https://api.mymemory.translated.net/");
    c.Timeout = TimeSpan.FromSeconds(20);
});

// Origens permitidas: "Cors:Origins" (separadas por vírgula, sem barra no final)
var origens = (builder.Configuration["Cors:Origins"] ?? "http://localhost:5173")
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

builder.Services.AddCors(o => o.AddPolicy("front", p =>
    p.WithOrigins(origens)
     .AllowAnyHeader()
     .AllowAnyMethod()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("front");
app.UseAuthorization();

app.MapGet("/", () => Results.Ok(new { servico = "Receitas API", status = "ok" }));
app.MapGet("/health", () => Results.Ok("ok"));
app.MapControllers();

app.Run();
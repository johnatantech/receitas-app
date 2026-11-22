using System.Text.Json;
using Receitas.Api.Models;

namespace Receitas.Api.Services;

public class MealDbClient : IMealDbClient
{
    private readonly HttpClient _http;

    public MealDbClient(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<ReceitaResumo>> BuscarPorIngredienteAsync(
        string ingrediente,
        CancellationToken ct = default)
    {
        var url = $"filter.php?i={Uri.EscapeDataString(ingrediente)}";
        using var resposta = await _http.GetAsync(url, ct);
        resposta.EnsureSuccessStatusCode();

        using var doc = await JsonDocument.ParseAsync(
            await resposta.Content.ReadAsStreamAsync(ct), cancellationToken: ct);

        // A API devolve {"meals": null} quando não encontra nada
        if (!doc.RootElement.TryGetProperty("meals", out var meals)
            || meals.ValueKind != JsonValueKind.Array)
            return [];

        return meals.EnumerateArray()
            .Select(m => new ReceitaResumo(
                m.GetProperty("idMeal").GetString()!,
                m.GetProperty("strMeal").GetString()!,
                m.GetProperty("strMealThumb").GetString()!))
            .ToList();
    }
}
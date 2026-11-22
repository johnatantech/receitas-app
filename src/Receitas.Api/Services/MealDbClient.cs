using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Receitas.Api.Models;

namespace Receitas.Api.Services;

public class MealDbClient : IMealDbClient
{
    private static readonly TimeSpan TempoCache = TimeSpan.FromHours(6);

    private readonly HttpClient _http;
    private readonly IMemoryCache _cache;

    public MealDbClient(HttpClient http, IMemoryCache cache)
    {
        _http = http;
        _cache = cache;
    }

    public async Task<IReadOnlyList<ReceitaResumo>> BuscarPorIngredienteAsync(
        string ingrediente, CancellationToken ct = default)
    {
        var chave = $"filtro:{ingrediente.Trim().ToLowerInvariant()}";

        return (await _cache.GetOrCreateAsync(chave, async entrada =>
        {
            entrada.AbsoluteExpirationRelativeToNow = TempoCache;

            using var doc = await GetJsonAsync(
                $"filter.php?i={Uri.EscapeDataString(ingrediente)}", ct);

            // A API devolve {"meals": null} quando não encontra nada
            if (!TemLista(doc, out var meals)) return (IReadOnlyList<ReceitaResumo>)[];

            return (IReadOnlyList<ReceitaResumo>)meals.EnumerateArray()
                .Select(m => new ReceitaResumo(
                    m.GetProperty("idMeal").GetString()!,
                    m.GetProperty("strMeal").GetString()!,
                    m.GetProperty("strMealThumb").GetString()!))
                .ToList();
        }))!;
    }

    public async Task<ReceitaDetalhe?> BuscarDetalheAsync(
        string id, CancellationToken ct = default)
    {
        return await _cache.GetOrCreateAsync($"detalhe:{id}", async entrada =>
        {
            entrada.AbsoluteExpirationRelativeToNow = TempoCache;

            using var doc = await GetJsonAsync(
                $"lookup.php?i={Uri.EscapeDataString(id)}", ct);

            if (!TemLista(doc, out var meals) || meals.GetArrayLength() == 0)
                return null;

            var m = meals[0];

            // A API usa strIngredient1 ... strIngredient20 (vazios quando não usados)
            var ingredientes = new List<string>();
            for (var i = 1; i <= 20; i++)
            {
                if (!m.TryGetProperty($"strIngredient{i}", out var p)) continue;
                var valor = p.GetString();
                if (!string.IsNullOrWhiteSpace(valor))
                    ingredientes.Add(valor.Trim());
            }

            return new ReceitaDetalhe(
                m.GetProperty("idMeal").GetString()!,
                m.GetProperty("strMeal").GetString()!,
                m.GetProperty("strMealThumb").GetString()!,
                Texto(m, "strCategory"),
                Texto(m, "strArea"),
                Texto(m, "strInstructions"),
                ingredientes);
        });
    }

    private async Task<JsonDocument> GetJsonAsync(string url, CancellationToken ct)
    {
        using var resposta = await _http.GetAsync(url, ct);
        resposta.EnsureSuccessStatusCode();
        return await JsonDocument.ParseAsync(
            await resposta.Content.ReadAsStreamAsync(ct), cancellationToken: ct);
    }

    private static bool TemLista(JsonDocument doc, out JsonElement meals) =>
        doc.RootElement.TryGetProperty("meals", out meals)
        && meals.ValueKind == JsonValueKind.Array;

    private static string? Texto(JsonElement e, string campo) =>
        e.TryGetProperty(campo, out var p) ? p.GetString() : null;
}
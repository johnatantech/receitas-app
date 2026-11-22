using Microsoft.AspNetCore.Mvc;
using Receitas.Api.Models;
using Receitas.Api.Services;

namespace Receitas.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReceitasController : ControllerBase
{
    private readonly IMealDbClient _client;

    public ReceitasController(IMealDbClient client) => _client = client;

    // GET /api/receitas?ingredientes=chicken,garlic,onion&limite=20
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ReceitaRanqueada>>> Buscar(
        [FromQuery] string ingredientes,
        [FromQuery] int limite = 20,
        CancellationToken ct = default)
    {
        var lista = (ingredientes ?? "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (lista.Count == 0)
            return BadRequest("Informe ao menos um ingrediente.");

        limite = Math.Clamp(limite, 1, 30);

        // 1. Uma busca por ingrediente, em paralelo
        var buscas = await Task.WhenAll(
            lista.Select(i => _client.BuscarPorIngredienteAsync(i, ct)));

        // 2. União: receitas que aparecem em mais buscas vêm primeiro
        var candidatos = buscas
            .SelectMany(b => b)
            .GroupBy(r => r.Id)
            .OrderByDescending(g => g.Count())
            .Take(limite)
            .Select(g => g.Key)
            .ToList();

        // 3. Detalhe de cada candidato (a maioria vem do cache)
        var detalhes = await Task.WhenAll(
            candidatos.Select(id => _client.BuscarDetalheAsync(id, ct)));

        // 4. Compatibilidade + ordenação
        var ranqueadas = RankingService.Ordenar(
            detalhes
                .Where(d => d is not null)
                .Select(d => RankingService.Calcular(lista, d!)));

        return Ok(ranqueadas);
    }
}
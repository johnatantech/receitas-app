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

    // GET /api/receitas?ingredientes=chicken,garlic
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReceitaResumo>>> Buscar(
        [FromQuery] string ingredientes,
        CancellationToken ct)
    {
        var lista = ingredientes
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (lista.Count == 0)
            return BadRequest("Informe ao menos um ingrediente.");

        var resultados = await Task.WhenAll(
            lista.Select(i => _client.BuscarPorIngredienteAsync(i, ct)));

        // Interseção: só receitas que aparecem em TODAS as buscas
        var comum = resultados
            .Skip(1)
            .Aggregate(
                resultados[0].Select(r => r.Id).ToHashSet(),
                (acc, atual) => { acc.IntersectWith(atual.Select(r => r.Id)); return acc; });

        return Ok(resultados[0].Where(r => comum.Contains(r.Id)));
    }
}
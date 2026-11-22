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

    // GET /api/receitas?ingredientes=frango,alho,cebola&limite=20
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ReceitaRanqueada>>> Buscar(
        [FromQuery] string ingredientes,
        [FromQuery] int limite = 20,
        CancellationToken ct = default)
    {
        // Traduz PT→EN (o que não estiver no dicionário passa como veio)
        var lista = (ingredientes ?? "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(TraducaoService.ParaIngles)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (lista.Count == 0)
            return BadRequest("Informe ao menos um ingrediente.");

        limite = Math.Clamp(limite, 1, 30);

        var buscas = await Task.WhenAll(
            lista.Select(i => _client.BuscarPorIngredienteAsync(i, ct)));

        var candidatos = buscas
            .SelectMany(b => b)
            .GroupBy(r => r.Id)
            .OrderByDescending(g => g.Count())
            .Take(limite)
            .Select(g => g.Key)
            .ToList();

        var detalhes = await Task.WhenAll(
            candidatos.Select(id => _client.BuscarDetalheAsync(id, ct)));

        var ranqueadas = RankingService.Ordenar(
            detalhes
                .Where(d => d is not null)
                .Select(d => RankingService.Calcular(lista, d!)));

        // Traduz de volta só para exibição (o ranking já foi calculado em inglês)
        var traduzidas = ranqueadas
            .Select(r => r with
            {
                Possui = r.Possui.Select(TraducaoService.ParaPortugues).ToList(),
                Faltando = r.Faltando.Select(TraducaoService.ParaPortugues).ToList()
            })
            .ToList();

        return Ok(traduzidas);
    }

    // GET /api/receitas/52795
    [HttpGet("{id}")]
    public async Task<ActionResult<ReceitaCompleta>> Detalhe(
        string id, CancellationToken ct)
    {
        var receita = await _client.BuscarCompletaAsync(id, ct);
        return receita is null ? NotFound() : Ok(receita);
    }
}
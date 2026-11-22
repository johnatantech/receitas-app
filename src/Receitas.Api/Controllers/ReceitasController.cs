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
        var lista = ParseIngredientes(ingredientes);

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

    // GET /api/receitas/52795?ingredientes=frango,alho
    [HttpGet("{id}")]
    public async Task<ActionResult<ReceitaCompleta>> Detalhe(
        string id,
        [FromQuery] string? ingredientes,
        CancellationToken ct)
    {
        var receita = await _client.BuscarCompletaAsync(id, ct);
        if (receita is null) return NotFound();

        var doUsuario = ParseIngredientes(ingredientes);

        // "with" cria cópias: o objeto em cache não é alterado
        var marcados = receita.Ingredientes
            .Select(i => i with
            {
                NomePt = TraducaoService.ParaPortugues(i.Nome),
                Basico = RankingService.EhBasico(i.Nome),
                Possui = doUsuario.Any(u => RankingService.Corresponde(i.Nome, u))
            })
            .ToList();

        int? compatibilidade = null;
        if (doUsuario.Count > 0)
        {
            var relevantes = marcados.Where(i => !i.Basico).ToList();
            compatibilidade = relevantes.Count == 0
                ? 0
                : (int)Math.Round(100.0 * relevantes.Count(i => i.Possui) / relevantes.Count);
        }

        return Ok(receita with
        {
            Ingredientes = marcados,
            Compatibilidade = compatibilidade
        });
    }

    // "frango, Alho" → ["chicken", "garlic"] (o que não está no dicionário passa como veio)
    private static List<string> ParseIngredientes(string? texto) =>
        (texto ?? "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(TraducaoService.ParaIngles)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
}
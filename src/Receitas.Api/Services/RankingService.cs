using Receitas.Api.Models;

namespace Receitas.Api.Services;

public static class RankingService
{
    // Itens que quase todo mundo tem em casa: não entram no cálculo
    private static readonly HashSet<string> Basicos = new(StringComparer.OrdinalIgnoreCase)
    {
        "salt", "water", "pepper", "black pepper", "ground black pepper"
    };

    public static ReceitaRanqueada Calcular(
        IReadOnlyCollection<string> doUsuario, ReceitaDetalhe receita)
    {
        var relevantes = receita.Ingredientes
            .Where(i => !Basicos.Contains(Normalizar(i)))
            .ToList();

        var possui = relevantes
            .Where(i => doUsuario.Any(u => Corresponde(i, u)))
            .ToList();

        var faltando = relevantes
            .Where(i => !possui.Contains(i))
            .ToList();

        var percentual = relevantes.Count == 0
            ? 0
            : (int)Math.Round(100.0 * possui.Count / relevantes.Count);

        return new ReceitaRanqueada(
            receita.Id, receita.Titulo, receita.Imagem, percentual, possui, faltando);
    }

    public static IReadOnlyList<ReceitaRanqueada> Ordenar(
        IEnumerable<ReceitaRanqueada> receitas) =>
        receitas
            .OrderByDescending(r => r.Compatibilidade)
            .ThenByDescending(r => r.Possui.Count)
            .ThenBy(r => r.Faltando.Count)
            .ThenBy(r => r.Titulo, StringComparer.OrdinalIgnoreCase)
            .ToList();

    // "chicken" corresponde a "Chicken Breasts"; "onion" corresponde a "Onions"
    private static bool Corresponde(string ingredienteReceita, string ingredienteUsuario)
    {
        var receita = Palavras(ingredienteReceita);
        var usuario = Palavras(ingredienteUsuario);
        return usuario.Length > 0 && usuario.All(receita.Contains);
    }

    private static string Normalizar(string texto) => string.Join(' ', Palavras(texto));

    private static string[] Palavras(string texto) =>
        texto.ToLowerInvariant()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(Singular)
            .ToArray();

    private static string Singular(string palavra) =>
        palavra.Length > 3 && palavra.EndsWith('s') && !palavra.EndsWith("ss")
            ? palavra[..^1]
            : palavra;
}
namespace Receitas.Api.Models;

public record IngredienteComMedida(string Nome, string Medida)
{
    public string NomePt { get; init; } = Nome;
    public bool Possui { get; init; }
    public bool Basico { get; init; }
}

public record ReceitaCompleta(
    string Id,
    string Titulo,
    string Imagem,
    string? Categoria,
    string? Area,
    string? Instrucoes,
    string? Video,
    IReadOnlyList<IngredienteComMedida> Ingredientes
)
{
    public int? Compatibilidade { get; init; }
    public string? InstrucoesPt { get; init; }
}
namespace Receitas.Api.Models;

public record IngredienteComMedida(string Nome, string Medida);

public record ReceitaCompleta(
    string Id,
    string Titulo,
    string Imagem,
    string? Categoria,
    string? Area,
    string? Instrucoes,
    string? Video,
    IReadOnlyList<IngredienteComMedida> Ingredientes
);
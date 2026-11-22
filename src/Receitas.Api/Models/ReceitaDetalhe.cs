namespace Receitas.Api.Models;

public record ReceitaDetalhe(
    string Id,
    string Titulo,
    string Imagem,
    string? Categoria,
    string? Area,
    string? Instrucoes,
    IReadOnlyList<string> Ingredientes
);
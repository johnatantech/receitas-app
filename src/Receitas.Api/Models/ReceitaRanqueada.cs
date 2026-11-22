namespace Receitas.Api.Models;

public record ReceitaRanqueada(
    string Id,
    string Titulo,
    string Imagem,
    int Compatibilidade,               // 0 a 100
    IReadOnlyList<string> Possui,
    IReadOnlyList<string> Faltando
);
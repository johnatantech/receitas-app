namespace Receitas.Api.Services;

public interface ITradutor
{
    /// <summary>Traduz EN→PT-BR. Devolve null se a tradução falhar.</summary>
    Task<string?> ParaPortuguesAsync(string texto, CancellationToken ct = default);
}
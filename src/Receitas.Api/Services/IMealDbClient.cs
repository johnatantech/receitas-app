using Receitas.Api.Models;

namespace Receitas.Api.Services;

public interface IMealDbClient
{
    Task<IReadOnlyList<ReceitaResumo>> BuscarPorIngredienteAsync(
        string ingrediente,
        CancellationToken ct = default);
}
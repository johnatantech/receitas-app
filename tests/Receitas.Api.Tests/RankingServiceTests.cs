using Receitas.Api.Models;
using Receitas.Api.Services;

namespace Receitas.Api.Tests;

public class RankingServiceTests
{
    private static ReceitaDetalhe Receita(string titulo, params string[] ingredientes) =>
        new(titulo, titulo, "img.jpg", null, null, null, ingredientes);

    [Fact]
    public void Retorna100_QuandoUsuarioTemTodosOsIngredientes()
    {
        var receita = Receita("Frango com alho", "Chicken", "Garlic");

        var r = RankingService.Calcular(["chicken", "garlic"], receita);

        Assert.Equal(100, r.Compatibilidade);
        Assert.Empty(r.Faltando);
    }

    [Fact]
    public void CalculaPercentualEListaFaltantes_QuandoParcial()
    {
        var receita = Receita("Prato", "Chicken Breast", "Garlic", "Onion", "Olive Oil");

        var r = RankingService.Calcular(["chicken", "garlic"], receita);

        Assert.Equal(50, r.Compatibilidade);
        Assert.Equal(["Onion", "Olive Oil"], r.Faltando);
        Assert.Equal(["Chicken Breast", "Garlic"], r.Possui);
    }

    [Fact]
    public void IgnoraIngredientesBasicos()
    {
        var receita = Receita("Simples", "Chicken", "Salt", "Water", "Black Pepper");

        var r = RankingService.Calcular(["chicken"], receita);

        Assert.Equal(100, r.Compatibilidade);
    }

    [Fact]
    public void IgnoraMaiusculasEPlural()
    {
        var receita = Receita("Cebola", "Onions");

        var r = RankingService.Calcular(["ONION"], receita);

        Assert.Equal(100, r.Compatibilidade);
    }

    [Fact]
    public void Retorna0_QuandoNadaCorresponde()
    {
        var receita = Receita("Outro", "Beef", "Carrot");

        var r = RankingService.Calcular(["chicken"], receita);

        Assert.Equal(0, r.Compatibilidade);
        Assert.Empty(r.Possui);
    }

    [Fact]
    public void NaoDivideProZero_QuandoReceitaSoTemBasicos()
    {
        var receita = Receita("Agua com sal", "Water", "Salt");

        var r = RankingService.Calcular(["chicken"], receita);

        Assert.Equal(0, r.Compatibilidade);
    }

    [Fact]

        [Theory]
    [InlineData("Salt", true)]
    [InlineData("Black Pepper", true)]
    [InlineData("Chicken", false)]
    public void IdentificaIngredientesBasicos(string ingrediente, bool esperado) =>
        Assert.Equal(esperado, RankingService.EhBasico(ingrediente));

    [Theory]
    [InlineData("Chicken Breasts", "chicken", true)]
    [InlineData("Onions", "onion", true)]
    [InlineData("Garlic", "chicken", false)]
    public void CorrespondeIngredienteDaReceitaAoDoUsuario(
        string daReceita, string doUsuario, bool esperado) =>
        Assert.Equal(esperado, RankingService.Corresponde(daReceita, doUsuario));
    public void Ordena_PorCompatibilidade_DepoisPorQuantidadeUsada()
    {
        var a = RankingService.Calcular(["a", "b"], Receita("A", "a", "b", "c", "d"));      // 50%, usa 2
        var b = RankingService.Calcular(["a", "b"], Receita("B", "a", "x"));                // 50%, usa 1
        var c = RankingService.Calcular(["a", "b"], Receita("C", "a", "b"));                // 100%

        var ordenadas = RankingService.Ordenar([b, a, c]);

        Assert.Equal(["C", "A", "B"], ordenadas.Select(r => r.Titulo));
    }
}
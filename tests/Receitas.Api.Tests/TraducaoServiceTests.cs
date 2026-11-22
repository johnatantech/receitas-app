using Receitas.Api.Services;

namespace Receitas.Api.Tests;

public class TraducaoServiceTests
{
    [Theory]
    [InlineData("frango", "chicken")]
    [InlineData("Alho", "garlic")]
    [InlineData("  CEBOLA  ", "onion")]
    [InlineData("cebolas", "onion")]
    [InlineData("limões", "lemon")]
    [InlineData("pimentão", "red pepper")]
    [InlineData("pimenta do reino", "black pepper")]
    [InlineData("peito de frango", "chicken breast")]
    public void TraduzPortuguesParaIngles(string entrada, string esperado) =>
        Assert.Equal(esperado, TraducaoService.ParaIngles(entrada));

    [Fact]
    public void MantemTermoDesconhecido_ComoFoiDigitado() =>
        Assert.Equal("chicken", TraducaoService.ParaIngles("chicken"));

    [Fact]
    public void MantemTermoDesconhecido_SemInventarTraducao() =>
        Assert.Equal("quinoa", TraducaoService.ParaIngles("quinoa"));

    [Theory]
    [InlineData("Olive Oil", "azeite")]
    [InlineData("Garlic", "alho")]
    [InlineData("Onion", "cebola")]
    public void TraduzInglesParaPortugues_ParaExibicao(string entrada, string esperado) =>
        Assert.Equal(esperado, TraducaoService.ParaPortugues(entrada));

    [Fact]
    public void ExibicaoMantemOriginal_QuandoNaoHaTraducao() =>
        Assert.Equal("Quinoa", TraducaoService.ParaPortugues("Quinoa"));

    [Fact]
    public void DoisTermosPortuguesesParaMesmoIngles_VoltamPeloPrimeiro() =>
        Assert.Equal("carne", TraducaoService.ParaPortugues("Beef"));
}
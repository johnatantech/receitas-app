using Receitas.Api.Services;

namespace Receitas.Api.Tests;

public class MyMemoryTradutorTests
{
    [Fact]
    public void TextoCurto_ViraUmTrechoSo()
    {
        var trechos = MyMemoryTradutor.Dividir("Heat the oil. Add the garlic.");

        Assert.Single(trechos);
    }

    [Fact]
    public void TextoLongo_NaoPassaDoLimite_ENaoPerdeConteudo()
    {
        var frase = "Stir the onions slowly until they turn golden brown. ";
        var texto = string.Concat(Enumerable.Repeat(frase, 30)).Trim();

        var trechos = MyMemoryTradutor.Dividir(texto, 200);

        Assert.True(trechos.Count > 1);
        Assert.All(trechos, t => Assert.True(t.Length <= 200));
        Assert.Equal(
            texto.Split(' ', StringSplitOptions.RemoveEmptyEntries),
            string.Join(' ', trechos).Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }

    [Fact]
    public void FraseGiganteSemPontuacao_EhCortadaEmEspacos()
    {
        var texto = string.Join(' ', Enumerable.Repeat("palavra", 200));

        var trechos = MyMemoryTradutor.Dividir(texto, 100);

        Assert.All(trechos, t => Assert.True(t.Length <= 100));
    }
}
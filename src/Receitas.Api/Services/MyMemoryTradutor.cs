using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;

namespace Receitas.Api.Services;

public class MyMemoryTradutor : ITradutor
{
    private const int MaxChars = 450;
    private static readonly TimeSpan TempoCache = TimeSpan.FromDays(30);

    private static readonly Regex Tags = new(@"<[^>]+>", RegexOptions.Compiled);
    private static readonly Regex EspacosMultiplos = new(@"[ \t]{2,}", RegexOptions.Compiled);

    private readonly HttpClient _http;
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _config;
    private readonly ILogger<MyMemoryTradutor> _log;

    public MyMemoryTradutor(
        HttpClient http, IMemoryCache cache,
        IConfiguration config, ILogger<MyMemoryTradutor> log)
    {
        _http = http;
        _cache = cache;
        _config = config;
        _log = log;
    }

    public async Task<string?> ParaPortuguesAsync(string texto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(texto)) return texto;

        // "v2" na chave: invalida traduções antigas que ficaram com tags
        var chave = "trad:v2:" + Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(texto)));

        if (_cache.TryGetValue(chave, out string? pronto)) return pronto;

        try
        {
            var paragrafos = texto.Split(
                ['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            // Parágrafos em paralelo; os trechos de cada parágrafo em sequência
            var traduzidos = await Task.WhenAll(paragrafos.Select(async p =>
            {
                var partes = new List<string>();
                foreach (var trecho in Dividir(p))
                    partes.Add(await TraduzirTrechoAsync(trecho, ct));
                return string.Join(' ', partes);
            }));

            var resultado = string.Join("\n",
                traduzidos.Where(t => !string.IsNullOrWhiteSpace(t)));

            _cache.Set(chave, resultado, TempoCache); // só guarda se deu tudo certo
            return resultado;
        }
        catch (Exception ex)
        {
            _log.LogWarning(ex, "Falha ao traduzir o texto; exibindo o original.");
            return null;
        }
    }

    private async Task<string> TraduzirTrechoAsync(string trecho, CancellationToken ct)
    {
        var url = $"get?q={Uri.EscapeDataString(trecho)}&langpair=en|pt-BR";

        // Opcional: informar um e-mail aumenta a cota diária gratuita
        var email = _config["Traducao:Email"];
        if (!string.IsNullOrWhiteSpace(email))
            url += $"&de={Uri.EscapeDataString(email)}";

        using var resposta = await _http.GetAsync(url, ct);
        resposta.EnsureSuccessStatusCode();

        using var doc = await JsonDocument.ParseAsync(
            await resposta.Content.ReadAsStreamAsync(ct), cancellationToken: ct);

        var status = doc.RootElement.GetProperty("responseStatus").ToString();
        var traduzido = doc.RootElement
            .GetProperty("responseData")
            .GetProperty("translatedText")
            .GetString();

        // Quando a cota acaba, a API devolve um aviso no lugar da tradução
        if (status != "200" || string.IsNullOrWhiteSpace(traduzido)
            || traduzido.Contains("MYMEMORY WARNING", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"MyMemory respondeu status {status}.");

        return Limpar(traduzido);
    }

    /// <summary>
    /// Remove entidades HTML e as tags internas que a MyMemory às vezes devolve
    /// (ex.: &lt;g id="1"&gt;, &lt;x id="2"/&gt;), mantendo o texto que estava dentro delas.
    /// </summary>
    public static string Limpar(string traduzido)
    {
        // Decodifica antes: a API pode devolver "&lt;g id=&quot;1&quot;&gt;" escapado
        var decodificado = WebUtility.HtmlDecode(traduzido);
        var semTags = Tags.Replace(decodificado, " ");
        return EspacosMultiplos.Replace(semTags, " ").Trim();
    }

    /// <summary>Divide em trechos de até <paramref name="max"/> caracteres, cortando em frases.</summary>
    public static IReadOnlyList<string> Dividir(string texto, int max = MaxChars)
    {
        var trechos = new List<string>();
        var atual = new StringBuilder();

        void Fechar()
        {
            if (atual.Length > 0) trechos.Add(atual.ToString().Trim());
            atual.Clear();
        }

        foreach (var frase in Regex.Split(texto.Trim(), @"(?<=[.!?])\s+"))
        {
            var restante = frase;

            // Frase maior que o limite: corta em espaços
            while (restante.Length > max)
            {
                var corte = restante.LastIndexOf(' ', max);
                if (corte <= 0) corte = max;
                Fechar();
                trechos.Add(restante[..corte].Trim());
                restante = restante[corte..].Trim();
            }

            if (atual.Length + restante.Length + 1 > max) Fechar();
            if (atual.Length > 0) atual.Append(' ');
            atual.Append(restante);
        }

        Fechar();
        return trechos.Where(t => t.Length > 0).ToList();
    }
}
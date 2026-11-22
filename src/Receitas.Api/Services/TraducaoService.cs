using System.Globalization;
using System.Text;

namespace Receitas.Api.Services;

public static class TraducaoService
{
    // (português, inglês). O PRIMEIRO português de cada inglês é usado na volta.
    private static readonly (string Pt, string En)[] Pares =
    [
        // Carnes e peixes
        ("frango", "chicken"), ("peito de frango", "chicken breast"),
        ("coxa de frango", "chicken thighs"),
        ("carne", "beef"), ("carne bovina", "beef"), ("carne moida", "minced beef"),
        ("porco", "pork"), ("bacon", "bacon"), ("presunto", "ham"),
        ("linguica", "sausage"), ("cordeiro", "lamb"), ("peru", "turkey"),
        ("peixe", "white fish"), ("salmao", "salmon"), ("atum", "tuna"),
        ("camarao", "prawns"), ("bacalhau", "salt cod"),

        // Legumes e verduras
        ("alho", "garlic"), ("cebola", "onion"), ("cebolinha", "spring onions"),
        ("tomate", "tomato"), ("batata", "potatoes"), ("batata doce", "sweet potatoes"),
        ("cenoura", "carrots"), ("abobora", "pumpkin"), ("abobrinha", "courgettes"),
        ("pimentao", "red pepper"), ("pimentao verde", "green pepper"),
        ("brocolis", "broccoli"), ("espinafre", "spinach"), ("cogumelo", "mushrooms"),
        ("champignon", "mushrooms"), ("milho", "sweetcorn"), ("ervilha", "peas"),
        ("berinjela", "aubergine"), ("pepino", "cucumber"), ("alface", "lettuce"),
        ("couve", "kale"), ("repolho", "cabbage"), ("gengibre", "ginger"),
        ("aipo", "celery"), ("mandioca", "cassava"),

        // Frutas
        ("limao", "lemon"), ("laranja", "orange"), ("maca", "apple"),
        ("banana", "banana"), ("abacate", "avocado"), ("coco", "coconut"),
        ("abacaxi", "pineapple"), ("uva passa", "raisins"),

        // Laticínios e ovos
        ("ovo", "eggs"), ("leite", "milk"), ("manteiga", "butter"),
        ("queijo", "cheese"), ("queijo parmesao", "parmesan"),
        ("parmesao", "parmesan"), ("mussarela", "mozzarella"),
        ("creme de leite", "double cream"), ("iogurte", "yogurt"),

        // Básicos e despensa
        ("arroz", "rice"), ("feijao", "kidney beans"), ("grao de bico", "chickpeas"),
        ("lentilha", "lentils"), ("macarrao", "spaghetti"), ("espaguete", "spaghetti"),
        ("farinha", "flour"), ("farinha de trigo", "plain flour"),
        ("acucar", "sugar"), ("acucar mascavo", "brown sugar"),
        ("pao", "bread"), ("azeite", "olive oil"), ("oleo", "vegetable oil"),
        ("vinagre", "white wine vinegar"), ("molho de tomate", "tomato puree"),
        ("extrato de tomate", "tomato puree"), ("tomate pelado", "chopped tomatoes"),
        ("caldo de galinha", "chicken stock"), ("caldo de carne", "beef stock"),
        ("mel", "honey"), ("mostarda", "dijon mustard"), ("molho de soja", "soy sauce"),
        ("amendoim", "peanuts"), ("castanha", "cashew nuts"), ("azeitona", "olives"),

        // Temperos
        ("sal", "salt"), ("pimenta do reino", "black pepper"), ("pimenta", "black pepper"),
        ("salsinha", "parsley"), ("salsa", "parsley"), ("coentro", "coriander"),
        ("manjericao", "basil"), ("oregano", "oregano"), ("tomilho", "thyme"),
        ("alecrim", "rosemary"), ("louro", "bay leaf"), ("canela", "cinnamon"),
        ("paprica", "paprika"), ("cominho", "cumin"), ("curry", "curry powder"),
        ("pimenta caiena", "cayenne pepper"), ("agua", "water"),
    ];

    private static readonly Dictionary<string, string> ParaInglesMap = CriarMapaPtEn();
    private static readonly Dictionary<string, string> ParaPortuguesMap = CriarMapaEnPt();

    /// <summary>Traduz PT→EN. Se não achar no dicionário, devolve o termo original.</summary>
    public static string ParaIngles(string termo)
    {
        var chave = Normalizar(termo);
        return ParaInglesMap.TryGetValue(chave, out var en) ? en : termo.Trim();
    }

    /// <summary>Traduz EN→PT para exibição. Se não achar, devolve o termo original.</summary>
    public static string ParaPortugues(string termo)
    {
        var chave = Normalizar(termo);
        return ParaPortuguesMap.TryGetValue(chave, out var pt) ? pt : termo.Trim();
    }

    private static Dictionary<string, string> CriarMapaPtEn() =>
        Pares.ToDictionary(p => Normalizar(p.Pt), p => p.En);

    private static Dictionary<string, string> CriarMapaEnPt()
    {
        var mapa = new Dictionary<string, string>();
        foreach (var (pt, en) in Pares)
            mapa.TryAdd(Normalizar(en), pt); // TryAdd: vale o primeiro português listado
        return mapa;
    }

    // minúsculas, sem acento, sem plural simples: "Cebolas" → "cebola", "Limões" → "limao"
    private static string Normalizar(string texto)
    {
        var sb = new StringBuilder();
        foreach (var c in texto.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD))
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);

        var limpo = sb.ToString().Normalize(NormalizationForm.FormC);
        return string.Join(' ', limpo.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(Singular));
    }

    private static string Singular(string p)
    {
        if (p.Length > 4 && p.EndsWith("oes")) return p[..^3] + "ao";
        if (p.Length > 4 && p.EndsWith("aes")) return p[..^3] + "ao";
        if (p.Length > 3 && p.EndsWith('s') && !p.EndsWith("ss")) return p[..^1];
        return p;
    }
}


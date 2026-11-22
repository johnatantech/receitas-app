import { Link, useParams } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";
import { buscarReceita } from "../api/receitas";
import { useIngredientes } from "../hooks/useIngredientes";

function idDoYoutube(url: string): string | null {
  try {
    return new URL(url).searchParams.get("v");
  } catch {
    return null;
  }
}

export default function RecipeDetail() {
  const { id = "" } = useParams();
  const [ingredientes] = useIngredientes();
  const busca = ingredientes.join(",");

  const { data, isLoading, error } = useQuery({
    queryKey: ["receita", id, busca],
    queryFn: () => buscarReceita(id, ingredientes),
    retry: false,
  });

  const voltar = (
    <Link to={busca ? `/?ingredientes=${encodeURIComponent(busca)}` : "/"}>
      ← Voltar para a busca
    </Link>
  );

  if (isLoading) return <p>Carregando receita...</p>;

  if (error?.message === "NAO_ENCONTRADA")
    return (
      <>
        {voltar}
        <p>Receita não encontrada.</p>
      </>
    );

  if (error || !data)
    return (
      <>
        {voltar}
        <p>Não foi possível carregar a receita.</p>
      </>
    );

  const videoId = data.video ? idDoYoutube(data.video) : null;
  const passos = (data.instrucoes ?? "")
    .split(/\r?\n/)
    .map((p) => p.trim())
    .filter(Boolean);

  const temBusca = data.compatibilidade !== null;
  const relevantes = data.ingredientes.filter((i) => !i.basico);
  const quantosTem = relevantes.filter((i) => i.possui).length;

  return (
    <article>
      {voltar}
      <h1>{data.titulo}</h1>
      <p style={{ color: "#9ca3af" }}>
        {[data.categoria, data.area].filter(Boolean).join(" · ")}
      </p>

      <img
        src={data.imagem}
        alt={data.titulo}
        style={{ width: "100%", maxWidth: 480, borderRadius: 8 }}
      />

      <h2>Ingredientes</h2>

      {temBusca && (
        <p style={{ fontWeight: 600 }}>
          Você tem {quantosTem} de {relevantes.length} ingredientes (
          {data.compatibilidade}% compatível)
        </p>
      )}

      <ul style={{ listStyle: "none", padding: 0 }}>
        {data.ingredientes.map((i) => {
          let marca = "";
          let cor = "inherit";
          let rotulo = "";

          if (temBusca) {
            if (i.basico) {
              marca = "•";
              cor = "#9ca3af";
              rotulo = "item básico";
            } else if (i.possui) {
              marca = "✓";
              cor = "#16a34a";
              rotulo = "você tem";
            } else {
              marca = "✗";
              cor = "#dc2626";
              rotulo = "falta";
            }
          }

          return (
            <li key={i.nome} style={{ padding: "4px 0" }}>
              {temBusca && (
                <span
                  aria-label={rotulo}
                  title={rotulo}
                  style={{ color: cor, fontWeight: 700, marginRight: 8 }}
                >
                  {marca}
                </span>
              )}
              {i.medida && <strong>{i.medida} </strong>}
              {i.nomePt}
              {i.nomePt.toLowerCase() !== i.nome.toLowerCase() && (
                <span style={{ color: "#9ca3af" }}> ({i.nome})</span>
              )}
            </li>
          );
        })}
      </ul>

      <h2>Modo de preparo</h2>
      {passos.map((p, idx) => (
        <p key={idx}>{p}</p>
      ))}

      {videoId && (
        <>
          <h2>Vídeo</h2>
          <iframe
            title={`Vídeo: ${data.titulo}`}
            src={`https://www.youtube.com/embed/${videoId}`}
            style={{ width: "100%", maxWidth: 640, aspectRatio: "16 / 9", border: 0 }}
            allowFullScreen
          />
        </>
      )}
    </article>
  );
}
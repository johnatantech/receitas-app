import { Link, useParams, useSearchParams } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";
import { buscarReceita } from "../api/receitas";

function idDoYoutube(url: string): string | null {
  try {
    return new URL(url).searchParams.get("v");
  } catch {
    return null;
  }
}

export default function RecipeDetail() {
  const { id = "" } = useParams();
  const [params] = useSearchParams();
  const busca = params.get("ingredientes") ?? "";

  const { data, isLoading, error } = useQuery({
    queryKey: ["receita", id],
    queryFn: () => buscarReceita(id),
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
      <ul>
        {data.ingredientes.map((i) => (
          <li key={i.nome}>
            {i.medida && <strong>{i.medida} </strong>}
            {i.nome}
          </li>
        ))}
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
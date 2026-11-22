import { useState } from "react";
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
  const [verOriginal, setVerOriginal] = useState(false);

  const { data, isLoading, error } = useQuery({
    queryKey: ["receita", id, busca],
    queryFn: () => buscarReceita(id, ingredientes),
    retry: false,
  });

  const voltar = (
    <Link
      to={busca ? `/?ingredientes=${encodeURIComponent(busca)}` : "/"}
      className="inline-flex items-center gap-1 text-sm font-medium text-amber-700 hover:underline dark:text-amber-500"
    >
      ← Voltar para a busca
    </Link>
  );

  if (isLoading)
    return (
      <div className="space-y-4">
        {voltar}
        <p className="animate-pulse text-stone-500">Carregando receita...</p>
      </div>
    );

  if (error?.message === "NAO_ENCONTRADA")
    return (
      <div className="space-y-4">
        {voltar}
        <p>Receita não encontrada.</p>
      </div>
    );

  if (error || !data)
    return (
      <div className="space-y-4">
        {voltar}
        <p role="alert">Não foi possível carregar a receita.</p>
      </div>
    );

  const videoId = data.video ? idDoYoutube(data.video) : null;

  const traducaoDisponivel = Boolean(data.instrucoesPt);
  const textoPreparo =
    traducaoDisponivel && !verOriginal ? data.instrucoesPt : data.instrucoes;
  const passos = (textoPreparo ?? "")
    .split(/\r?\n/)
    .map((p) => p.trim())
    .filter(Boolean);

  const temBusca = data.compatibilidade !== null;
  const relevantes = data.ingredientes.filter((i) => !i.basico);
  const quantosTem = relevantes.filter((i) => i.possui).length;
  const pct = data.compatibilidade ?? 0;
  const corBarra =
    pct >= 80 ? "bg-emerald-600" : pct >= 50 ? "bg-amber-600" : "bg-rose-600";

  return (
    <article className="space-y-6">
      {voltar}

      <header className="space-y-1">
        <h1 className="text-3xl font-extrabold tracking-tight sm:text-4xl">
          {data.titulo}
        </h1>
        <p className="text-stone-500">
          {[data.categoria, data.area].filter(Boolean).join(" · ")}
        </p>
      </header>

      <div className="grid gap-8 md:grid-cols-[minmax(0,2fr)_minmax(0,3fr)]">
        <aside className="space-y-6">
          <img
            src={data.imagem}
            alt={data.titulo}
            className="aspect-[4/3] w-full rounded-2xl object-cover shadow"
          />

          <section className="rounded-2xl border border-stone-200 bg-white p-5 dark:border-stone-800 dark:bg-stone-900">
            <h2 className="mb-3 text-xl font-bold">Ingredientes</h2>

            {temBusca && (
              <div className="mb-4 space-y-2">
                <p className="text-sm font-medium">
                  Você tem {quantosTem} de {relevantes.length} ({pct}% compatível)
                </p>
                <div
                  role="progressbar"
                  aria-valuenow={pct}
                  aria-valuemin={0}
                  aria-valuemax={100}
                  className="h-2 overflow-hidden rounded-full bg-stone-200 dark:bg-stone-800"
                >
                  <div
                    className={`h-full rounded-full ${corBarra}`}
                    style={{ width: `${pct}%` }}
                  />
                </div>
              </div>
            )}

            <ul className="divide-y divide-stone-100 dark:divide-stone-800">
              {data.ingredientes.map((i) => {
                let marca = "";
                let classe = "";
                let rotulo = "";

                if (temBusca) {
                  if (i.basico) {
                    marca = "•";
                    classe = "text-stone-400";
                    rotulo = "item básico";
                  } else if (i.possui) {
                    marca = "✓";
                    classe = "text-emerald-600";
                    rotulo = "você tem";
                  } else {
                    marca = "✗";
                    classe = "text-rose-600";
                    rotulo = "falta";
                  }
                }

                return (
                  <li key={i.nome} className="flex gap-3 py-2 text-sm">
                    {temBusca && (
                      <span
                        aria-label={rotulo}
                        title={rotulo}
                        className={`w-4 shrink-0 text-center font-bold ${classe}`}
                      >
                        {marca}
                      </span>
                    )}
                    <span>
                      {i.medida && <strong>{i.medida} </strong>}
                      {i.nomePt}
                      {i.nomePt.toLowerCase() !== i.nome.toLowerCase() && (
                        <span className="text-stone-400"> ({i.nome})</span>
                      )}
                    </span>
                  </li>
                );
              })}
            </ul>
          </section>
        </aside>

        <div className="space-y-6">
          <section>
            <div className="mb-3 flex flex-wrap items-center justify-between gap-2">
              <h2 className="text-xl font-bold">Modo de preparo</h2>
              {traducaoDisponivel && (
                <button
                  type="button"
                  onClick={() => setVerOriginal((v) => !v)}
                  className="rounded-lg border border-stone-300 px-3 py-1 text-sm transition hover:bg-stone-100 dark:border-stone-700 dark:hover:bg-stone-800"
                >
                  {verOriginal ? "Ver em português" : "Ver original (inglês)"}
                </button>
              )}
            </div>

            {!traducaoDisponivel && (
              <p className="mb-3 text-sm text-stone-500">
                Tradução indisponível no momento. Exibindo o original em inglês.
              </p>
            )}
            {traducaoDisponivel && !verOriginal && (
              <p className="mb-3 text-sm text-stone-500">
                Tradução automática: confira quantidades e tempos no original, se
                tiver dúvida.
              </p>
            )}

            <div className="space-y-3 leading-relaxed">
              {passos.map((p, idx) => (
                <p key={idx}>{p}</p>
              ))}
            </div>
          </section>

          {videoId && (
            <section>
              <h2 className="mb-3 text-xl font-bold">Vídeo</h2>
              <iframe
                title={`Vídeo: ${data.titulo}`}
                src={`https://www.youtube.com/embed/${videoId}`}
                className="aspect-video w-full rounded-2xl border-0 shadow"
                allowFullScreen
              />
            </section>
          )}
        </div>
      </div>
    </article>
  );
}
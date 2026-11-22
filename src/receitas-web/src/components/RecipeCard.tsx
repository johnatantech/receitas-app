import { Link, useSearchParams } from "react-router-dom";
import type { ReceitaRanqueada } from "../types/receita";

function corDaBadge(pct: number) {
  if (pct >= 80) return "#16a34a"; // verde
  if (pct >= 50) return "#ca8a04"; // amarelo
  return "#dc2626"; // vermelho
}

export function RecipeCard({ receita }: { receita: ReceitaRanqueada }) {
  const [params] = useSearchParams();
  const busca = params.get("ingredientes") ?? "";

  return (
    <Link
      to={`/receita/${receita.id}?ingredientes=${encodeURIComponent(busca)}`}
      style={{ textDecoration: "none", color: "inherit" }}
    >
      <article
        style={{ border: "1px solid #444", borderRadius: 8, overflow: "hidden", height: "100%" }}
      >
        <img
          src={receita.imagem}
          alt={receita.titulo}
          style={{ width: "100%", display: "block" }}
        />
        <div style={{ padding: 12 }}>
          <span
            style={{
              background: corDaBadge(receita.compatibilidade),
              color: "#fff",
              padding: "2px 8px",
              borderRadius: 999,
              fontSize: 13,
              fontWeight: 600,
            }}
          >
            {receita.compatibilidade}% compatível
          </span>
          <h3 style={{ fontSize: 16, color: "#f3f4f6", margin: "8px 0" }}>
            {receita.titulo}
          </h3>
          {receita.faltando.length > 0 && (
            <p style={{ fontSize: 13, color: "#9ca3af", margin: 0 }}>
              Falta: {receita.faltando.join(", ")}
            </p>
          )}
        </div>
      </article>
    </Link>
  );
}
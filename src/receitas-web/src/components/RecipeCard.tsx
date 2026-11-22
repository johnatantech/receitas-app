import { Link, useSearchParams } from "react-router-dom";
import type { ReceitaRanqueada } from "../types/receita";

function classeDaBadge(pct: number) {
  if (pct >= 80) return "bg-emerald-600";
  if (pct >= 50) return "bg-amber-600";
  return "bg-rose-600";
}

export function RecipeCard({ receita }: { receita: ReceitaRanqueada }) {
  const [params] = useSearchParams();
  const busca = params.get("ingredientes") ?? "";

  return (
    <Link
      to={`/receita/${receita.id}?ingredientes=${encodeURIComponent(busca)}`}
      className="group flex h-full flex-col overflow-hidden rounded-2xl border border-stone-200 bg-white shadow-sm transition hover:-translate-y-1 hover:shadow-lg dark:border-stone-800 dark:bg-stone-900"
    >
      <div className="relative aspect-[4/3] overflow-hidden bg-stone-200 dark:bg-stone-800">
        <img
          src={receita.imagem}
          alt={receita.titulo}
          loading="lazy"
          className="h-full w-full object-cover transition duration-300 group-hover:scale-105"
        />
        <span
          className={`absolute left-3 top-3 rounded-full px-3 py-1 text-xs font-bold text-white shadow ${classeDaBadge(
            receita.compatibilidade
          )}`}
        >
          {receita.compatibilidade}% compatível
        </span>
      </div>

      <div className="flex flex-1 flex-col gap-2 p-4">
        <h3 className="font-semibold leading-snug">{receita.titulo}</h3>
        {receita.faltando.length > 0 ? (
          <p className="text-sm text-stone-500 dark:text-stone-400">
            <span className="font-medium">Falta:</span>{" "}
            {receita.faltando.join(", ")}
          </p>
        ) : (
          <p className="text-sm font-medium text-emerald-600">
            Você tem tudo! 🎉
          </p>
        )}
      </div>
    </Link>
  );
}
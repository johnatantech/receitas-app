import { useState } from "react";

interface Props {
  ingredientes: string[];
  onChange: (ingredientes: string[]) => void;
}

export function IngredientInput({ ingredientes, onChange }: Props) {
  const [texto, setTexto] = useState("");

  function adicionar() {
    const novos = texto
      .split(/,|\se\s/i)
      .map((s) => s.trim().toLowerCase())
      .filter((s) => s && !ingredientes.includes(s));

    if (novos.length > 0) {
      onChange([...ingredientes, ...new Set(novos)]);
    }
    setTexto("");
  }

  return (
    <div className="space-y-3">
      <div className="flex gap-2">
        <input
          value={texto}
          onChange={(e) => setTexto(e.target.value)}
          onKeyDown={(e) => {
            if (e.key === "Enter" || e.key === ",") {
              e.preventDefault();
              adicionar();
            }
          }}
          placeholder="Ex.: frango, alho, cebola"
          aria-label="Adicionar ingrediente"
          className="w-full rounded-xl border border-stone-300 bg-white px-4 py-3 shadow-sm outline-none transition placeholder:text-stone-400 focus:border-amber-500 focus:ring-2 focus:ring-amber-500/30 dark:border-stone-700 dark:bg-stone-900"
        />
        <button
          type="button"
          onClick={adicionar}
          className="rounded-xl bg-amber-600 px-5 py-3 font-semibold text-white shadow-sm transition hover:bg-amber-700 focus:outline-none focus:ring-2 focus:ring-amber-500/50"
        >
          Adicionar
        </button>
      </div>

      {ingredientes.length > 0 && (
        <ul className="flex flex-wrap gap-2">
          {ingredientes.map((i) => (
            <li
              key={i}
              className="flex items-center gap-1 rounded-full bg-amber-100 py-1 pl-3 pr-1 text-sm font-medium text-amber-900 dark:bg-amber-900/40 dark:text-amber-100"
            >
              {i}
              <button
                type="button"
                aria-label={`Remover ${i}`}
                onClick={() => onChange(ingredientes.filter((x) => x !== i))}
                className="flex h-6 w-6 items-center justify-center rounded-full text-base leading-none transition hover:bg-amber-200 dark:hover:bg-amber-800"
              >
                ×
              </button>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
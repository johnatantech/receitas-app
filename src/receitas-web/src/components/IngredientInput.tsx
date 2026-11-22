import { useState } from "react";

interface Props {
  ingredientes: string[];
  onChange: (ingredientes: string[]) => void;
}

export function IngredientInput({ ingredientes, onChange }: Props) {
  const [texto, setTexto] = useState("");

  function adicionar() {
    const valor = texto.trim().toLowerCase();
    if (valor && !ingredientes.includes(valor)) {
      onChange([...ingredientes, valor]);
    }
    setTexto("");
  }

  return (
    <div>
      <div style={{ display: "flex", gap: 8, flexWrap: "wrap", marginBottom: 8 }}>
        {ingredientes.map((i) => (
          <span
            key={i}
            style={{ background: "#e0e7ff", color: "#1e1b4b", padding: "4px 10px", borderRadius: 999 }}
          >
            {i}{" "}
      <button
  aria-label={`Remover ${i}`}
  onClick={() => onChange(ingredientes.filter((x) => x !== i))}
  style={{ border: "none", background: "none", cursor: "pointer", color: "#1e1b4b", fontSize: 16 }}
>
  ×
</button>
          </span>
        ))}
      </div>
      <input
        value={texto}
        onChange={(e) => setTexto(e.target.value)}
        onKeyDown={(e) => {
          if (e.key === "Enter" || e.key === ",") {
            e.preventDefault();
            adicionar();
          }
        }}
        placeholder="Digite um ingrediente (em inglês) e tecle Enter"
        style={{ padding: 8, width: "100%", maxWidth: 400 }}
      />
    </div>
  );
}
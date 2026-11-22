import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { buscarReceitas } from "./api/receitas";
import { IngredientInput } from "./components/IngredientInput";

export default function App() {
  const [ingredientes, setIngredientes] = useState<string[]>([]);

  const { data, isLoading, isError } = useQuery({
    queryKey: ["receitas", ingredientes],
    queryFn: () => buscarReceitas(ingredientes),
    enabled: ingredientes.length > 0,
  });

  return (
    <main style={{ maxWidth: 960, margin: "0 auto", padding: 24 }}>
      <h1>Receitas por ingredientes</h1>
      <IngredientInput ingredientes={ingredientes} onChange={setIngredientes} />

      {isLoading && <p>Buscando receitas...</p>}
      {isError && <p>Algo deu errado. Tente novamente.</p>}
      {data?.length === 0 && <p>Nenhuma receita encontrada com esses ingredientes.</p>}

      <div
        style={{
          display: "grid",
          gridTemplateColumns: "repeat(auto-fill, minmax(200px, 1fr))",
          gap: 16,
          marginTop: 24,
        }}
      >
        {data?.map((r) => (
          <article key={r.id} style={{ border: "1px solid #ddd", borderRadius: 8, overflow: "hidden" }}>
            <img src={r.imagem} alt={r.titulo} style={{ width: "100%", display: "block" }} />
            <h3 style={{ padding: "0 12px", fontSize: 16 }}>{r.titulo}</h3>
          </article>
        ))}
      </div>
    </main>
  );
}
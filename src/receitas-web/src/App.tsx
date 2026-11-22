import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { buscarReceitas } from "./api/receitas";
import { IngredientInput } from "./components/IngredientInput";
import { RecipeCard } from "./components/RecipeCard";

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
          <RecipeCard key={r.id} receita={r} />
        ))}
      </div>
    </main>
  );
}
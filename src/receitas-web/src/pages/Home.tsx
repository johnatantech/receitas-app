import { useQuery } from "@tanstack/react-query";
import { buscarReceitas } from "../api/receitas";
import { IngredientInput } from "../components/IngredientInput";
import { RecipeCard } from "../components/RecipeCard";
import { useIngredientes } from "../hooks/useIngredientes";

const SUGESTOES = ["frango", "alho", "cebola", "arroz", "ovo", "tomate", "batata"];

function Esqueleto() {
  return (
    <div className="animate-pulse overflow-hidden rounded-2xl border border-stone-200 bg-white dark:border-stone-800 dark:bg-stone-900">
      <div className="aspect-[4/3] bg-stone-200 dark:bg-stone-800" />
      <div className="space-y-3 p-4">
        <div className="h-4 w-3/4 rounded bg-stone-200 dark:bg-stone-800" />
        <div className="h-3 w-full rounded bg-stone-200 dark:bg-stone-800" />
      </div>
    </div>
  );
}

export default function Home() {
  const [ingredientes, setIngredientes] = useIngredientes();

  const { data, isLoading, isError } = useQuery({
    queryKey: ["receitas", ingredientes],
    queryFn: () => buscarReceitas(ingredientes),
    enabled: ingredientes.length > 0,
  });

  const grade = "grid gap-5 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4";

  return (
    <div className="space-y-8">
      <section className="space-y-2">
        <h1 className="text-3xl font-extrabold tracking-tight sm:text-4xl">
          O que tem na sua geladeira?
        </h1>
        <p className="text-stone-600 dark:text-stone-400">
          Informe os ingredientes e veja as receitas que você já consegue fazer.
        </p>
      </section>

      <section className="max-w-2xl">
        <IngredientInput ingredientes={ingredientes} onChange={setIngredientes} />
      </section>

      {ingredientes.length === 0 && (
        <section className="rounded-2xl border border-dashed border-stone-300 p-8 text-center dark:border-stone-700">
          <p className="mb-4 text-stone-600 dark:text-stone-400">
            Experimente começar por um destes:
          </p>
          <div className="flex flex-wrap justify-center gap-2">
            {SUGESTOES.map((s) => (
              <button
                key={s}
                type="button"
                onClick={() => setIngredientes([...ingredientes, s])}
                className="rounded-full border border-stone-300 px-4 py-1.5 text-sm font-medium transition hover:border-amber-500 hover:bg-amber-50 hover:text-amber-800 dark:border-stone-700 dark:hover:bg-amber-900/30 dark:hover:text-amber-100"
              >
                + {s}
              </button>
            ))}
          </div>
        </section>
      )}

      {isError && (
        <p
          role="alert"
          className="rounded-xl border border-rose-300 bg-rose-50 p-4 text-rose-800 dark:border-rose-900 dark:bg-rose-950 dark:text-rose-200"
        >
          Algo deu errado ao buscar as receitas. Confira se a API está rodando e
          tente novamente.
        </p>
      )}

      {isLoading && (
        <div className={grade}>
          {Array.from({ length: 8 }).map((_, i) => (
            <Esqueleto key={i} />
          ))}
        </div>
      )}

      {data?.length === 0 && (
        <p className="text-stone-600 dark:text-stone-400">
          Nenhuma receita encontrada com esses ingredientes. Tente remover algum.
        </p>
      )}

      {data && data.length > 0 && (
        <>
          <p className="text-sm text-stone-500">
            {data.length} receitas, das mais para as menos compatíveis
          </p>
          <div className={grade}>
            {data.map((r) => (
              <RecipeCard key={r.id} receita={r} />
            ))}
          </div>
        </>
      )}
    </div>
  );
}
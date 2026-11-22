import { Link, Route, Routes } from "react-router-dom";
import Home from "./pages/Home";
import RecipeDetail from "./pages/RecipeDetail";

export default function App() {
  return (
    <div className="min-h-screen">
      <header className="border-b border-stone-200 bg-white/80 backdrop-blur dark:border-stone-800 dark:bg-stone-900/80">
        <div className="mx-auto flex max-w-6xl items-center gap-2 px-4 py-4">
          <Link to="/" className="flex items-center gap-2 text-lg font-bold">
            <span aria-hidden>🍳</span>
            <span>
              Receitas<span className="text-amber-600"> por ingredientes</span>
            </span>
          </Link>
        </div>
      </header>

      <main className="mx-auto max-w-6xl px-4 py-8">
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/receita/:id" element={<RecipeDetail />} />
          <Route
            path="*"
            element={<p className="text-stone-500">Página não encontrada.</p>}
          />
        </Routes>
      </main>
    </div>
  );
}
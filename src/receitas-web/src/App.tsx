import { Route, Routes } from "react-router-dom";
import Home from "./pages/Home";
import RecipeDetail from "./pages/RecipeDetail";

export default function App() {
  return (
    <main style={{ maxWidth: 960, margin: "0 auto", padding: 24 }}>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/receita/:id" element={<RecipeDetail />} />
        <Route path="*" element={<p>Página não encontrada.</p>} />
      </Routes>
    </main>
  );
}
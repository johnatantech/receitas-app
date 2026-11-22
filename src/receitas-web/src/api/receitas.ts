import type { ReceitaRanqueada } from "../types/receita";

const API_URL = "http://localhost:5222";

export async function buscarReceitas(
  ingredientes: string[]
): Promise<ReceitaRanqueada[]> {
  const params = new URLSearchParams({ ingredientes: ingredientes.join(",") });
  const resposta = await fetch(`${API_URL}/api/receitas?${params}`);
  if (!resposta.ok) throw new Error("Erro ao buscar receitas");
  return resposta.json();
}
import type { ReceitaCompleta, ReceitaRanqueada } from "../types/receita";

const API_URL = "http://localhost:5222";

export async function buscarReceitas(
  ingredientes: string[]
): Promise<ReceitaRanqueada[]> {
  const params = new URLSearchParams({ ingredientes: ingredientes.join(",") });
  const resposta = await fetch(`${API_URL}/api/receitas?${params}`);
  if (!resposta.ok) throw new Error("Erro ao buscar receitas");
  return resposta.json();
}

export async function buscarReceita(id: string): Promise<ReceitaCompleta> {
  const resposta = await fetch(`${API_URL}/api/receitas/${encodeURIComponent(id)}`);
  if (resposta.status === 404) throw new Error("NAO_ENCONTRADA");
  if (!resposta.ok) throw new Error("Erro ao buscar a receita");
  return resposta.json();
}
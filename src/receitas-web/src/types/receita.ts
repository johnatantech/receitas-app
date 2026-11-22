export interface ReceitaRanqueada {
  id: string;
  titulo: string;
  imagem: string;
  compatibilidade: number;
  possui: string[];
  faltando: string[];
}
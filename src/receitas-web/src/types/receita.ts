export interface ReceitaRanqueada {
  id: string;
  titulo: string;
  imagem: string;
  compatibilidade: number;
  possui: string[];
  faltando: string[];
}

export interface IngredienteComMedida {
  nome: string;
  medida: string;
}

export interface ReceitaCompleta {
  id: string;
  titulo: string;
  imagem: string;
  categoria: string | null;
  area: string | null;
  instrucoes: string | null;
  video: string | null;
  ingredientes: IngredienteComMedida[];
}
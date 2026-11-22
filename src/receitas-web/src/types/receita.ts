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
  nomePt: string;
  medida: string;
  possui: boolean;
  basico: boolean;
}

export interface ReceitaCompleta {
  id: string;
  titulo: string;
  imagem: string;
  categoria: string | null;
  area: string | null;
  instrucoes: string | null;
  instrucoesPt: string | null;
  video: string | null;
  compatibilidade: number | null;
  ingredientes: IngredienteComMedida[];
}
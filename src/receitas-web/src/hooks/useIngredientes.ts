import { useSearchParams } from "react-router-dom";

export function useIngredientes() {
  const [params, setParams] = useSearchParams();

  const ingredientes = (params.get("ingredientes") ?? "")
    .split(",")
    .map((s) => s.trim())
    .filter(Boolean);

  function definir(novos: string[]) {
    if (novos.length === 0) setParams({}, { replace: true });
    else setParams({ ingredientes: novos.join(",") }, { replace: true });
  }

  return [ingredientes, definir] as const;
}
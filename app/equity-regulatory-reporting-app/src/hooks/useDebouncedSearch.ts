import { useState } from "react";
import { useDebounce } from "./useDebounce";
import { SEARCH_DEBOUNCE_MS } from "@/lib/constants";

export function useDebouncedSearch(delay: number = SEARCH_DEBOUNCE_MS) {
  const [search, setSearch] = useState("");
  const debouncedSearch = useDebounce(search, delay);

  return { search, setSearch, debouncedSearch };
}

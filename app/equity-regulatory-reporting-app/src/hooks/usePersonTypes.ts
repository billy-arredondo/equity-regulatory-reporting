import { useQuery } from "@tanstack/react-query";
import { getPersonTypes } from "@/api/person-types.api";

export const personTypeKeys = {
  all: ["person-types"] as const,
};

export function usePersonTypesQuery() {
  return useQuery({
    queryKey: personTypeKeys.all,
    queryFn: getPersonTypes,
    staleTime: Infinity,
  });
}

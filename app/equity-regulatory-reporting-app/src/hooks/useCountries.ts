import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";
import {
  createCountry,
  deleteCountry,
  getCountriesPaged,
  getCountryById,
  updateCountry,
} from "@/api/countries.api";
import type { CreateCountryDto, UpdateCountryDto } from "@/types/country";
import type { PageRequest } from "@/types/paged";

export const countryKeys = {
  all: ["countries"] as const,
  list: (req: PageRequest) => ["countries", "list", req] as const,
  detail: (id: string) => ["countries", id] as const,
};

export function useCountriesQuery(req: PageRequest) {
  return useQuery({
    queryKey: countryKeys.list(req),
    queryFn: () => getCountriesPaged(req),
  });
}

export function useCountryDetailQuery(id: string) {
  return useQuery({
    queryKey: countryKeys.detail(id),
    queryFn: () => getCountryById(id),
    enabled: !!id,
  });
}

export function useCreateCountryMutation() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: createCountry,
    onSuccess: () => {
      void qc.invalidateQueries({ queryKey: countryKeys.all });
      toast.success("País creado.");
    },
    onError: () => toast.error("Error al crear el país."),
  });
}

export function useUpdateCountryMutation() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, dto }: { id: string; dto: UpdateCountryDto }) =>
      updateCountry(id, dto),
    onSuccess: (_data, { id }) => {
      void qc.invalidateQueries({ queryKey: countryKeys.all });
      void qc.invalidateQueries({ queryKey: countryKeys.detail(id) });
      toast.success("País actualizado.");
    },
    onError: () => toast.error("Error al actualizar el país."),
  });
}

export function useDeleteCountryMutation() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: deleteCountry,
    onSuccess: (_data, id) => {
      qc.removeQueries({ queryKey: countryKeys.detail(id) });
      void qc.invalidateQueries({ queryKey: countryKeys.all });
      toast.success("País eliminado.");
    },
    onError: () => toast.error("Error al eliminar el país."),
  });
}

export function useCreateCountryDto(): CreateCountryDto {
  return { name: "", abbreviation: "" };
}

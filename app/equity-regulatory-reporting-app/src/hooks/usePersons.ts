import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";
import {
  createPerson,
  deletePerson,
  getPersonById,
  getPersonsPaged,
  updatePerson,
  type PersonsPageRequest,
} from "@/api/persons.api";
import type { UpdatePersonDto } from "@/types/person";

export const personKeys = {
  all: ["persons"] as const,
  list: (req: PersonsPageRequest) => ["persons", "list", req] as const,
  detail: (id: string) => ["persons", id] as const,
};

export function usePersonsQuery(req: PersonsPageRequest) {
  return useQuery({
    queryKey: personKeys.list(req),
    queryFn: () => getPersonsPaged(req),
  });
}

export function usePersonDetailQuery(id: string) {
  return useQuery({
    queryKey: personKeys.detail(id),
    queryFn: () => getPersonById(id),
    enabled: !!id,
  });
}

export function useCreatePersonMutation() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: createPerson,
    onSuccess: () => {
      void qc.invalidateQueries({ queryKey: personKeys.all });
      toast.success("Persona creada.");
    },
    onError: () => toast.error("Error al crear la persona."),
  });
}

export function useUpdatePersonMutation() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, dto }: { id: string; dto: UpdatePersonDto }) =>
      updatePerson(id, dto),
    onSuccess: (_data, { id }) => {
      void qc.invalidateQueries({ queryKey: personKeys.all });
      void qc.invalidateQueries({ queryKey: personKeys.detail(id) });
      toast.success("Persona actualizada.");
    },
    onError: () => toast.error("Error al actualizar la persona."),
  });
}

export function useDeletePersonMutation() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: deletePerson,
    onSuccess: (_data, id) => {
      qc.removeQueries({ queryKey: personKeys.detail(id) });
      void qc.invalidateQueries({ queryKey: personKeys.all });
      toast.success("Persona eliminada.");
    },
    onError: () => toast.error("Error al eliminar la persona."),
  });
}

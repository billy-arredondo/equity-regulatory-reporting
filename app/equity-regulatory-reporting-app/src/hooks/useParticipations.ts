import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";
import {
  createParticipation,
  deleteParticipation,
  getParticipationById,
  getParticipationsPaged,
  updateParticipation,
  type ParticipationsPageRequest,
} from "@/api/participations.api";
import type { UpdateParticipationDto } from "@/types/participation";

export const participationKeys = {
  all: ["participations"] as const,
  list: (req: ParticipationsPageRequest) => ["participations", "list", req] as const,
  detail: (id: string) => ["participations", id] as const,
};

export function useParticipationsQuery(req: ParticipationsPageRequest) {
  return useQuery({
    queryKey: participationKeys.list(req),
    queryFn: () => getParticipationsPaged(req),
  });
}

export function useParticipationDetailQuery(id: string) {
  return useQuery({
    queryKey: participationKeys.detail(id),
    queryFn: () => getParticipationById(id),
    enabled: !!id,
  });
}

export function useCreateParticipationMutation() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: createParticipation,
    onSuccess: () => {
      void qc.invalidateQueries({ queryKey: participationKeys.all });
      toast.success("Participación creada.");
    },
    onError: () => toast.error("Error al crear la participación."),
  });
}

export function useUpdateParticipationMutation() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, dto }: { id: string; dto: UpdateParticipationDto }) =>
      updateParticipation(id, dto),
    onSuccess: (_data, { id }) => {
      void qc.invalidateQueries({ queryKey: participationKeys.all });
      void qc.invalidateQueries({ queryKey: participationKeys.detail(id) });
      toast.success("Participación actualizada.");
    },
    onError: () => toast.error("Error al actualizar la participación."),
  });
}

export function useDeleteParticipationMutation() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: deleteParticipation,
    onSuccess: (_data, id) => {
      qc.removeQueries({ queryKey: participationKeys.detail(id) });
      void qc.invalidateQueries({ queryKey: participationKeys.all });
      toast.success("Participación eliminada.");
    },
    onError: () => toast.error("Error al eliminar la participación."),
  });
}

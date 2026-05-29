import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";
import {
  createPosition,
  deletePosition,
  getPositionById,
  getPositionsPaged,
  updatePosition,
} from "@/api/positions.api";
import type { CreatePositionDto, UpdatePositionDto } from "@/types/position";
import type { PageRequest } from "@/types/paged";

export const positionKeys = {
  all: ["positions"] as const,
  list: (req: PageRequest) => ["positions", "list", req] as const,
  detail: (id: string) => ["positions", id] as const,
};

export const NO_POSITION_NAME = "No position";

export function usePositionsQuery(req: PageRequest) {
  return useQuery({
    queryKey: positionKeys.list(req),
    queryFn: () => getPositionsPaged(req),
  });
}

export function usePositionDetailQuery(id: string) {
  return useQuery({
    queryKey: positionKeys.detail(id),
    queryFn: () => getPositionById(id),
    enabled: !!id,
  });
}

export function useCreatePositionMutation() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: createPosition,
    onSuccess: () => {
      void qc.invalidateQueries({ queryKey: positionKeys.all });
      toast.success("Cargo creado.");
    },
    onError: () => toast.error("Error al crear el cargo."),
  });
}

export function useUpdatePositionMutation() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, dto }: { id: string; dto: UpdatePositionDto }) =>
      updatePosition(id, dto),
    onSuccess: (_data, { id }) => {
      void qc.invalidateQueries({ queryKey: positionKeys.all });
      void qc.invalidateQueries({ queryKey: positionKeys.detail(id) });
      toast.success("Cargo actualizado.");
    },
    onError: () => toast.error("Error al actualizar el cargo."),
  });
}

export function useDeletePositionMutation() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: deletePosition,
    onSuccess: (_data, id) => {
      qc.removeQueries({ queryKey: positionKeys.detail(id) });
      void qc.invalidateQueries({ queryKey: positionKeys.all });
      toast.success("Cargo eliminado.");
    },
    onError: () => toast.error("Error al eliminar el cargo."),
  });
}

export function useCreatePositionDto(): CreatePositionDto {
  return { name: "" };
}

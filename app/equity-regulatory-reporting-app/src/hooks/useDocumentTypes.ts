import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";
import {
  createDocumentType,
  deleteDocumentType,
  getDocumentTypeById,
  getDocumentTypesPaged,
  updateDocumentType,
} from "@/api/document-types.api";
import type { UpdateDocumentTypeDto } from "@/types/document-type";
import type { PageRequest } from "@/types/paged";

export const documentTypeKeys = {
  all: ["document-types"] as const,
  list: (req: PageRequest) => ["document-types", "list", req] as const,
  detail: (id: string) => ["document-types", id] as const,
};

export function useDocumentTypesQuery(req: PageRequest) {
  return useQuery({
    queryKey: documentTypeKeys.list(req),
    queryFn: () => getDocumentTypesPaged(req),
  });
}

export function useDocumentTypeDetailQuery(id: string) {
  return useQuery({
    queryKey: documentTypeKeys.detail(id),
    queryFn: () => getDocumentTypeById(id),
    enabled: !!id,
  });
}

export function useCreateDocumentTypeMutation() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: createDocumentType,
    onSuccess: () => {
      void qc.invalidateQueries({ queryKey: documentTypeKeys.all });
      toast.success("Tipo de documento creado.");
    },
    onError: () => toast.error("Error al crear el tipo de documento."),
  });
}

export function useUpdateDocumentTypeMutation() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, dto }: { id: string; dto: UpdateDocumentTypeDto }) =>
      updateDocumentType(id, dto),
    onSuccess: (_data, { id }) => {
      void qc.invalidateQueries({ queryKey: documentTypeKeys.all });
      void qc.invalidateQueries({ queryKey: documentTypeKeys.detail(id) });
      toast.success("Tipo de documento actualizado.");
    },
    onError: () => toast.error("Error al actualizar el tipo de documento."),
  });
}

export function useDeleteDocumentTypeMutation() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: deleteDocumentType,
    onSuccess: (_data, id) => {
      qc.removeQueries({ queryKey: documentTypeKeys.detail(id) });
      void qc.invalidateQueries({ queryKey: documentTypeKeys.all });
      toast.success("Tipo de documento eliminado.");
    },
    onError: () => toast.error("Error al eliminar el tipo de documento."),
  });
}

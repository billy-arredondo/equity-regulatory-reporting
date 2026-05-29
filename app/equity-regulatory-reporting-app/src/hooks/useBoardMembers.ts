import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";
import {
  createBoardMember,
  deleteBoardMember,
  getBoardMemberById,
  getBoardMembersPaged,
  updateBoardMember,
  type BoardMembersPageRequest,
} from "@/api/board-members.api";
import type { UpdateBoardMemberDto } from "@/types/board-member";

export const boardMemberKeys = {
  all: ["boardMembers"] as const,
  list: (req: BoardMembersPageRequest) => ["boardMembers", "list", req] as const,
  detail: (id: string) => ["boardMembers", id] as const,
};

export function useBoardMembersQuery(req: BoardMembersPageRequest) {
  return useQuery({
    queryKey: boardMemberKeys.list(req),
    queryFn: () => getBoardMembersPaged(req),
  });
}

export function useBoardMemberDetailQuery(id: string) {
  return useQuery({
    queryKey: boardMemberKeys.detail(id),
    queryFn: () => getBoardMemberById(id),
    enabled: !!id,
  });
}

export function useCreateBoardMemberMutation() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: createBoardMember,
    onSuccess: () => {
      void qc.invalidateQueries({ queryKey: boardMemberKeys.all });
      toast.success("Miembro creado.");
    },
    onError: () => toast.error("Error al crear el miembro."),
  });
}

export function useUpdateBoardMemberMutation() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, dto }: { id: string; dto: UpdateBoardMemberDto }) =>
      updateBoardMember(id, dto),
    onSuccess: (_data, { id }) => {
      void qc.invalidateQueries({ queryKey: boardMemberKeys.all });
      void qc.invalidateQueries({ queryKey: boardMemberKeys.detail(id) });
      toast.success("Miembro actualizado.");
    },
    onError: () => toast.error("Error al actualizar el miembro."),
  });
}

export function useDeleteBoardMemberMutation() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: deleteBoardMember,
    onSuccess: (_data, id) => {
      qc.removeQueries({ queryKey: boardMemberKeys.detail(id) });
      void qc.invalidateQueries({ queryKey: boardMemberKeys.all });
      toast.success("Miembro eliminado.");
    },
    onError: () => toast.error("Error al eliminar el miembro."),
  });
}

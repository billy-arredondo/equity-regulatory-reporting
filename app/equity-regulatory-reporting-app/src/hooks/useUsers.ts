import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";
import {
  assignRole,
  deleteUser,
  getRoles,
  getUserById,
  getUsersPaged,
  updateUser,
} from "@/api/users.api";
import type { UpdateUserDto, AssignRoleDto } from "@/types/user";
import type { PageRequest } from "@/types/paged";

export const userKeys = {
  all: ["users"] as const,
  list: (req: PageRequest) => ["users", "list", req] as const,
  detail: (id: string) => ["users", id] as const,
  roles: ["users", "roles"] as const,
};

export function useUsersQuery(req: PageRequest) {
  return useQuery({
    queryKey: userKeys.list(req),
    queryFn: () => getUsersPaged(req),
  });
}

export function useUserDetailQuery(id: string) {
  return useQuery({
    queryKey: userKeys.detail(id),
    queryFn: () => getUserById(id),
    enabled: !!id,
  });
}

export function useRolesQuery() {
  return useQuery({
    queryKey: userKeys.roles,
    queryFn: getRoles,
    staleTime: Infinity,
  });
}

export function useUpdateUserMutation() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, dto }: { id: string; dto: UpdateUserDto }) => updateUser(id, dto),
    onSuccess: (_data, { id }) => {
      void qc.invalidateQueries({ queryKey: userKeys.all });
      void qc.invalidateQueries({ queryKey: userKeys.detail(id) });
      toast.success("Usuario actualizado.");
    },
    onError: () => toast.error("Error al actualizar el usuario."),
  });
}

export function useDeleteUserMutation() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: deleteUser,
    onSuccess: (_data, id) => {
      qc.removeQueries({ queryKey: userKeys.detail(id) });
      void qc.invalidateQueries({ queryKey: userKeys.all });
      toast.success("Usuario eliminado.");
    },
    onError: () => toast.error("Error al eliminar el usuario."),
  });
}

export function useAssignRoleMutation() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, dto }: { id: string; dto: AssignRoleDto }) => assignRole(id, dto),
    onSuccess: (_data, { id }) => {
      void qc.invalidateQueries({ queryKey: userKeys.detail(id) });
      void qc.invalidateQueries({ queryKey: userKeys.list({ page: 1, pageSize: 25 }) });
      toast.success("Rol asignado.");
    },
    onError: () => toast.error("Error al asignar el rol."),
  });
}

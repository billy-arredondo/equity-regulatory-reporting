import { useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { PageHeader } from "@/components/shared/PageHeader";
import { ConfirmDialog } from "@/components/shared/ConfirmDialog";
import { PageLoading } from "@/components/shared/LoadingSpinner";
import { PermissionGuard } from "@/components/shared/PermissionGuard";
import { RoleBadge } from "@/components/shared/RoleBadge";
import { AssignRoleDialog } from "./AssignRoleDialog";
import { Permission } from "@/lib/permissions";
import { useUserDetailQuery, useDeleteUserMutation } from "@/hooks/useUsers";

export function UserDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { data, isLoading } = useUserDetailQuery(id ?? "");
  const { mutate: remove, isPending } = useDeleteUserMutation();
  const [confirmOpen, setConfirmOpen] = useState(false);
  const [assignRoleOpen, setAssignRoleOpen] = useState(false);

  if (isLoading) return <PageLoading />;
  if (!data) return <p className="text-muted-foreground">Usuario no encontrado.</p>;

  return (
    <div>
      <Link
        to="/users"
        className="mb-5 inline-flex items-center gap-1 text-sm text-muted-foreground hover:text-foreground"
      >
        ← Volver a usuarios
      </Link>
      <PageHeader
        title={`${data.firstName} ${data.lastName}`}
        actions={
          <div className="flex gap-2">
            <PermissionGuard perm={Permission.UserWrite}>
              <Button variant="outline" size="sm" render={<Link to={`/users/${id}/edit`} />}>
                Editar
              </Button>
              <Button variant="outline" size="sm" onClick={() => setAssignRoleOpen(true)}>
                Asignar rol
              </Button>
            </PermissionGuard>
            <PermissionGuard perm={Permission.UserDelete}>
              <Button
                variant="destructive"
                size="sm"
                onClick={() => setConfirmOpen(true)}
                disabled={isPending}
              >
                Eliminar
              </Button>
            </PermissionGuard>
          </div>
        }
      />
      <div className="mt-4 max-w-lg grid grid-cols-2 gap-3">
        <div className="space-y-1.5">
          <p className="text-xs text-muted-foreground">Nombre</p>
          <p className="flex h-9 items-center text-sm font-medium">{data.firstName}</p>
        </div>
        <div className="space-y-1.5">
          <p className="text-xs text-muted-foreground">Apellido</p>
          <p className="flex h-9 items-center text-sm font-medium">{data.lastName}</p>
        </div>
        <div className="col-span-2 space-y-1.5">
          <p className="text-xs text-muted-foreground">Correo electrónico</p>
          <p className="flex h-9 items-center text-sm font-medium">{data.email}</p>
        </div>
        <div className="col-span-2 space-y-1.5">
          <p className="text-xs text-muted-foreground">Roles</p>
          <div className="flex flex-wrap gap-1.5 py-1">
            {data.roles.length === 0 ? (
              <span className="text-sm text-muted-foreground">Sin roles asignados</span>
            ) : (
              data.roles.map((role) => <RoleBadge key={role} role={role} />)
            )}
          </div>
        </div>
      </div>
      <ConfirmDialog
        open={confirmOpen}
        onOpenChange={setConfirmOpen}
        title="¿Eliminar usuario?"
        description={`Se eliminará la cuenta de "${data.email}". Esta acción no se puede deshacer.`}
        confirmLabel="Eliminar"
        onConfirm={() => {
          remove(id!, { onSuccess: () => void navigate("/users") });
          setConfirmOpen(false);
        }}
      />
      <AssignRoleDialog
        userId={id!}
        open={assignRoleOpen}
        onOpenChange={setAssignRoleOpen}
      />
    </div>
  );
}

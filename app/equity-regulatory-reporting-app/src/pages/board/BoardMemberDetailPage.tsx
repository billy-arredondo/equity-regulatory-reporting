import { useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { PageHeader } from "@/components/shared/PageHeader";
import { ConfirmDialog } from "@/components/shared/ConfirmDialog";
import { PageLoading } from "@/components/shared/LoadingSpinner";
import { PermissionGuard } from "@/components/shared/PermissionGuard";
import { Permission } from "@/lib/permissions";
import { useBoardMemberDetailQuery, useDeleteBoardMemberMutation } from "@/hooks/useBoardMembers";
import { personBaseRoute } from "@/lib/person-routes";

export function BoardMemberDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { data, isLoading } = useBoardMemberDetailQuery(id ?? "");
  const { mutate: remove, isPending } = useDeleteBoardMemberMutation();
  const [confirmOpen, setConfirmOpen] = useState(false);

  if (isLoading) return <PageLoading />;
  if (!data) return <p className="text-muted-foreground">Miembro no encontrado.</p>;

  return (
    <div>
      <Link
        to="/board"
        className="mb-5 inline-flex items-center gap-1 text-sm text-muted-foreground hover:text-foreground"
      >
        ← Volver a junta directiva
      </Link>
      <PageHeader
        title="Miembro de junta directiva"
        actions={
          <PermissionGuard perm={Permission.BoardWrite}>
            <div className="flex gap-2">
              <Button variant="outline" size="sm" render={<Link to={`/board/${id}/edit`} />}>
                Editar
              </Button>
              <PermissionGuard perm={Permission.BoardDelete}>
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
          </PermissionGuard>
        }
      />
      <div className="mt-4 max-w-lg grid grid-cols-2 gap-3">
        <div className="space-y-1.5">
          <p className="text-xs text-muted-foreground">Empresa</p>
          <p className="flex h-9 items-center text-sm font-medium">
            <Link to={`${personBaseRoute(data.companyPersonType)}/${data.companyId}`} className="hover:underline">
              {data.companyName}
            </Link>
          </p>
        </div>
        <div className="space-y-1.5">
          <p className="text-xs text-muted-foreground">Miembro</p>
          <p className="flex h-9 items-center text-sm font-medium">
            <Link to={`/people/${data.memberId}`} className="hover:underline">
              {data.memberName}
            </Link>
          </p>
        </div>
        <div className="space-y-1.5">
          <p className="text-xs text-muted-foreground">Cargo principal</p>
          <p className="flex h-9 items-center text-sm font-medium">{data.primaryPositionName}</p>
        </div>
        <div className="space-y-1.5">
          <p className="text-xs text-muted-foreground">Cargo secundario</p>
          <p className="flex h-9 items-center text-sm font-medium">{data.secondaryPositionName}</p>
        </div>
      </div>
      <ConfirmDialog
        open={confirmOpen}
        onOpenChange={setConfirmOpen}
        title="¿Eliminar miembro?"
        description={`Se eliminará a "${data.memberName}" de la junta de "${data.companyName}". Esta acción no se puede deshacer.`}
        confirmLabel="Eliminar"
        onConfirm={() => {
          remove(id!, { onSuccess: () => void navigate("/board") });
          setConfirmOpen(false);
        }}
      />
    </div>
  );
}

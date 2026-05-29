import { useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { PageHeader } from "@/components/shared/PageHeader";
import { ConfirmDialog } from "@/components/shared/ConfirmDialog";
import { PageLoading } from "@/components/shared/LoadingSpinner";
import { PermissionGuard } from "@/components/shared/PermissionGuard";
import { Permission } from "@/lib/permissions";
import {
  usePositionDetailQuery,
  useDeletePositionMutation,
  NO_POSITION_NAME,
} from "@/hooks/usePositions";

export function PositionDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { data, isLoading } = usePositionDetailQuery(id ?? "");
  const { mutate: remove, isPending } = useDeletePositionMutation();
  const [confirmOpen, setConfirmOpen] = useState(false);

  if (isLoading) return <PageLoading />;
  if (!data) return <p className="text-muted-foreground">Cargo no encontrado.</p>;

  const isProtected = data.name === NO_POSITION_NAME;

  return (
    <div>
      <Link
        to="/positions"
        className="mb-5 inline-flex items-center gap-1 text-sm text-muted-foreground hover:text-foreground"
      >
        ← Volver a cargos
      </Link>
      <PageHeader
        title={data.name}
        actions={
          <PermissionGuard perm={Permission.PositionWrite}>
            <div className="flex gap-2">
              {!isProtected && (
                <Button variant="outline" size="sm" render={<Link to={`/positions/${id}/edit`} />}>
                  Editar
                </Button>
              )}
              <PermissionGuard perm={Permission.PositionDelete}>
                {!isProtected && (
                  <Button
                    variant="destructive"
                    size="sm"
                    onClick={() => setConfirmOpen(true)}
                    disabled={isPending}
                  >
                    Eliminar
                  </Button>
                )}
              </PermissionGuard>
            </div>
          </PermissionGuard>
        }
      />
      <div className="mt-4 max-w-lg">
        <div className="space-y-1.5">
          <p className="text-xs text-muted-foreground">Nombre</p>
          <p className="flex h-9 items-center text-sm font-medium">{data.name}</p>
        </div>
      </div>
      <ConfirmDialog
        open={confirmOpen}
        onOpenChange={setConfirmOpen}
        title="¿Eliminar cargo?"
        description={`Se eliminará "${data.name}". Esta acción no se puede deshacer.`}
        confirmLabel="Eliminar"
        onConfirm={() => {
          remove(id!, { onSuccess: () => void navigate("/positions") });
          setConfirmOpen(false);
        }}
      />
    </div>
  );
}

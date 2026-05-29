import { useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { PageHeader } from "@/components/shared/PageHeader";
import { ConfirmDialog } from "@/components/shared/ConfirmDialog";
import { PageLoading } from "@/components/shared/LoadingSpinner";
import { PermissionGuard } from "@/components/shared/PermissionGuard";
import { Permission } from "@/lib/permissions";
import { useParticipationDetailQuery, useDeleteParticipationMutation } from "@/hooks/useParticipations";

function formatDate(d: string) {
  return new Date(d + "T00:00:00").toLocaleDateString("es-CO", {
    year: "numeric",
    month: "long",
    day: "numeric",
  });
}

export function ParticipationDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { data, isLoading } = useParticipationDetailQuery(id ?? "");
  const { mutate: remove, isPending } = useDeleteParticipationMutation();
  const [confirmOpen, setConfirmOpen] = useState(false);

  if (isLoading) return <PageLoading />;
  if (!data) return <p className="text-muted-foreground">Participación no encontrada.</p>;

  return (
    <div>
      <Link
        to="/participations"
        className="mb-5 inline-flex items-center gap-1 text-sm text-muted-foreground hover:text-foreground"
      >
        ← Volver a participaciones
      </Link>
      <PageHeader
        title="Participación accionaria"
        actions={
          <PermissionGuard perm={Permission.ParticipationWrite}>
            <div className="flex gap-2">
              <Button variant="outline" size="sm" render={<Link to={`/participations/${id}/edit`} />}>
                Editar
              </Button>
              <PermissionGuard perm={Permission.ParticipationDelete}>
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
            <Link to={`/persons/${data.companyId}`} className="hover:underline">
              {data.companyName}
            </Link>
          </p>
        </div>
        <div className="space-y-1.5">
          <p className="text-xs text-muted-foreground">Accionista</p>
          <p className="flex h-9 items-center text-sm font-medium">
            <Link to={`/persons/${data.shareholderId}`} className="hover:underline">
              {data.shareholderName}
            </Link>
          </p>
        </div>
        <div className="space-y-1.5">
          <p className="text-xs text-muted-foreground">Porcentaje</p>
          <p className="flex h-9 items-center font-mono text-sm font-medium">{data.percentage}%</p>
        </div>
        <div className="space-y-1.5">
          <p className="text-xs text-muted-foreground">Vigente</p>
          <p className="flex h-9 items-center text-sm font-medium">
            {data.effectiveTo ? "No" : "Sí"}
          </p>
        </div>
        <div className="space-y-1.5">
          <p className="text-xs text-muted-foreground">Desde</p>
          <p className="flex h-9 items-center text-sm font-medium">{formatDate(data.effectiveFrom)}</p>
        </div>
        {data.effectiveTo && (
          <div className="space-y-1.5">
            <p className="text-xs text-muted-foreground">Hasta</p>
            <p className="flex h-9 items-center text-sm font-medium">{formatDate(data.effectiveTo)}</p>
          </div>
        )}
      </div>
      <ConfirmDialog
        open={confirmOpen}
        onOpenChange={setConfirmOpen}
        title="¿Eliminar participación?"
        description={`Se eliminará la participación de "${data.shareholderName}" en "${data.companyName}". Esta acción no se puede deshacer.`}
        confirmLabel="Eliminar"
        onConfirm={() => {
          remove(id!, { onSuccess: () => void navigate("/participations") });
          setConfirmOpen(false);
        }}
      />
    </div>
  );
}

import { useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { PageHeader } from "@/components/shared/PageHeader";
import { ConfirmDialog } from "@/components/shared/ConfirmDialog";
import { PageLoading } from "@/components/shared/LoadingSpinner";
import { PermissionGuard } from "@/components/shared/PermissionGuard";
import { Permission } from "@/lib/permissions";
import { personTypeLabel } from "@/lib/person-types";
import { usePersonDetailQuery, useDeletePersonMutation } from "@/hooks/usePersons";

export function PersonDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { data, isLoading } = usePersonDetailQuery(id ?? "");
  const { mutate: remove, isPending } = useDeletePersonMutation();
  const [confirmOpen, setConfirmOpen] = useState(false);

  if (isLoading) return <PageLoading />;
  if (!data) return <p className="text-muted-foreground">Persona no encontrada.</p>;

  return (
    <div>
      <Link
        to="/persons"
        className="mb-5 inline-flex items-center gap-1 text-sm text-muted-foreground hover:text-foreground"
      >
        ← Volver a personas
      </Link>
      <PageHeader
        title={data.name}
        actions={
          <PermissionGuard perm={Permission.PersonWrite}>
            <div className="flex gap-2">
              <Button variant="outline" size="sm" render={<Link to={`/persons/${id}/edit`} />}>
                Editar
              </Button>
              <PermissionGuard perm={Permission.PersonDelete}>
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
          <p className="text-xs text-muted-foreground">Tipo de persona</p>
          <p className="flex h-9 items-center text-sm font-medium">{personTypeLabel(data.personType)}</p>
        </div>
        <div className="space-y-1.5">
          <p className="text-xs text-muted-foreground">CIIU</p>
          <p className="flex h-9 items-center font-mono text-sm font-medium">{data.ciiu}</p>
        </div>
        <div className="space-y-1.5">
          <p className="text-xs text-muted-foreground">Tipo de documento</p>
          <p className="flex h-9 items-center text-sm font-medium">{data.documentTypeName}</p>
        </div>
        <div className="space-y-1.5">
          <p className="text-xs text-muted-foreground">Número de documento</p>
          <p className="flex h-9 items-center text-sm font-medium">{data.documentNumber}</p>
        </div>
        <div className="col-span-2 space-y-1.5">
          <p className="text-xs text-muted-foreground">Dirección</p>
          <p className="flex h-9 items-center text-sm font-medium">{data.address}</p>
        </div>
        <div className="space-y-1.5">
          <p className="text-xs text-muted-foreground">País</p>
          <p className="flex h-9 items-center text-sm font-medium">{data.countryName}</p>
        </div>
        <div className="space-y-1.5">
          <p className="text-xs text-muted-foreground">Ubicación interna</p>
          <p className="flex h-9 items-center text-sm font-medium">{data.internalLocation}</p>
        </div>
        {data.entityCode && (
          <div className="space-y-1.5">
            <p className="text-xs text-muted-foreground">Código entidad</p>
            <p className="flex h-9 items-center text-sm font-medium">{data.entityCode}</p>
          </div>
        )}
        {data.representativeName && (
          <div className="space-y-1.5">
            <p className="text-xs text-muted-foreground">Representante</p>
            <p className="flex h-9 items-center text-sm">
              <Link
                to={`/persons/${data.representativeId}`}
                className="font-medium hover:underline"
              >
                {data.representativeName}
              </Link>
            </p>
          </div>
        )}
        <div className="space-y-1.5">
          <p className="text-xs text-muted-foreground">Incluir en reporte</p>
          <div className="flex h-9 items-center">
            <Badge variant={data.reportFlag ? "default" : "secondary"}>
              {data.reportFlag ? "Sí" : "No"}
            </Badge>
          </div>
        </div>
      </div>
      <ConfirmDialog
        open={confirmOpen}
        onOpenChange={setConfirmOpen}
        title="¿Eliminar persona?"
        description={`Se eliminará "${data.name}". Esta acción no se puede deshacer.`}
        confirmLabel="Eliminar"
        onConfirm={() => {
          remove(id!, { onSuccess: () => void navigate("/persons") });
          setConfirmOpen(false);
        }}
      />
    </div>
  );
}

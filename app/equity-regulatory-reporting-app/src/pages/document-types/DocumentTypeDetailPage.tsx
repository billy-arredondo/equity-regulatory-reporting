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
import {
  useDocumentTypeDetailQuery,
  useDeleteDocumentTypeMutation,
} from "@/hooks/useDocumentTypes";

export function DocumentTypeDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { data, isLoading } = useDocumentTypeDetailQuery(id ?? "");
  const { mutate: remove, isPending } = useDeleteDocumentTypeMutation();
  const [confirmOpen, setConfirmOpen] = useState(false);

  if (isLoading) return <PageLoading />;
  if (!data) return <p className="text-muted-foreground">Tipo de documento no encontrado.</p>;

  return (
    <div>
      <Link
        to="/document-types"
        className="mb-5 inline-flex items-center gap-1 text-sm text-muted-foreground hover:text-foreground"
      >
        ← Volver a tipos de documento
      </Link>
      <PageHeader
        title={data.name}
        actions={
          <PermissionGuard perm={Permission.DocumentTypeWrite}>
            <div className="flex gap-2">
              <Button variant="outline" size="sm" render={<Link to={`/document-types/${id}/edit`} />}>
                Editar
              </Button>
              <PermissionGuard perm={Permission.DocumentTypeDelete}>
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
          <p className="text-xs text-muted-foreground">Nombre</p>
          <p className="flex h-9 items-center text-sm font-medium">{data.name}</p>
        </div>
        <div className="space-y-1.5">
          <p className="text-xs text-muted-foreground">Abreviatura</p>
          <p className="flex h-9 items-center text-sm font-medium">{data.abbreviation}</p>
        </div>
        <div className="col-span-2 space-y-1.5">
          <p className="text-xs text-muted-foreground">Validación (regex)</p>
          <p className="flex h-9 items-center font-mono text-sm">
            {data.validationRegex ?? "—"}
          </p>
        </div>
        <div className="col-span-2 space-y-1.5">
          <p className="text-xs text-muted-foreground">Tipos de persona habilitados</p>
          <div className="flex flex-wrap gap-1 pt-1">
            {data.allowedPersonTypes.map((t) => (
              <Badge key={t} variant="secondary">{personTypeLabel(t)}</Badge>
            ))}
          </div>
        </div>
      </div>
      <ConfirmDialog
        open={confirmOpen}
        onOpenChange={setConfirmOpen}
        title="¿Eliminar tipo de documento?"
        description={`Se eliminará "${data.name}". Esta acción no se puede deshacer.`}
        confirmLabel="Eliminar"
        onConfirm={() => {
          remove(id!, { onSuccess: () => void navigate("/document-types") });
          setConfirmOpen(false);
        }}
      />
    </div>
  );
}

import { useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { PageHeader } from "@/components/shared/PageHeader";
import { ConfirmDialog } from "@/components/shared/ConfirmDialog";
import { PageLoading } from "@/components/shared/LoadingSpinner";
import { PermissionGuard } from "@/components/shared/PermissionGuard";
import { Permission } from "@/lib/permissions";
import {
  useCountryDetailQuery,
  useDeleteCountryMutation,
} from "@/hooks/useCountries";

export function CountryDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { data, isLoading } = useCountryDetailQuery(id ?? "");
  const { mutate: remove, isPending } = useDeleteCountryMutation();
  const [confirmOpen, setConfirmOpen] = useState(false);

  if (isLoading) return <PageLoading />;
  if (!data) return <p className="text-muted-foreground">País no encontrado.</p>;

  return (
    <div>
      <Link
        to="/countries"
        className="mb-5 inline-flex items-center gap-1 text-sm text-muted-foreground hover:text-foreground"
      >
        ← Volver a países
      </Link>
      <PageHeader
        title={data.name}
        actions={
          <PermissionGuard perm={Permission.CountryWrite}>
            <div className="flex gap-2">
              <Button variant="outline" size="sm" render={<Link to={`/countries/${id}/edit`} />}>
                Editar
              </Button>
              <PermissionGuard perm={Permission.CountryDelete}>
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
      </div>
      <ConfirmDialog
        open={confirmOpen}
        onOpenChange={setConfirmOpen}
        title="¿Eliminar país?"
        description={`Se eliminará "${data.name}". Esta acción no se puede deshacer.`}
        confirmLabel="Eliminar"
        onConfirm={() => {
          remove(id!, { onSuccess: () => void navigate("/countries") });
          setConfirmOpen(false);
        }}
      />
    </div>
  );
}

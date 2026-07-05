import { useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { PageHeader } from "@/components/shared/PageHeader";
import { ConfirmDialog } from "@/components/shared/ConfirmDialog";
import { PageLoading } from "@/components/shared/LoadingSpinner";
import { PermissionGuard } from "@/components/shared/PermissionGuard";
import { Permission } from "@/lib/permissions";
import { usePersonDetailQuery, useDeletePersonMutation } from "@/hooks/usePersons";
import { PersonDetailFields } from "./PersonDetailFields";

interface Props {
  baseRoute: string;
}

export function PersonDetailPage({ baseRoute }: Props) {
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
        to={baseRoute}
        className="mb-5 inline-flex items-center gap-1 text-sm text-muted-foreground hover:text-foreground"
      >
        ← Volver
      </Link>
      <PageHeader
        title={data.name}
        actions={
          <PermissionGuard perm={Permission.PersonWrite}>
            <div className="flex gap-2">
              <Button variant="outline" size="sm" render={<Link to={`${baseRoute}/${id}/edit`} />}>
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
      <PersonDetailFields data={data} />
      <ConfirmDialog
        open={confirmOpen}
        onOpenChange={setConfirmOpen}
        title="¿Eliminar persona?"
        description={`Se eliminará "${data.name}". Esta acción no se puede deshacer.`}
        confirmLabel="Eliminar"
        onConfirm={() => {
          remove(id!, { onSuccess: () => void navigate(baseRoute) });
          setConfirmOpen(false);
        }}
      />
    </div>
  );
}

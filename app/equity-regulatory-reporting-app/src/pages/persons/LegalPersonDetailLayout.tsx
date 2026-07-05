import { useState } from "react";
import { Link, NavLink, Route, Routes, useNavigate, useParams } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { PageHeader } from "@/components/shared/PageHeader";
import { ConfirmDialog } from "@/components/shared/ConfirmDialog";
import { PageLoading } from "@/components/shared/LoadingSpinner";
import { PermissionGuard } from "@/components/shared/PermissionGuard";
import { Permission } from "@/lib/permissions";
import { cn } from "@/lib/utils";
import { usePersonDetailQuery, useDeletePersonMutation } from "@/hooks/usePersons";
import { PersonDetailFields } from "./PersonDetailFields";
import { PersonParticipationsSection } from "@/pages/participations";
import { PersonBoardSection } from "@/pages/board";

interface Props {
  baseRoute: string;
}

function tabClass({ isActive }: { isActive: boolean }) {
  return cn(
    "-mb-px border-b-2 px-4 py-2 text-sm font-medium transition-colors",
    isActive
      ? "border-primary text-foreground"
      : "border-transparent text-muted-foreground hover:border-muted-foreground/40 hover:text-foreground",
  );
}

export function LegalPersonDetailLayout({ baseRoute }: Props) {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { data, isLoading } = usePersonDetailQuery(id ?? "");
  const { mutate: remove, isPending } = useDeletePersonMutation();
  const [confirmOpen, setConfirmOpen] = useState(false);

  const basePath = `${baseRoute}/${id}`;

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
              <Button variant="outline" size="sm" render={<Link to={`${basePath}/edit`} />}>
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

      {/* Tab strip */}
      <div className="mt-4 mb-6 flex border-b">
        <NavLink to={basePath} end className={tabClass}>
          General
        </NavLink>
        <PermissionGuard perm={Permission.ParticipationRead}>
          <NavLink to={`${basePath}/participations`} className={tabClass}>
            Participaciones
          </NavLink>
        </PermissionGuard>
        <PermissionGuard perm={Permission.BoardRead}>
          <NavLink to={`${basePath}/board`} className={tabClass}>
            Junta directiva
          </NavLink>
        </PermissionGuard>
      </div>

      {/* Tab content */}
      <Routes>
        <Route index element={<PersonDetailFields data={data} />} />
        <Route
          path="participations/*"
          element={
            <PersonParticipationsSection
              companyId={id!}
              companyName={data.name}
              basePath={`${basePath}/participations`}
            />
          }
        />
        <Route
          path="board/*"
          element={
            <PersonBoardSection
              companyId={id!}
              companyName={data.name}
              basePath={`${basePath}/board`}
            />
          }
        />
      </Routes>

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

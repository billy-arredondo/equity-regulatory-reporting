import { Navigate, Route, Routes } from "react-router-dom";
import { AppLayout } from "@/components/shared/AppLayout";
import { AuthGuard } from "@/components/shared/AuthGuard";
import { PermissionGuard } from "@/components/shared/PermissionGuard";
import { Permission } from "@/lib/permissions";
import { LoginPage } from "@/pages/auth/LoginPage";
import { CountriesPage } from "@/pages/countries";
import { PersonsPage } from "@/pages/persons";
import { PeoplePage } from "@/pages/people";
import { CompaniesPage } from "@/pages/companies";
import { EntitiesPage } from "@/pages/entities";
import { PersonTypesPage } from "@/pages/person-types";
import { DocumentTypesPage } from "@/pages/document-types";
import { ParticipationsPage } from "@/pages/participations";
import { BoardPage } from "@/pages/board";
import { PositionsPage } from "@/pages/positions";
import { UsersPage } from "@/pages/users";

function NotFound() {
  return (
    <div className="flex h-40 flex-col items-center justify-center gap-2 text-muted-foreground">
      <p className="text-lg font-medium">Página no encontrada</p>
      <p className="text-sm">La ruta solicitada no existe.</p>
    </div>
  );
}

export function AppRouter() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />

      <Route element={<AuthGuard />}>
        <Route element={<AppLayout />}>
          <Route index element={<Navigate to="/people" replace />} />
          <Route path="people/*" element={<PeoplePage />} />
          <Route path="companies/*" element={<CompaniesPage />} />
          <Route path="entities/*" element={<EntitiesPage />} />
          <Route path="persons/*" element={<PersonsPage />} />
          <Route path="person-types" element={<PersonTypesPage />} />
          <Route path="document-types/*" element={<DocumentTypesPage />} />
          <Route path="countries/*" element={<CountriesPage />} />
          <Route path="participations/*" element={<ParticipationsPage />} />
          <Route path="board/*" element={<BoardPage />} />
          <Route path="positions/*" element={<PositionsPage />} />
          <Route
            path="users/*"
            element={
              <PermissionGuard
                perm={Permission.UserRead}
                fallback={
                  <div className="flex h-40 items-center justify-center text-muted-foreground">
                    No tienes permiso para ver esta sección.
                  </div>
                }
              >
                <UsersPage />
              </PermissionGuard>
            }
          />
          <Route path="*" element={<NotFound />} />
        </Route>
      </Route>
    </Routes>
  );
}

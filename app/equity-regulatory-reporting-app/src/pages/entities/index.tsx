import { Route, Routes } from "react-router-dom";
import { LegalEntityListPage } from "./LegalEntityListPage";
import { LegalEntityFormPage } from "./LegalEntityFormPage";
import { PersonDetailPage } from "@/pages/persons/PersonDetailPage";

export function EntitiesPage() {
  return (
    <Routes>
      <Route index element={<LegalEntityListPage />} />
      <Route path="new" element={<LegalEntityFormPage />} />
      <Route path=":id" element={<PersonDetailPage baseRoute="/entities" />} />
      <Route path=":id/edit" element={<LegalEntityFormPage />} />
    </Routes>
  );
}

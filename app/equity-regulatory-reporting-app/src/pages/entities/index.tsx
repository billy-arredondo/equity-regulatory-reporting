import { Route, Routes } from "react-router-dom";
import { LegalEntityListPage } from "./LegalEntityListPage";
import { LegalEntityFormPage } from "./LegalEntityFormPage";
import { LegalPersonDetailLayout } from "@/pages/persons/LegalPersonDetailLayout";

export function EntitiesPage() {
  return (
    <Routes>
      <Route index element={<LegalEntityListPage />} />
      <Route path="new" element={<LegalEntityFormPage />} />
      <Route path=":id/edit" element={<LegalEntityFormPage />} />
      <Route path=":id/*" element={<LegalPersonDetailLayout baseRoute="/entities" />} />
    </Routes>
  );
}

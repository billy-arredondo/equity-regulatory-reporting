import { Route, Routes } from "react-router-dom";
import { LegalPersonListPage } from "./LegalPersonListPage";
import { LegalPersonFormPage } from "./LegalPersonFormPage";
import { LegalPersonDetailLayout } from "@/pages/persons/LegalPersonDetailLayout";

export function CompaniesPage() {
  return (
    <Routes>
      <Route index element={<LegalPersonListPage />} />
      <Route path="new" element={<LegalPersonFormPage />} />
      <Route path=":id/edit" element={<LegalPersonFormPage />} />
      <Route path=":id/*" element={<LegalPersonDetailLayout baseRoute="/companies" />} />
    </Routes>
  );
}

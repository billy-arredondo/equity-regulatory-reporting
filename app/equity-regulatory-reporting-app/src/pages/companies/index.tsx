import { Route, Routes } from "react-router-dom";
import { LegalPersonListPage } from "./LegalPersonListPage";
import { LegalPersonFormPage } from "./LegalPersonFormPage";
import { PersonDetailPage } from "@/pages/persons/PersonDetailPage";

export function CompaniesPage() {
  return (
    <Routes>
      <Route index element={<LegalPersonListPage />} />
      <Route path="new" element={<LegalPersonFormPage />} />
      <Route path=":id" element={<PersonDetailPage baseRoute="/companies" />} />
      <Route path=":id/edit" element={<LegalPersonFormPage />} />
    </Routes>
  );
}

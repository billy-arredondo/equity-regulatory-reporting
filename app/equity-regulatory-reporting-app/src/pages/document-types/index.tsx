import { Route, Routes } from "react-router-dom";
import { DocumentTypeListPage } from "./DocumentTypeListPage";
import { DocumentTypeDetailPage } from "./DocumentTypeDetailPage";
import { DocumentTypeFormPage } from "./DocumentTypeFormPage";

export function DocumentTypesPage() {
  return (
    <Routes>
      <Route index element={<DocumentTypeListPage />} />
      <Route path="new" element={<DocumentTypeFormPage />} />
      <Route path=":id" element={<DocumentTypeDetailPage />} />
      <Route path=":id/edit" element={<DocumentTypeFormPage />} />
    </Routes>
  );
}

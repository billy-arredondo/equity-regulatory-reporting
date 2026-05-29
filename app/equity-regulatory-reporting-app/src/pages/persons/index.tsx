import { Route, Routes } from "react-router-dom";
import { PersonListPage } from "./PersonListPage";
import { PersonDetailPage } from "./PersonDetailPage";
import { PersonFormPage } from "./PersonFormPage";

export function PersonsPage() {
  return (
    <Routes>
      <Route index element={<PersonListPage />} />
      <Route path="new" element={<PersonFormPage />} />
      <Route path=":id" element={<PersonDetailPage />} />
      <Route path=":id/edit" element={<PersonFormPage />} />
    </Routes>
  );
}

import { Route, Routes } from "react-router-dom";
import { NaturalPersonListPage } from "./NaturalPersonListPage";
import { NaturalPersonFormPage } from "./NaturalPersonFormPage";
import { PersonDetailPage } from "@/pages/persons/PersonDetailPage";

export function PeoplePage() {
  return (
    <Routes>
      <Route index element={<NaturalPersonListPage />} />
      <Route path="new" element={<NaturalPersonFormPage />} />
      <Route path=":id" element={<PersonDetailPage baseRoute="/people" />} />
      <Route path=":id/edit" element={<NaturalPersonFormPage />} />
    </Routes>
  );
}

import { Route, Routes } from "react-router-dom";
import { ParticipationListPage } from "./ParticipationListPage";
import { ParticipationDetailPage } from "./ParticipationDetailPage";
import { ParticipationFormPage } from "./ParticipationFormPage";

export function ParticipationsPage() {
  return (
    <Routes>
      <Route index element={<ParticipationListPage />} />
      <Route path="new" element={<ParticipationFormPage />} />
      <Route path=":id" element={<ParticipationDetailPage />} />
      <Route path=":id/edit" element={<ParticipationFormPage />} />
    </Routes>
  );
}

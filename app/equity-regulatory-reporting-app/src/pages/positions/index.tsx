import { Route, Routes } from "react-router-dom";
import { PositionListPage } from "./PositionListPage";
import { PositionDetailPage } from "./PositionDetailPage";
import { PositionFormPage } from "./PositionFormPage";

export function PositionsPage() {
  return (
    <Routes>
      <Route index element={<PositionListPage />} />
      <Route path="new" element={<PositionFormPage />} />
      <Route path=":id" element={<PositionDetailPage />} />
      <Route path=":id/edit" element={<PositionFormPage />} />
    </Routes>
  );
}

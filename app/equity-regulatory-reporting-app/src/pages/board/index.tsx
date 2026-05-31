import { Route, Routes } from "react-router-dom";
import { BoardMemberListPage } from "./BoardMemberListPage";
import { BoardMemberDetailPage } from "./BoardMemberDetailPage";
import { BoardMemberFormPage } from "./BoardMemberFormPage";

export function BoardPage() {
  return (
    <Routes>
      <Route index element={<BoardMemberListPage />} />
      <Route path="new" element={<BoardMemberFormPage />} />
      <Route path=":id" element={<BoardMemberDetailPage />} />
      <Route path=":id/edit" element={<BoardMemberFormPage />} />
    </Routes>
  );
}

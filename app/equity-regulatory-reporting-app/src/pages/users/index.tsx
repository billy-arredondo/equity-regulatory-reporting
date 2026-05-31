import { Route, Routes } from "react-router-dom";
import { UserListPage } from "./UserListPage";
import { UserDetailPage } from "./UserDetailPage";
import { UserFormPage } from "./UserFormPage";

export function UsersPage() {
  return (
    <Routes>
      <Route index element={<UserListPage />} />
      <Route path=":id" element={<UserDetailPage />} />
      <Route path=":id/edit" element={<UserFormPage />} />
    </Routes>
  );
}

import { Navigate, Route, Routes } from "react-router-dom";

// /persons is kept only as a fallback redirect to /people.
// All person views now live under /people, /companies, /entities.
export function PersonsPage() {
  return (
    <Routes>
      <Route index element={<Navigate to="/people" replace />} />
      <Route path="*" element={<Navigate to="/people" replace />} />
    </Routes>
  );
}

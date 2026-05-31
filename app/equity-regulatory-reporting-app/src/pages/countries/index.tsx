import { Route, Routes } from "react-router-dom";
import { CountryListPage } from "./CountryListPage";
import { CountryDetailPage } from "./CountryDetailPage";
import { CountryFormPage } from "./CountryFormPage";

export function CountriesPage() {
  return (
    <Routes>
      <Route index element={<CountryListPage />} />
      <Route path="new" element={<CountryFormPage />} />
      <Route path=":id" element={<CountryDetailPage />} />
      <Route path=":id/edit" element={<CountryFormPage />} />
    </Routes>
  );
}

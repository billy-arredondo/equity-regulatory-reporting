import { PersonType } from "@/lib/person-types";
import { PersonListPage } from "@/pages/persons/PersonListPage";

export function LegalPersonListPage() {
  return (
    <PersonListPage
      personType={PersonType.Legal}
      title="Personas Jurídicas"
      baseRoute="/companies"
      newLabel="Nueva persona jurídica"
    />
  );
}

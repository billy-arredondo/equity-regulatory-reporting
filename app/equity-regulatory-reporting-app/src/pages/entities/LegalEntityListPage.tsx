import { PersonType } from "@/lib/person-types";
import { PersonListPage } from "@/pages/persons/PersonListPage";

export function LegalEntityListPage() {
  return (
    <PersonListPage
      personType={PersonType.LegalEntity}
      title="Entes Jurídicos"
      baseRoute="/entities"
      newLabel="Nuevo ente jurídico"
    />
  );
}

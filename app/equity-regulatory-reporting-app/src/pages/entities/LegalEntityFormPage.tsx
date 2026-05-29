import { PersonType } from "@/lib/person-types";
import { PersonFormPage } from "@/pages/persons/PersonFormPage";

export function LegalEntityFormPage() {
  return (
    <PersonFormPage
      personType={PersonType.LegalEntity}
      baseRoute="/entities"
      entityLabel="ente jurídico"
    />
  );
}

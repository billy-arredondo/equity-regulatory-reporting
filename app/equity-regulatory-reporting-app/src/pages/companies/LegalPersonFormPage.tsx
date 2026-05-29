import { PersonType } from "@/lib/person-types";
import { PersonFormPage } from "@/pages/persons/PersonFormPage";

export function LegalPersonFormPage() {
  return (
    <PersonFormPage
      personType={PersonType.Legal}
      baseRoute="/companies"
      entityLabel="persona jurídica"
    />
  );
}

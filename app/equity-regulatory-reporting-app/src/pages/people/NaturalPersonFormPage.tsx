import { PersonType } from "@/lib/person-types";
import { PersonFormPage } from "@/pages/persons/PersonFormPage";

export function NaturalPersonFormPage() {
  return (
    <PersonFormPage
      personType={PersonType.Natural}
      baseRoute="/people"
      entityLabel="persona natural"
    />
  );
}

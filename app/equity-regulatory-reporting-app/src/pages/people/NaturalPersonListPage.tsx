import { PersonType } from "@/lib/person-types";
import { PersonListPage } from "@/pages/persons/PersonListPage";

export function NaturalPersonListPage() {
  return (
    <PersonListPage
      personType={PersonType.Natural}
      title="Personas Naturales"
      baseRoute="/people"
      newLabel="Nueva persona natural"
    />
  );
}

import { PersonType, type PersonTypeValue } from "@/lib/person-types";

export function personBaseRoute(type: PersonTypeValue): string {
  if (type === PersonType.Natural) return "/people";
  if (type === PersonType.Legal) return "/companies";
  return "/entities";
}

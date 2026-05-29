import http from "@/lib/http";
import type { PersonTypeDto } from "@/types/person-type";

export function getPersonTypes(): Promise<PersonTypeDto[]> {
  return http("/api/PersonTypes");
}

import http from "@/lib/http";
import type { PagedResult, PageRequest } from "@/types/paged";
import type { PersonDto, PersonDetailDto, CreatePersonDto, UpdatePersonDto } from "@/types/person";
import type { PersonTypeValue } from "@/lib/person-types";

const BASE = "/api/Persons";

export interface PersonsPageRequest extends PageRequest {
  personType?: PersonTypeValue;
}

function toQuery(req: PersonsPageRequest): string {
  const p = new URLSearchParams();
  if (req.page) p.set("Page", String(req.page));
  if (req.pageSize) p.set("PageSize", String(req.pageSize));
  if (req.search) p.set("Search", req.search);
  if (req.personType != null) p.set("personType", String(req.personType));
  return p.toString() ? `?${p.toString()}` : "";
}

export function getPersonsPaged(req: PersonsPageRequest): Promise<PagedResult<PersonDto>> {
  return http(`${BASE}${toQuery(req)}`);
}

export function getPersonById(id: string): Promise<PersonDetailDto> {
  return http(`${BASE}/${id}`);
}

export function createPerson(dto: CreatePersonDto): Promise<{ id: string }> {
  return http(BASE, { method: "POST", body: JSON.stringify(dto) });
}

export function updatePerson(id: string, dto: UpdatePersonDto): Promise<void> {
  return http(`${BASE}/${id}`, { method: "PUT", body: JSON.stringify(dto) });
}

export function deletePerson(id: string): Promise<void> {
  return http(`${BASE}/${id}`, { method: "DELETE" });
}

import http from "@/lib/http";
import type { PagedResult, PageRequest } from "@/types/paged";
import type { CountryDto, CreateCountryDto, UpdateCountryDto } from "@/types/country";

const BASE = "/api/Countries";

function toQuery(req: PageRequest): string {
  const p = new URLSearchParams();
  if (req.page) p.set("Page", String(req.page));
  if (req.pageSize) p.set("PageSize", String(req.pageSize));
  if (req.search) p.set("Search", req.search);
  return p.toString() ? `?${p.toString()}` : "";
}

export function getCountriesPaged(req: PageRequest): Promise<PagedResult<CountryDto>> {
  return http(`${BASE}${toQuery(req)}`);
}

export function getCountryById(id: string): Promise<CountryDto> {
  return http(`${BASE}/${id}`);
}

export function createCountry(dto: CreateCountryDto): Promise<{ id: string }> {
  return http(BASE, { method: "POST", body: JSON.stringify(dto) });
}

export function updateCountry(id: string, dto: UpdateCountryDto): Promise<void> {
  return http(`${BASE}/${id}`, { method: "PUT", body: JSON.stringify(dto) });
}

export function deleteCountry(id: string): Promise<void> {
  return http(`${BASE}/${id}`, { method: "DELETE" });
}

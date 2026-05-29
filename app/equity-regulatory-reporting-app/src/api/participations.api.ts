import http from "@/lib/http";
import type { PagedResult, PageRequest } from "@/types/paged";
import type { ParticipationDto, CreateParticipationDto, UpdateParticipationDto } from "@/types/participation";

const BASE = "/api/Participations";

export interface ParticipationsPageRequest extends PageRequest {
  companyId?: string;
}

function toQuery(req: ParticipationsPageRequest): string {
  const p = new URLSearchParams();
  if (req.page) p.set("Page", String(req.page));
  if (req.pageSize) p.set("PageSize", String(req.pageSize));
  if (req.search) p.set("Search", req.search);
  if (req.companyId) p.set("companyId", req.companyId);
  return p.toString() ? `?${p.toString()}` : "";
}

export function getParticipationsPaged(req: ParticipationsPageRequest): Promise<PagedResult<ParticipationDto>> {
  return http(`${BASE}${toQuery(req)}`);
}

export function getParticipationById(id: string): Promise<ParticipationDto> {
  return http(`${BASE}/${id}`);
}

export function createParticipation(dto: CreateParticipationDto): Promise<{ id: string }> {
  return http(BASE, { method: "POST", body: JSON.stringify(dto) });
}

export function updateParticipation(id: string, dto: UpdateParticipationDto): Promise<void> {
  return http(`${BASE}/${id}`, { method: "PUT", body: JSON.stringify(dto) });
}

export function deleteParticipation(id: string): Promise<void> {
  return http(`${BASE}/${id}`, { method: "DELETE" });
}

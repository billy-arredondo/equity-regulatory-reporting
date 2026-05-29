import http from "@/lib/http";
import type { PagedResult, PageRequest } from "@/types/paged";
import type { PositionDto, CreatePositionDto, UpdatePositionDto } from "@/types/position";

const BASE = "/api/Positions";

function toQuery(req: PageRequest): string {
  const p = new URLSearchParams();
  if (req.page) p.set("Page", String(req.page));
  if (req.pageSize) p.set("PageSize", String(req.pageSize));
  if (req.search) p.set("Search", req.search);
  return p.toString() ? `?${p.toString()}` : "";
}

export function getPositionsPaged(req: PageRequest): Promise<PagedResult<PositionDto>> {
  return http(`${BASE}${toQuery(req)}`);
}

export function getPositionById(id: string): Promise<PositionDto> {
  return http(`${BASE}/${id}`);
}

export function createPosition(dto: CreatePositionDto): Promise<{ id: string }> {
  return http(BASE, { method: "POST", body: JSON.stringify(dto) });
}

export function updatePosition(id: string, dto: UpdatePositionDto): Promise<void> {
  return http(`${BASE}/${id}`, { method: "PUT", body: JSON.stringify(dto) });
}

export function deletePosition(id: string): Promise<void> {
  return http(`${BASE}/${id}`, { method: "DELETE" });
}

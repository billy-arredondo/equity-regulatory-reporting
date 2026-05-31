import http from "@/lib/http";
import type { PagedResult, PageRequest } from "@/types/paged";
import type { DocumentTypeDto, CreateDocumentTypeDto, UpdateDocumentTypeDto } from "@/types/document-type";

const BASE = "/api/DocumentTypes";

function toQuery(req: PageRequest): string {
  const p = new URLSearchParams();
  if (req.page) p.set("Page", String(req.page));
  if (req.pageSize) p.set("PageSize", String(req.pageSize));
  if (req.search) p.set("Search", req.search);
  return p.toString() ? `?${p.toString()}` : "";
}

export function getDocumentTypesPaged(req: PageRequest): Promise<PagedResult<DocumentTypeDto>> {
  return http(`${BASE}${toQuery(req)}`);
}

export function getDocumentTypeById(id: string): Promise<DocumentTypeDto> {
  return http(`${BASE}/${id}`);
}

export function createDocumentType(dto: CreateDocumentTypeDto): Promise<{ id: string }> {
  return http(BASE, { method: "POST", body: JSON.stringify(dto) });
}

export function updateDocumentType(id: string, dto: UpdateDocumentTypeDto): Promise<void> {
  return http(`${BASE}/${id}`, { method: "PUT", body: JSON.stringify(dto) });
}

export function deleteDocumentType(id: string): Promise<void> {
  return http(`${BASE}/${id}`, { method: "DELETE" });
}

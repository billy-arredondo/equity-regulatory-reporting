import http from "@/lib/http";
import type { PagedResult, PageRequest } from "@/types/paged";
import type { BoardMemberDto, CreateBoardMemberDto, UpdateBoardMemberDto } from "@/types/board-member";

const BASE = "/api/BoardMembers";

export interface BoardMembersPageRequest extends PageRequest {
  companyId?: string;
}

function toQuery(req: BoardMembersPageRequest): string {
  const p = new URLSearchParams();
  if (req.page) p.set("Page", String(req.page));
  if (req.pageSize) p.set("PageSize", String(req.pageSize));
  if (req.search) p.set("Search", req.search);
  if (req.companyId) p.set("companyId", req.companyId);
  return p.toString() ? `?${p.toString()}` : "";
}

export function getBoardMembersPaged(req: BoardMembersPageRequest): Promise<PagedResult<BoardMemberDto>> {
  return http(`${BASE}${toQuery(req)}`);
}

export function getBoardMemberById(id: string): Promise<BoardMemberDto> {
  return http(`${BASE}/${id}`);
}

export function createBoardMember(dto: CreateBoardMemberDto): Promise<{ id: string }> {
  return http(BASE, { method: "POST", body: JSON.stringify(dto) });
}

export function updateBoardMember(id: string, dto: UpdateBoardMemberDto): Promise<void> {
  return http(`${BASE}/${id}`, { method: "PUT", body: JSON.stringify(dto) });
}

export function deleteBoardMember(id: string): Promise<void> {
  return http(`${BASE}/${id}`, { method: "DELETE" });
}

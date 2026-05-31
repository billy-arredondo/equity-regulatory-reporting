import http from "@/lib/http";
import type { PagedResult, PageRequest } from "@/types/paged";
import type { UserDto, UpdateUserDto, AssignRoleDto } from "@/types/user";

const BASE = "/api/Users";

function toQuery(req: PageRequest): string {
  const p = new URLSearchParams();
  if (req.page) p.set("Page", String(req.page));
  if (req.pageSize) p.set("PageSize", String(req.pageSize));
  if (req.search) p.set("Search", req.search);
  return p.toString() ? `?${p.toString()}` : "";
}

export function getUsersPaged(req: PageRequest): Promise<PagedResult<UserDto>> {
  return http(`${BASE}${toQuery(req)}`);
}

export function getUserById(id: string): Promise<UserDto> {
  return http(`${BASE}/${id}`);
}

export function updateUser(id: string, dto: UpdateUserDto): Promise<void> {
  return http(`${BASE}/${id}`, { method: "PUT", body: JSON.stringify(dto) });
}

export function deleteUser(id: string): Promise<void> {
  return http(`${BASE}/${id}`, { method: "DELETE" });
}

export function assignRole(id: string, dto: AssignRoleDto): Promise<void> {
  return http(`${BASE}/${id}/role`, { method: "POST", body: JSON.stringify(dto) });
}

export function getRoles(): Promise<string[]> {
  return http(`${BASE}/roles`);
}

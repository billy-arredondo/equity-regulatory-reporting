import type { AccessTokenVm, LoginDto } from "@/types/auth";

const BASE = "/api/Auth";

export async function login(dto: LoginDto): Promise<AccessTokenVm> {
  const res = await fetch(`${BASE}/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(dto),
    credentials: "include",
  });
  if (!res.ok) {
    const text = await res.text().catch(() => "");
    throw new Error(text || "Credenciales inválidas.");
  }
  return res.json() as Promise<AccessTokenVm>;
}

export async function refreshToken(): Promise<AccessTokenVm> {
  const res = await fetch(`${BASE}/refresh`, {
    method: "POST",
    credentials: "include",
  });
  if (!res.ok) throw new Error("Sesión expirada.");
  return res.json() as Promise<AccessTokenVm>;
}

export async function revokeToken(): Promise<void> {
  await fetch(`${BASE}/revoke`, {
    method: "POST",
    credentials: "include",
  });
}

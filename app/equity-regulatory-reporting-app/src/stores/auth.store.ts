import { create } from "zustand";
import { persist } from "zustand/middleware";
import type { JwtUser } from "@/types/auth";

function parseJwt(token: string): Record<string, unknown> | null {
  try {
    const payload = token.split(".")[1];
    const decoded = atob(payload.replace(/-/g, "+").replace(/_/g, "/"));
    return JSON.parse(decoded) as Record<string, unknown>;
  } catch {
    return null;
  }
}

function decodeUser(token: string): JwtUser | null {
  const claims = parseJwt(token);
  if (!claims) return null;
  return {
    id: String(claims["sub"] ?? ""),
    email: String(claims["email"] ?? ""),
    perm: parseInt(String(claims["perm"] ?? "0"), 10),
    roles: Array.isArray(claims[
      "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
    ])
      ? (claims[
          "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
        ] as string[])
      : typeof claims[
            "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
          ] === "string"
        ? [
            String(
              claims[
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
              ],
            ),
          ]
        : [],
  };
}

interface AuthState {
  accessToken: string | null;
  user: JwtUser | null;
  setAccessToken: (token: string) => void;
  clearAuth: () => void;
  hasPermission: (flag: number) => boolean;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set, get) => ({
      accessToken: null,
      user: null,
      setAccessToken(token) {
        set({ accessToken: token, user: decodeUser(token) });
      },
      clearAuth() {
        set({ accessToken: null, user: null });
      },
      hasPermission(flag) {
        const perm = get().user?.perm ?? 0;
        return (perm & flag) !== 0;
      },
    }),
    {
      name: "auth-storage",
      partialize: (state) => ({ accessToken: state.accessToken }),
      onRehydrateStorage: () => (state) => {
        if (state?.accessToken) {
          state.user = decodeUser(state.accessToken);
        }
      },
    },
  ),
);

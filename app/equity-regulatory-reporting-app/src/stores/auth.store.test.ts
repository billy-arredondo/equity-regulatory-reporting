import { describe, it, expect, beforeEach } from "vitest";
import { useAuthStore } from "./auth.store";
import { Permission } from "@/lib/permissions";
import { makeJwt } from "@/test/jwt";

const ROLE_CLAIM =
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

beforeEach(() => {
  useAuthStore.setState({ accessToken: null, user: null });
  localStorage.clear();
});

describe("setAccessToken", () => {
  it("decodes user fields from a valid JWT", () => {
    const token = makeJwt({
      sub: "user-123",
      email: "test@example.com",
      perm: 7,
      [ROLE_CLAIM]: ["Admin"],
    });

    useAuthStore.getState().setAccessToken(token);
    const { accessToken, user } = useAuthStore.getState();

    expect(accessToken).toBe(token);
    expect(user).not.toBeNull();
    expect(user?.id).toBe("user-123");
    expect(user?.email).toBe("test@example.com");
    expect(user?.perm).toBe(7);
    expect(user?.roles).toEqual(["Admin"]);
  });

  it("wraps a single string role in an array", () => {
    const token = makeJwt({ sub: "u1", [ROLE_CLAIM]: "Operator" });
    useAuthStore.getState().setAccessToken(token);
    expect(useAuthStore.getState().user?.roles).toEqual(["Operator"]);
  });

  it("defaults roles to [] when the claim is absent", () => {
    const token = makeJwt({ sub: "u1" });
    useAuthStore.getState().setAccessToken(token);
    expect(useAuthStore.getState().user?.roles).toEqual([]);
  });

  it("sets user to null for a malformed token, but still stores accessToken", () => {
    useAuthStore.getState().setAccessToken("not.a.jwt");
    const { accessToken, user } = useAuthStore.getState();
    expect(accessToken).toBe("not.a.jwt");
    expect(user).toBeNull();
  });

  it("perm defaults to 0 when the claim is absent", () => {
    const token = makeJwt({ sub: "u1", email: "a@b.com" });
    useAuthStore.getState().setAccessToken(token);
    expect(useAuthStore.getState().user?.perm).toBe(0);
  });
});

describe("clearAuth", () => {
  it("resets accessToken and user to null", () => {
    const token = makeJwt({ sub: "u1" });
    useAuthStore.getState().setAccessToken(token);
    useAuthStore.getState().clearAuth();

    const { accessToken, user } = useAuthStore.getState();
    expect(accessToken).toBeNull();
    expect(user).toBeNull();
  });
});

describe("hasPermission", () => {
  it("returns true when the user has the flag", () => {
    const token = makeJwt({ perm: Permission.PersonRead | Permission.PersonWrite });
    useAuthStore.getState().setAccessToken(token);
    expect(useAuthStore.getState().hasPermission(Permission.PersonRead)).toBe(true);
    expect(useAuthStore.getState().hasPermission(Permission.PersonWrite)).toBe(true);
  });

  it("returns false when the user lacks the flag", () => {
    const token = makeJwt({ perm: Permission.PersonRead });
    useAuthStore.getState().setAccessToken(token);
    expect(useAuthStore.getState().hasPermission(Permission.PersonWrite)).toBe(false);
  });

  it("returns false for Permission.None (0) regardless of user perm", () => {
    const token = makeJwt({ perm: Permission.Admin });
    useAuthStore.getState().setAccessToken(token);
    expect(useAuthStore.getState().hasPermission(Permission.None)).toBe(false);
  });

  it("returns false when there is no user", () => {
    expect(useAuthStore.getState().hasPermission(Permission.PersonRead)).toBe(false);
  });

  it("Admin (~0) grants any non-zero permission", () => {
    const token = makeJwt({ perm: Permission.Admin });
    useAuthStore.getState().setAccessToken(token);
    expect(useAuthStore.getState().hasPermission(Permission.PositionDelete)).toBe(true);
    expect(useAuthStore.getState().hasPermission(Permission.BoardWrite)).toBe(true);
  });
});

describe("persist middleware", () => {
  it("persists only accessToken to localStorage, not user", () => {
    const token = makeJwt({ sub: "u1", email: "a@b.com", perm: 3 });
    useAuthStore.getState().setAccessToken(token);

    const stored = JSON.parse(localStorage.getItem("auth-storage") ?? "{}") as {
      state?: { accessToken?: string; user?: unknown };
    };

    expect(stored.state?.accessToken).toBe(token);
    expect(stored.state).not.toHaveProperty("user");
  });
});

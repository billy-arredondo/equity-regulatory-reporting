import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { makeJwt } from "@/test/jwt";

// Helpers ----------------------------------------------------------------

function mockLocation() {
  const loc = { href: "" };
  Object.defineProperty(window, "location", {
    value: loc,
    writable: true,
    configurable: true,
  });
  return loc;
}

function makeResponse(
  status: number,
  body?: unknown,
  contentType = "application/json",
): Response {
  const bodyText = body !== undefined ? JSON.stringify(body) : "";
  return new Response(bodyText, {
    status,
    headers: { "Content-Type": contentType },
  });
}

function makeTextResponse(status: number, text: string): Response {
  return new Response(text, {
    status,
    headers: { "Content-Type": "text/plain" },
  });
}

function validToken() {
  return makeJwt({ sub: "u1", exp: 9_999_999_999 });
}

function expiredToken() {
  return makeJwt({ sub: "u1", exp: 1 });
}

// Re-import both modules after vi.resetModules() so they share the same
// module registry. Without this, the fresh http module uses a different
// auth.store instance than the one the test file references.
async function getModules() {
  const [httpMod, storeMod] = await Promise.all([
    import("./http"),
    import("@/stores/auth.store"),
  ]);
  return { http: httpMod.default, useAuthStore: storeMod.useAuthStore };
}

// Setup ------------------------------------------------------------------

beforeEach(() => {
  vi.resetModules();
  localStorage.clear();
  mockLocation();
});

afterEach(() => {
  vi.restoreAllMocks();
  vi.useRealTimers();
});

// Tests ------------------------------------------------------------------

describe("http — happy path", () => {
  it("returns parsed JSON for a 200 response", async () => {
    vi.stubGlobal("fetch", vi.fn().mockResolvedValueOnce(makeResponse(200, { id: 1 })));
    const { http, useAuthStore } = await getModules();
    useAuthStore.getState().setAccessToken(validToken());

    const result = await http<{ id: number }>("/api/things");
    expect(result).toEqual({ id: 1 });
  });

  it("sends Authorization, Content-Type and credentials on every request", async () => {
    const fetchMock = vi.fn().mockResolvedValueOnce(makeResponse(200, {}));
    vi.stubGlobal("fetch", fetchMock);
    const { http, useAuthStore } = await getModules();
    const token = validToken();
    useAuthStore.getState().setAccessToken(token);

    await http("/api/things");

    const [, init] = fetchMock.mock.calls[0] as [string, RequestInit];
    const headers = new Headers(init.headers);
    expect(headers.get("Authorization")).toBe(`Bearer ${token}`);
    expect(headers.get("Content-Type")).toBe("application/json");
    expect(init.credentials).toBe("include");
  });

  it("returns undefined for a 204 response", async () => {
    vi.stubGlobal("fetch", vi.fn().mockResolvedValueOnce(new Response(null, { status: 204 })));
    const { http, useAuthStore } = await getModules();
    useAuthStore.getState().setAccessToken(validToken());

    const result = await http("/api/things");
    expect(result).toBeUndefined();
  });

  it("does not set Content-Type when body is FormData", async () => {
    const fetchMock = vi.fn().mockResolvedValueOnce(makeResponse(200, {}));
    vi.stubGlobal("fetch", fetchMock);
    const { http, useAuthStore } = await getModules();
    useAuthStore.getState().setAccessToken(validToken());

    await http("/api/upload", { method: "POST", body: new FormData() });

    const [, init] = fetchMock.mock.calls[0] as [string, RequestInit];
    const headers = new Headers(init.headers);
    expect(headers.has("Content-Type")).toBe(false);
  });
});

describe("http — non-ok errors", () => {
  it("throws using 'detail' field from JSON body", async () => {
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValueOnce(makeResponse(400, { detail: "Invalid input" })),
    );
    const { http, useAuthStore } = await getModules();
    useAuthStore.getState().setAccessToken(validToken());

    await expect(http("/api/things")).rejects.toThrow("Invalid input");
  });

  it("falls back to 'title' when 'detail' is absent", async () => {
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValueOnce(makeResponse(422, { title: "Unprocessable" })),
    );
    const { http, useAuthStore } = await getModules();
    useAuthStore.getState().setAccessToken(validToken());

    await expect(http("/api/things")).rejects.toThrow("Unprocessable");
  });

  it("falls back to 'Error <status>' when body has neither field", async () => {
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValueOnce(makeResponse(500, {})),
    );
    const { http, useAuthStore } = await getModules();
    useAuthStore.getState().setAccessToken(validToken());

    await expect(http("/api/things")).rejects.toThrow("Error 500");
  });

  it("uses plain text body as error message", async () => {
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValueOnce(makeTextResponse(503, "Service unavailable")),
    );
    const { http, useAuthStore } = await getModules();
    useAuthStore.getState().setAccessToken(validToken());

    await expect(http("/api/things")).rejects.toThrow("Service unavailable");
  });
});

describe("http — 401 refresh flow", () => {
  it("retries with new token after a successful refresh", async () => {
    const newToken = makeJwt({ sub: "u1", exp: 9_999_999_999, perm: 3 });
    const fetchMock = vi
      .fn()
      .mockResolvedValueOnce(new Response(null, { status: 401 }))
      .mockResolvedValueOnce(makeResponse(200, { accessToken: newToken }))
      .mockResolvedValueOnce(makeResponse(200, { data: "ok" }));

    vi.stubGlobal("fetch", fetchMock);
    const { http, useAuthStore } = await getModules();
    useAuthStore.getState().setAccessToken(validToken());

    const result = await http<{ data: string }>("/api/things");

    expect(result).toEqual({ data: "ok" });
    expect(fetchMock).toHaveBeenCalledTimes(3);

    const [refreshUrl, refreshInit] = fetchMock.mock.calls[1] as [string, RequestInit];
    expect(refreshUrl).toBe("/api/Auth/refresh");
    expect(refreshInit.method).toBe("POST");
    expect(refreshInit.credentials).toBe("include");
  });

  it("clears session and throws when refresh fails after a 401", async () => {
    const location = mockLocation();
    const fetchMock = vi
      .fn()
      .mockResolvedValueOnce(new Response(null, { status: 401 }))
      .mockResolvedValueOnce(new Response(null, { status: 401 }));

    vi.stubGlobal("fetch", fetchMock);
    const { http, useAuthStore } = await getModules();
    useAuthStore.getState().setAccessToken(validToken());

    await expect(http("/api/things")).rejects.toThrow("Session expired.");

    expect(useAuthStore.getState().accessToken).toBeNull();
    expect(location.href).toBe("/login");
  });
});

describe("http — pre-request expired token", () => {
  it("triggers refresh before sending when the token is expired", async () => {
    const newToken = makeJwt({ sub: "u1", exp: 9_999_999_999 });
    const fetchMock = vi
      .fn()
      .mockResolvedValueOnce(makeResponse(200, { accessToken: newToken }))
      .mockResolvedValueOnce(makeResponse(200, { ok: true }));

    vi.stubGlobal("fetch", fetchMock);
    const { http, useAuthStore } = await getModules();
    useAuthStore.getState().setAccessToken(expiredToken());

    const result = await http<{ ok: boolean }>("/api/things");

    expect(result).toEqual({ ok: true });
    const [firstUrl] = fetchMock.mock.calls[0] as [string, RequestInit];
    expect(firstUrl).toBe("/api/Auth/refresh");
  });

  it("clears session and throws when refresh fails for an expired token", async () => {
    const location = mockLocation();
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValueOnce(new Response(null, { status: 401 })),
    );
    const { http, useAuthStore } = await getModules();
    useAuthStore.getState().setAccessToken(expiredToken());

    await expect(http("/api/things")).rejects.toThrow("Session expired.");
    expect(location.href).toBe("/login");
  });
});

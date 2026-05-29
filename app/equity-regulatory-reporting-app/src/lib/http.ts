import type { AccessTokenVm } from "@/types/auth";

type RequestInitExtended = RequestInit & { _retry?: boolean };

let refreshPromise: Promise<string | null> | null = null;

function getAccessToken(): string | null {
  try {
    const raw = localStorage.getItem("auth-storage");
    if (!raw) return null;
    const parsed = JSON.parse(raw) as { state?: { accessToken?: string } };
    return parsed.state?.accessToken ?? null;
  } catch {
    return null;
  }
}

function setAccessToken(token: string): void {
  try {
    const raw = localStorage.getItem("auth-storage");
    const parsed = raw
      ? (JSON.parse(raw) as { state?: Record<string, unknown> })
      : { state: {} };
    parsed.state = { ...parsed.state, accessToken: token };
    localStorage.setItem("auth-storage", JSON.stringify(parsed));
  } catch {
    // ignore
  }
}

async function attemptRefresh(): Promise<string | null> {
  try {
    const res = await fetch("/api/Auth/refresh", {
      method: "POST",
      credentials: "include",
    });
    if (!res.ok) return null;
    const data = (await res.json()) as AccessTokenVm;
    setAccessToken(data.accessToken);
    return data.accessToken;
  } catch {
    return null;
  }
}

function clearSession(): void {
  localStorage.removeItem("auth-storage");
  window.location.href = "/login";
}

async function http<T = void>(
  path: string,
  init: RequestInitExtended = {},
): Promise<T> {
  const accessToken = getAccessToken();
  const headers = new Headers(init.headers);

  if (!headers.has("Content-Type") && !(init.body instanceof FormData)) {
    headers.set("Content-Type", "application/json");
  }

  if (accessToken) {
    headers.set("Authorization", `Bearer ${accessToken}`);
  }

  const res = await fetch(path, {
    ...init,
    headers,
    credentials: "include",
  });

  if (res.status === 401 && !init._retry) {
    if (!refreshPromise) {
      refreshPromise = attemptRefresh().finally(() => {
        refreshPromise = null;
      });
    }

    const newToken = await refreshPromise;

    if (!newToken) {
      clearSession();
      throw new Error("Session expired.");
    }

    const retryHeaders = new Headers(headers);
    retryHeaders.set("Authorization", `Bearer ${newToken}`);
    const retryRes = await fetch(path, {
      ...init,
      headers: retryHeaders,
      credentials: "include",
      _retry: true,
    } as RequestInitExtended);

    if (!retryRes.ok) {
      await throwFromResponse(retryRes);
    }

    if (retryRes.status === 204) return undefined as T;
    return retryRes.json() as Promise<T>;
  }

  if (!res.ok) {
    await throwFromResponse(res);
  }

  if (res.status === 204) return undefined as T;
  return res.json() as Promise<T>;
}

async function throwFromResponse(res: Response): Promise<never> {
  let message = `Error ${res.status}`;
  try {
    const ct = res.headers.get("content-type") ?? "";
    if (ct.includes("json")) {
      const body = (await res.json()) as { title?: string; detail?: string };
      message = body.detail ?? body.title ?? message;
    } else {
      const text = await res.text();
      if (text) message = text;
    }
  } catch {
    // ignore parse errors
  }
  throw new Error(message);
}

export default http;

import type { ReactNode } from "react";
import { useAuthStore } from "@/stores/auth.store";

interface Props {
  perm: number;
  children: ReactNode;
  fallback?: ReactNode;
}

export function PermissionGuard({ perm, children, fallback = null }: Props) {
  const hasPermission = useAuthStore((s) => s.hasPermission);
  return hasPermission(perm) ? <>{children}</> : <>{fallback}</>;
}

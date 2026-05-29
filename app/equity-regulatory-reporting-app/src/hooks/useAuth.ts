import { useMutation } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { toast } from "sonner";
import { login, revokeToken } from "@/api/auth.api";
import { useAuthStore } from "@/stores/auth.store";
import type { LoginDto } from "@/types/auth";

export function useLoginMutation() {
  const setAccessToken = useAuthStore((s) => s.setAccessToken);
  const navigate = useNavigate();

  return useMutation({
    mutationFn: (dto: LoginDto) => login(dto),
    onSuccess(data) {
      setAccessToken(data.accessToken);
      void navigate("/");
    },
    onError(err) {
      toast.error(err instanceof Error ? err.message : "Error al iniciar sesión.");
    },
  });
}

export function useLogoutMutation() {
  const clearAuth = useAuthStore((s) => s.clearAuth);
  const navigate = useNavigate();

  return useMutation({
    mutationFn: revokeToken,
    onSettled() {
      clearAuth();
      void navigate("/login");
    },
  });
}

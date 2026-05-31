import { useState } from "react";
import { Navigate } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { useLoginMutation } from "@/hooks/useAuth";
import { useAuthStore } from "@/stores/auth.store";
import type { LoginDto } from "@/types/auth";

export function LoginPage() {
  const accessToken = useAuthStore((s) => s.accessToken);
  const { mutate: login, isPending, error } = useLoginMutation();
  const [form, setForm] = useState<LoginDto>({ email: "", password: "" });

  if (accessToken) return <Navigate to="/" replace />;

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    login(form);
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-background">
      <div className="w-full max-w-sm space-y-6 rounded-xl border bg-card p-8 shadow-sm">
        <div className="space-y-1">
          <h1 className="text-xl font-semibold">Iniciar sesión</h1>
          <p className="text-sm text-muted-foreground">
            Equity Regulatory Reporting
          </p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="email">Correo electrónico</Label>
            <Input
              id="email"
              type="email"
              value={form.email}
              onChange={(e) => setForm((f) => ({ ...f, email: e.target.value }))}
              placeholder="usuario@ejemplo.com"
              autoComplete="email"
              required
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="password">Contraseña</Label>
            <Input
              id="password"
              type="password"
              value={form.password}
              onChange={(e) =>
                setForm((f) => ({ ...f, password: e.target.value }))
              }
              placeholder="••••••••"
              autoComplete="current-password"
              required
            />
          </div>

          {error && (
            <p className="text-sm text-destructive">
              {error instanceof Error ? error.message : "Error al iniciar sesión."}
            </p>
          )}

          <Button type="submit" className="w-full" disabled={isPending}>
            {isPending ? "..." : "Ingresar"}
          </Button>
        </form>
      </div>
    </div>
  );
}

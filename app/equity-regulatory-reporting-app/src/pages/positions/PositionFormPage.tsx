import { useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Button } from "@/components/ui/button";
import { PageHeader } from "@/components/shared/PageHeader";
import { ActionButton } from "@/elements/ActionButton";
import {
  usePositionDetailQuery,
  useCreatePositionMutation,
  useUpdatePositionMutation,
  NO_POSITION_NAME,
} from "@/hooks/usePositions";
import type { CreatePositionDto } from "@/types/position";

export function PositionFormPage() {
  const { id } = useParams<{ id: string }>();
  const isEdit = !!id;
  const navigate = useNavigate();
  const [initialized, setInitialized] = useState(false);
  const [form, setForm] = useState<CreatePositionDto>({ name: "", reportCode: "" });

  const { data } = usePositionDetailQuery(id ?? "");
  const { mutate: create, isPending: isCreating } = useCreatePositionMutation();
  const { mutate: update, isPending: isUpdating } = useUpdatePositionMutation();
  const isPending = isCreating || isUpdating;

  const isProtected = data?.name === NO_POSITION_NAME;

  useEffect(() => {
    if (!isEdit || initialized || !data) return;
    setForm({ name: data.name, reportCode: data.reportCode });
    setInitialized(true);
  }, [isEdit, data, initialized]);

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (isEdit) {
      update({ id: id!, dto: form }, { onSuccess: () => void navigate(`/positions/${id}`) });
    } else {
      create(form, { onSuccess: (res) => void navigate(`/positions/${res.id}`) });
    }
  }

  return (
    <div>
      <Link
        to={isEdit ? `/positions/${id}` : "/positions"}
        className="mb-5 inline-flex items-center gap-1 text-sm text-muted-foreground hover:text-foreground"
      >
        ← {isEdit ? "Volver al detalle" : "Volver a cargos"}
      </Link>
      <PageHeader title={isEdit ? "Editar cargo" : "Nuevo cargo"} />
      <form onSubmit={handleSubmit} className="mt-4 max-w-lg space-y-4">
        <div className="space-y-2">
          <Label htmlFor="name">Nombre</Label>
          <Input
            id="name"
            value={form.name}
            disabled={isProtected}
            onChange={(e) => setForm((f) => ({ ...f, name: e.target.value }))}
            required
          />
          {isProtected && (
            <p className="text-xs text-muted-foreground">
              Este cargo del sistema no puede modificarse.
            </p>
          )}
        </div>
        <div className="space-y-2">
          <Label htmlFor="reportCode">Código de reporte</Label>
          <Input
            id="reportCode"
            value={form.reportCode}
            disabled={isProtected}
            onChange={(e) => setForm((f) => ({ ...f, reportCode: e.target.value }))}
            maxLength={2}
            placeholder="00"
            required
          />
        </div>
        <div className="mt-6 flex gap-2">
          <ActionButton type="submit" isLoading={isPending} disabled={isProtected}>
            {isEdit ? "Guardar cambios" : "Crear cargo"}
          </ActionButton>
          <Button type="button" variant="ghost" onClick={() => void navigate(-1)}>
            Cancelar
          </Button>
        </div>
      </form>
    </div>
  );
}

import { useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Button } from "@/components/ui/button";
import { PageHeader } from "@/components/shared/PageHeader";
import { ActionButton } from "@/elements/ActionButton";
import { useUserDetailQuery, useUpdateUserMutation } from "@/hooks/useUsers";
import type { UpdateUserDto } from "@/types/user";

export function UserFormPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [initialized, setInitialized] = useState(false);
  const [form, setForm] = useState<UpdateUserDto>({ firstName: "", lastName: "" });

  const { data: editData } = useUserDetailQuery(id ?? "");
  const { mutate: update, isPending } = useUpdateUserMutation();

  useEffect(() => {
    if (initialized || !editData) return;
    setForm({ firstName: editData.firstName, lastName: editData.lastName });
    setInitialized(true);
  }, [editData, initialized]);

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!id) return;
    update({ id, dto: form }, { onSuccess: () => void navigate(`/users/${id}`) });
  }

  return (
    <div>
      <Link
        to={`/users/${id}`}
        className="mb-5 inline-flex items-center gap-1 text-sm text-muted-foreground hover:text-foreground"
      >
        ← Volver al detalle
      </Link>
      <PageHeader title="Editar usuario" />
      <form onSubmit={handleSubmit} className="mt-4 max-w-lg space-y-4">
        <div className="space-y-2">
          <Label>Nombre</Label>
          <Input
            value={form.firstName}
            onChange={(e) => setForm((f) => ({ ...f, firstName: e.target.value }))}
            required
          />
        </div>
        <div className="space-y-2">
          <Label>Apellido</Label>
          <Input
            value={form.lastName}
            onChange={(e) => setForm((f) => ({ ...f, lastName: e.target.value }))}
            required
          />
        </div>
        <div className="mt-6 flex gap-2">
          <ActionButton type="submit" isLoading={isPending} disabled={isPending}>
            Guardar cambios
          </ActionButton>
          <Button type="button" variant="ghost" onClick={() => void navigate(-1)}>
            Cancelar
          </Button>
        </div>
      </form>
    </div>
  );
}

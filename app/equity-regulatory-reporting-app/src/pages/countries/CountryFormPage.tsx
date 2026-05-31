import { useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Button } from "@/components/ui/button";
import { PageHeader } from "@/components/shared/PageHeader";
import { ActionButton } from "@/elements/ActionButton";
import {
  useCountryDetailQuery,
  useCreateCountryMutation,
  useUpdateCountryMutation,
} from "@/hooks/useCountries";
import type { CreateCountryDto } from "@/types/country";

export function CountryFormPage() {
  const { id } = useParams<{ id: string }>();
  const isEdit = !!id;
  const navigate = useNavigate();
  const [initialized, setInitialized] = useState(false);
  const [form, setForm] = useState<CreateCountryDto>({ name: "", abbreviation: "" });

  const { data } = useCountryDetailQuery(id ?? "");
  const { mutate: create, isPending: isCreating } = useCreateCountryMutation();
  const { mutate: update, isPending: isUpdating } = useUpdateCountryMutation();
  const isPending = isCreating || isUpdating;

  useEffect(() => {
    if (!isEdit || initialized || !data) return;
    setForm({ name: data.name, abbreviation: data.abbreviation });
    setInitialized(true);
  }, [isEdit, data, initialized]);

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (isEdit) {
      update({ id: id!, dto: form }, { onSuccess: () => void navigate(`/countries/${id}`) });
    } else {
      create(form, { onSuccess: (res) => void navigate(`/countries/${res.id}`) });
    }
  }

  return (
    <div>
      <Link
        to={isEdit ? `/countries/${id}` : "/countries"}
        className="mb-5 inline-flex items-center gap-1 text-sm text-muted-foreground hover:text-foreground"
      >
        ← {isEdit ? "Volver al detalle" : "Volver a países"}
      </Link>
      <PageHeader title={isEdit ? "Editar país" : "Nuevo país"} />
      <form onSubmit={handleSubmit} className="mt-4 max-w-lg space-y-4">
        <div className="space-y-2">
          <Label htmlFor="name">Nombre</Label>
          <Input
            id="name"
            value={form.name}
            onChange={(e) => setForm((f) => ({ ...f, name: e.target.value }))}
            required
          />
        </div>
        <div className="space-y-2">
          <Label htmlFor="abbreviation">Abreviatura</Label>
          <Input
            id="abbreviation"
            value={form.abbreviation}
            maxLength={10}
            onChange={(e) => setForm((f) => ({ ...f, abbreviation: e.target.value }))}
            required
          />
        </div>
        <div className="mt-6 flex gap-2">
          <ActionButton type="submit" isLoading={isPending}>
            {isEdit ? "Guardar cambios" : "Crear país"}
          </ActionButton>
          <Button type="button" variant="ghost" onClick={() => void navigate(-1)}>
            Cancelar
          </Button>
        </div>
      </form>
    </div>
  );
}

import { useCallback, useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import { PageHeader } from "@/components/shared/PageHeader";
import { ActionButton } from "@/elements/ActionButton";
import { SearchableCombobox, type ComboboxOption } from "@/elements/SearchableCombobox";
import { usePersonsQuery } from "@/hooks/usePersons";
import {
  useParticipationDetailQuery,
  useCreateParticipationMutation,
  useUpdateParticipationMutation,
} from "@/hooks/useParticipations";
import type { CreateParticipationDto } from "@/types/participation";

interface Props {
  lockedCompanyId: string;
  lockedCompanyName: string;
  basePath: string;
}

const PAGE_SIZE = 25;

function emptyForm(lockedCompanyId: string): CreateParticipationDto {
  return {
    companyId: lockedCompanyId,
    shareholderId: "",
    percentage: 0,
    effectiveFrom: "",
    effectiveTo: null,
  };
}

export function ParticipationFormPage({ lockedCompanyId, lockedCompanyName, basePath }: Props) {
  const { participationId } = useParams<{ participationId: string }>();
  const isEdit = !!participationId;
  const navigate = useNavigate();
  const [initialized, setInitialized] = useState(false);
  const [form, setForm] = useState<CreateParticipationDto>(emptyForm(lockedCompanyId));
  const [isActive, setIsActive] = useState(true);
  const [shareholderSearch, setShareholderSearch] = useState("");

  const { data: editData } = useParticipationDetailQuery(participationId ?? "");
  const { mutate: create, isPending: isCreating } = useCreateParticipationMutation();
  const { mutate: update, isPending: isUpdating } = useUpdateParticipationMutation();
  const isPending = isCreating || isUpdating;

  const { data: shareholders, isLoading: shareholdersLoading } = usePersonsQuery({
    page: 1,
    pageSize: PAGE_SIZE,
    search: shareholderSearch || undefined,
  });

  const shareholderOptions: ComboboxOption[] = (shareholders?.items ?? []).map((p) => ({
    id: p.id,
    label: p.name,
    sublabel: p.documentNumber,
  }));

  const handleShareholderSearch = useCallback((s: string) => setShareholderSearch(s), []);

  useEffect(() => {
    if (!isEdit || initialized || !editData) return;
    setForm({
      companyId: editData.companyId,
      shareholderId: editData.shareholderId,
      percentage: editData.percentage,
      effectiveFrom: editData.effectiveFrom,
      effectiveTo: editData.effectiveTo,
    });
    setIsActive(editData.effectiveTo === null);
    setInitialized(true);
  }, [isEdit, editData, initialized]);

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!form.companyId || !form.shareholderId || !form.effectiveFrom) return;
    const dto: CreateParticipationDto = {
      ...form,
      effectiveTo: isActive ? null : form.effectiveTo,
    };
    if (isEdit) {
      update(
        { id: participationId!, dto },
        { onSuccess: () => void navigate(`${basePath}/${participationId}`) },
      );
    } else {
      create(dto, { onSuccess: (res) => void navigate(`${basePath}/${res.id}`) });
    }
  }

  const canSubmit =
    !isPending &&
    !!form.shareholderId &&
    form.percentage >= 0 &&
    form.percentage <= 100 &&
    !!form.effectiveFrom &&
    (isActive || !!form.effectiveTo);

  return (
    <div>
      <Link
        to={isEdit ? `${basePath}/${participationId}` : basePath}
        className="mb-5 inline-flex items-center gap-1 text-sm text-muted-foreground hover:text-foreground"
      >
        ← {isEdit ? "Volver al detalle" : "Volver a participaciones"}
      </Link>
      <PageHeader title={isEdit ? "Editar participación" : "Nueva participación"} />
      <form onSubmit={handleSubmit} className="mt-4 max-w-lg space-y-4">
        <div className="space-y-2">
          <Label>Empresa</Label>
          <p className="flex h-9 items-center rounded-md border bg-muted px-3 text-sm font-medium text-muted-foreground">
            {lockedCompanyName}
          </p>
        </div>
        <div className="space-y-2">
          <Label>
            Accionista <span className="text-destructive">*</span>
          </Label>
          <SearchableCombobox
            value={form.shareholderId || null}
            onChange={(v) => setForm((f) => ({ ...f, shareholderId: v ?? "" }))}
            options={shareholderOptions}
            isLoading={shareholdersLoading}
            onSearchChange={handleShareholderSearch}
            placeholder="Buscar persona..."
          />
        </div>
        <div className="space-y-2">
          <Label>Porcentaje (%)</Label>
          <Input
            type="number"
            min={0}
            max={100}
            step={0.01}
            value={form.percentage}
            onChange={(e) => setForm((f) => ({ ...f, percentage: parseFloat(e.target.value) || 0 }))}
            required
          />
        </div>
        <div className="space-y-2">
          <Label>Fecha desde</Label>
          <Input
            type="date"
            value={form.effectiveFrom}
            onChange={(e) => setForm((f) => ({ ...f, effectiveFrom: e.target.value }))}
            required
          />
        </div>
        <div className="flex items-center gap-2">
          <div
            role="checkbox"
            aria-checked={isActive}
            tabIndex={0}
            onClick={() => setIsActive((v) => !v)}
            onKeyDown={(e) => {
              if (e.key === "Enter" || e.key === " ") {
                e.preventDefault();
                setIsActive((v) => !v);
              }
            }}
          >
            <Checkbox
              id="isActive"
              checked={isActive}
              className="pointer-events-none"
              tabIndex={-1}
            />
          </div>
          <Label htmlFor="isActive" className="cursor-pointer font-normal">
            Vigente (sin fecha de vencimiento)
          </Label>
        </div>
        {!isActive && (
          <div className="space-y-2">
            <Label>Fecha hasta</Label>
            <Input
              type="date"
              value={form.effectiveTo ?? ""}
              min={form.effectiveFrom || undefined}
              onChange={(e) => setForm((f) => ({ ...f, effectiveTo: e.target.value || null }))}
              required
            />
          </div>
        )}
        <div className="mt-6 flex gap-2">
          <ActionButton type="submit" isLoading={isPending} disabled={!canSubmit}>
            {isEdit ? "Guardar cambios" : "Crear participación"}
          </ActionButton>
          <Button type="button" variant="ghost" onClick={() => void navigate(-1)}>
            Cancelar
          </Button>
        </div>
      </form>
    </div>
  );
}

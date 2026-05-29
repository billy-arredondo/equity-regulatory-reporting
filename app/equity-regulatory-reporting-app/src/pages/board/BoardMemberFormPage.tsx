import { useCallback, useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { Label } from "@/components/ui/label";
import { Button } from "@/components/ui/button";
import { PageHeader } from "@/components/shared/PageHeader";
import { ActionButton } from "@/elements/ActionButton";
import { SelectField } from "@/elements/SelectField";
import { SearchableCombobox, type ComboboxOption } from "@/elements/SearchableCombobox";
import { usePersonsQuery } from "@/hooks/usePersons";
import { usePositionsQuery, NO_POSITION_NAME } from "@/hooks/usePositions";
import {
  useBoardMemberDetailQuery,
  useCreateBoardMemberMutation,
  useUpdateBoardMemberMutation,
} from "@/hooks/useBoardMembers";
import { PersonType } from "@/lib/person-types";
import type { CreateBoardMemberDto } from "@/types/board-member";

const PAGE_SIZE = 25;
const POSITIONS_PAGE_SIZE = 100;

function emptyForm(): CreateBoardMemberDto {
  return {
    companyId: "",
    memberId: "",
    primaryPositionId: "",
    secondaryPositionId: null,
  };
}

export function BoardMemberFormPage() {
  const { id } = useParams<{ id: string }>();
  const isEdit = !!id;
  const navigate = useNavigate();
  const [initialized, setInitialized] = useState(false);
  const [form, setForm] = useState<CreateBoardMemberDto>(emptyForm());
  const [companySearch, setCompanySearch] = useState("");
  const [memberSearch, setMemberSearch] = useState("");

  const { data: editData } = useBoardMemberDetailQuery(id ?? "");
  const { mutate: create, isPending: isCreating } = useCreateBoardMemberMutation();
  const { mutate: update, isPending: isUpdating } = useUpdateBoardMemberMutation();
  const isPending = isCreating || isUpdating;

  const { data: companies, isLoading: companiesLoading } = usePersonsQuery({
    page: 1,
    pageSize: PAGE_SIZE,
    search: companySearch || undefined,
    personType: PersonType.Legal,
  });

  const { data: members, isLoading: membersLoading } = usePersonsQuery({
    page: 1,
    pageSize: PAGE_SIZE,
    search: memberSearch || undefined,
    personType: PersonType.Natural,
  });

  const { data: positions } = usePositionsQuery({ page: 1, pageSize: POSITIONS_PAGE_SIZE });

  const noPositionId = positions?.items.find((p) => p.name === NO_POSITION_NAME)?.id ?? "";

  const companyOptions: ComboboxOption[] = (companies?.items ?? []).map((p) => ({
    id: p.id,
    label: p.name,
    sublabel: p.documentNumber,
  }));

  const memberOptions: ComboboxOption[] = (members?.items ?? []).map((p) => ({
    id: p.id,
    label: p.name,
    sublabel: p.documentNumber,
  }));

  const positionOptions = (positions?.items ?? []).map((p) => ({
    value: p.id,
    label: p.name,
  }));

  const handleCompanySearch = useCallback((s: string) => setCompanySearch(s), []);
  const handleMemberSearch = useCallback((s: string) => setMemberSearch(s), []);

  useEffect(() => {
    if (!isEdit || initialized || !editData) return;
    setForm({
      companyId: editData.companyId,
      memberId: editData.memberId,
      primaryPositionId: editData.primaryPositionId,
      secondaryPositionId: editData.secondaryPositionId,
    });
    setInitialized(true);
  }, [isEdit, editData, initialized]);

  useEffect(() => {
    if (!isEdit && !initialized && noPositionId && !form.secondaryPositionId) {
      setForm((f) => ({ ...f, secondaryPositionId: noPositionId }));
    }
  }, [isEdit, initialized, noPositionId, form.secondaryPositionId]);

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!form.companyId || !form.memberId || !form.primaryPositionId) return;
    const dto: CreateBoardMemberDto = {
      ...form,
      secondaryPositionId: form.secondaryPositionId || null,
    };
    if (isEdit) {
      update({ id: id!, dto }, { onSuccess: () => void navigate(`/board/${id}`) });
    } else {
      create(dto, { onSuccess: (res) => void navigate(`/board/${res.id}`) });
    }
  }

  const canSubmit =
    !isPending && !!form.companyId && !!form.memberId && !!form.primaryPositionId;

  return (
    <div>
      <Link
        to={isEdit ? `/board/${id}` : "/board"}
        className="mb-5 inline-flex items-center gap-1 text-sm text-muted-foreground hover:text-foreground"
      >
        ← {isEdit ? "Volver al detalle" : "Volver a junta directiva"}
      </Link>
      <PageHeader title={isEdit ? "Editar miembro" : "Nuevo miembro"} />
      <form onSubmit={handleSubmit} className="mt-4 max-w-lg space-y-4">
        <div className="space-y-2">
          <Label>
            Empresa <span className="text-destructive">*</span>
          </Label>
          <SearchableCombobox
            value={form.companyId || null}
            onChange={(v) => setForm((f) => ({ ...f, companyId: v ?? "" }))}
            options={companyOptions}
            isLoading={companiesLoading}
            onSearchChange={handleCompanySearch}
            placeholder="Buscar empresa..."
          />
        </div>
        <div className="space-y-2">
          <Label>
            Miembro <span className="text-destructive">*</span>
          </Label>
          <SearchableCombobox
            value={form.memberId || null}
            onChange={(v) => setForm((f) => ({ ...f, memberId: v ?? "" }))}
            options={memberOptions}
            isLoading={membersLoading}
            onSearchChange={handleMemberSearch}
            placeholder="Buscar persona natural..."
          />
        </div>
        <div className="space-y-2">
          <Label>
            Cargo principal <span className="text-destructive">*</span>
          </Label>
          <SelectField
            value={form.primaryPositionId || null}
            onChange={(v) => setForm((f) => ({ ...f, primaryPositionId: v }))}
            options={positionOptions}
            placeholder="Seleccionar cargo..."
          />
        </div>
        <div className="space-y-2">
          <Label>Cargo secundario</Label>
          <SelectField
            value={form.secondaryPositionId || null}
            onChange={(v) => setForm((f) => ({ ...f, secondaryPositionId: v }))}
            options={positionOptions}
            placeholder="Seleccionar cargo secundario..."
          />
        </div>
        <div className="mt-6 flex gap-2">
          <ActionButton type="submit" isLoading={isPending} disabled={!canSubmit}>
            {isEdit ? "Guardar cambios" : "Crear miembro"}
          </ActionButton>
          <Button type="button" variant="ghost" onClick={() => void navigate(-1)}>
            Cancelar
          </Button>
        </div>
      </form>
    </div>
  );
}

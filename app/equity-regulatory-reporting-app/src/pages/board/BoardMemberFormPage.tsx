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

interface Props {
  lockedCompanyId: string;
  lockedCompanyName: string;
  basePath: string;
}

const PAGE_SIZE = 25;
const POSITIONS_PAGE_SIZE = 100;

function emptyForm(lockedCompanyId: string): CreateBoardMemberDto {
  return {
    companyId: lockedCompanyId,
    memberId: "",
    primaryPositionId: "",
    secondaryPositionId: null,
  };
}

export function BoardMemberFormPage({ lockedCompanyId, lockedCompanyName, basePath }: Props) {
  const { boardMemberId } = useParams<{ boardMemberId: string }>();
  const isEdit = !!boardMemberId;
  const navigate = useNavigate();
  const [initialized, setInitialized] = useState(false);
  const [form, setForm] = useState<CreateBoardMemberDto>(emptyForm(lockedCompanyId));
  const [memberSearch, setMemberSearch] = useState("");

  const { data: editData } = useBoardMemberDetailQuery(boardMemberId ?? "");
  const { mutate: create, isPending: isCreating } = useCreateBoardMemberMutation();
  const { mutate: update, isPending: isUpdating } = useUpdateBoardMemberMutation();
  const isPending = isCreating || isUpdating;

  const { data: members, isLoading: membersLoading } = usePersonsQuery({
    page: 1,
    pageSize: PAGE_SIZE,
    search: memberSearch || undefined,
    personType: PersonType.Natural,
  });

  const { data: positions } = usePositionsQuery({ page: 1, pageSize: POSITIONS_PAGE_SIZE });

  const noPositionId = positions?.items.find((p) => p.name === NO_POSITION_NAME)?.id ?? "";

  const memberOptions: ComboboxOption[] = (members?.items ?? []).map((p) => ({
    id: p.id,
    label: p.name,
    sublabel: p.documentNumber,
  }));

  const positionOptions = (positions?.items ?? []).map((p) => ({
    value: p.id,
    label: p.name,
  }));

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
      update(
        { id: boardMemberId!, dto },
        { onSuccess: () => void navigate(`${basePath}/${boardMemberId}`) },
      );
    } else {
      create(dto, { onSuccess: (res) => void navigate(`${basePath}/${res.id}`) });
    }
  }

  const canSubmit = !isPending && !!form.memberId && !!form.primaryPositionId;

  return (
    <div>
      <Link
        to={isEdit ? `${basePath}/${boardMemberId}` : basePath}
        className="mb-5 inline-flex items-center gap-1 text-sm text-muted-foreground hover:text-foreground"
      >
        ← {isEdit ? "Volver al detalle" : "Volver a junta directiva"}
      </Link>
      <PageHeader title={isEdit ? "Editar miembro" : "Nuevo miembro"} />
      <form onSubmit={handleSubmit} className="mt-4 max-w-lg space-y-4">
        <div className="space-y-2">
          <Label>Empresa</Label>
          <p className="flex h-9 items-center rounded-md border bg-muted px-3 text-sm font-medium text-muted-foreground">
            {lockedCompanyName}
          </p>
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

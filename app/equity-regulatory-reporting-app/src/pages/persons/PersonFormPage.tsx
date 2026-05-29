import { useCallback, useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import { Check } from "lucide-react";
import { PageHeader } from "@/components/shared/PageHeader";
import { ActionButton } from "@/elements/ActionButton";
import { FieldTip } from "@/elements/FieldTip";
import { SelectField } from "@/elements/SelectField";
import { SearchableCombobox, type ComboboxOption } from "@/elements/SearchableCombobox";
import {
  PERSON_TYPE_OPTIONS,
  PersonType,
  personTypeLabel,
  type PersonTypeValue,
} from "@/lib/person-types";
import { useDebounce } from "@/hooks/useDebounce";
import { usePersonDetailQuery, useCreatePersonMutation, useUpdatePersonMutation } from "@/hooks/usePersons";
import { usePersonsQuery } from "@/hooks/usePersons";
import { useCountriesQuery } from "@/hooks/useCountries";
import { useDocumentTypesQuery } from "@/hooks/useDocumentTypes";
import type { CreatePersonDto } from "@/types/person";

const CIIU_REGEX = /^\d{4}$/;
const PAGE_SIZE = 25;

function emptyForm(): CreatePersonDto {
  return {
    name: "",
    personType: PersonType.Natural,
    ciiu: "",
    address: "",
    documentTypeId: "",
    documentNumber: "",
    entityCode: null,
    representativeId: null,
    reportFlag: false,
    countryId: "",
    internalLocation: "",
  };
}

export function PersonFormPage() {
  const { id } = useParams<{ id: string }>();
  const isEdit = !!id;
  const navigate = useNavigate();
  const [initialized, setInitialized] = useState(false);
  const [form, setForm] = useState<CreatePersonDto>(emptyForm());
  const [docNumError, setDocNumError] = useState<string | null>(null);
  const [repSearch, setRepSearch] = useState("");
  const [countrySearch, setCountrySearch] = useState("");

  const { data: editData } = usePersonDetailQuery(id ?? "");
  const { mutate: create, isPending: isCreating } = useCreatePersonMutation();
  const { mutate: update, isPending: isUpdating } = useUpdatePersonMutation();
  const isPending = isCreating || isUpdating;

  const { data: representatives, isLoading: repLoading } = usePersonsQuery({
    page: 1,
    pageSize: PAGE_SIZE,
    personType: PersonType.Natural,
    search: repSearch || undefined,
  });

  const { data: countries, isLoading: countriesLoading } = useCountriesQuery({
    page: 1,
    pageSize: PAGE_SIZE,
    search: countrySearch || undefined,
  });

  const { data: documentTypes } = useDocumentTypesQuery({
    page: 1,
    pageSize: PAGE_SIZE,
  });

  const filteredDocTypes = documentTypes?.items.filter((dt) =>
    dt.allowedPersonTypes.includes(form.personType),
  ) ?? [];

  const countryOptions: ComboboxOption[] = (countries?.items ?? []).map((c) => ({
    id: c.id,
    label: c.name,
    sublabel: c.abbreviation,
  }));

  const repOptions: ComboboxOption[] = (representatives?.items ?? []).map((p) => ({
    id: p.id,
    label: p.name,
    sublabel: p.documentNumber,
  }));

  const requiresRepresentative =
    form.personType === PersonType.Legal || form.personType === PersonType.LegalEntity;

  const selectedDocType = documentTypes?.items.find((dt) => dt.id === form.documentTypeId);

  const debouncedDocNumber = useDebounce(form.documentNumber, 400);

  const docNumValid =
    form.documentNumber.length > 0 &&
    (!selectedDocType?.validationRegex ||
      new RegExp(selectedDocType.validationRegex).test(form.documentNumber));

  useEffect(() => {
    if (!debouncedDocNumber) { setDocNumError(null); return; }
    if (!selectedDocType?.validationRegex) { setDocNumError(null); return; }
    const regex = new RegExp(selectedDocType.validationRegex);
    setDocNumError(regex.test(debouncedDocNumber) ? null : "Formato inválido para este tipo de documento.");
  }, [debouncedDocNumber, selectedDocType?.validationRegex]);

  const handleRepSearch = useCallback((s: string) => setRepSearch(s), []);
  const handleCountrySearch = useCallback((s: string) => setCountrySearch(s), []);

  useEffect(() => {
    if (!isEdit || initialized || !editData) return;
    setForm({
      name: editData.name,
      personType: editData.personType,
      ciiu: editData.ciiu,
      address: editData.address,
      documentTypeId: editData.documentTypeId,
      documentNumber: editData.documentNumber,
      entityCode: editData.entityCode,
      representativeId: editData.representativeId,
      reportFlag: editData.reportFlag,
      countryId: editData.countryId,
      internalLocation: editData.internalLocation,
    });
    setInitialized(true);
  }, [isEdit, editData, initialized]);

  function handlePersonTypeChange(value: PersonTypeValue) {
    setForm((f) => ({
      ...f,
      personType: value,
      documentTypeId: "",
      documentNumber: "",
      reportFlag: value === PersonType.Natural ? false : f.reportFlag,
      representativeId: value === PersonType.Natural ? null : f.representativeId,
    }));
    setDocNumError(null);
  }

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (docNumError) return;
    if (requiresRepresentative && !form.representativeId) return;
    const dto: CreatePersonDto = { ...form, entityCode: form.entityCode || null };
    if (isEdit) {
      update({ id: id!, dto }, { onSuccess: () => void navigate(`/persons/${id}`) });
    } else {
      create(dto, { onSuccess: (res) => void navigate(`/persons/${res.id}`) });
    }
  }

  return (
    <div>
      <Link
        to={isEdit ? `/persons/${id}` : "/persons"}
        className="mb-5 inline-flex items-center gap-1 text-sm text-muted-foreground hover:text-foreground"
      >
        ← {isEdit ? "Volver al detalle" : "Volver a personas"}
      </Link>
      <PageHeader title={isEdit ? "Editar persona" : "Nueva persona"} />
      <form onSubmit={handleSubmit} className="mt-4 max-w-lg space-y-4">
        <div className="space-y-2">
          <Label>Nombre / Razón social</Label>
          <Input
            value={form.name}
            onChange={(e) => setForm((f) => ({ ...f, name: e.target.value }))}
            required
          />
        </div>
        <div className="space-y-2">
          <Label>Tipo de persona</Label>
          <SelectField
            value={form.personType}
            onChange={handlePersonTypeChange}
            options={PERSON_TYPE_OPTIONS.map((o) => ({ value: o.value, label: personTypeLabel(o.value) }))}
          />
        </div>
        <div className="space-y-2">
          <Label>Tipo de documento</Label>
          <SelectField
            value={form.documentTypeId || null}
            onChange={(v) => setForm((f) => ({ ...f, documentTypeId: String(v), documentNumber: "" }))}
            options={filteredDocTypes.map((dt) => ({ value: dt.id, label: `${dt.abbreviation} — ${dt.name}` }))}
            placeholder="Seleccionar tipo..."
          />
        </div>
        <div className="space-y-2">
          <Label>Número de documento</Label>
          <div className="relative">
            <Input
              value={form.documentNumber}
              onChange={(e) => setForm((f) => ({ ...f, documentNumber: e.target.value }))}
              className={docNumValid ? "pr-8" : undefined}
              required
            />
            {docNumValid && (
              <Check className="pointer-events-none absolute right-2.5 top-1/2 size-4 -translate-y-1/2 text-foreground" />
            )}
          </div>
          {docNumError && <p className="text-xs text-destructive">{docNumError}</p>}
        </div>
        <div className="space-y-2">
          <Label>CIIU</Label>
          <Input
            value={form.ciiu}
            maxLength={4}
            placeholder="0000"
            onChange={(e) => setForm((f) => ({ ...f, ciiu: e.target.value }))}
            pattern="\d{4}"
            required
          />
          <FieldTip>Código de 4 dígitos numéricos.</FieldTip>
          {form.ciiu && !CIIU_REGEX.test(form.ciiu) && (
            <p className="text-xs text-destructive">Debe ser exactamente 4 dígitos.</p>
          )}
        </div>
        <div className="space-y-2">
          <Label>País</Label>
          <SearchableCombobox
            value={form.countryId || null}
            onChange={(v) => setForm((f) => ({ ...f, countryId: v ?? "" }))}
            options={countryOptions}
            isLoading={countriesLoading}
            onSearchChange={handleCountrySearch}
            placeholder="Buscar país..."
          />
        </div>
        <div className="space-y-2">
          <Label>Dirección</Label>
          <Input
            value={form.address}
            onChange={(e) => setForm((f) => ({ ...f, address: e.target.value }))}
            required
          />
        </div>
        <div className="space-y-2">
          <Label>Ubicación interna en el país</Label>
          <Input
            value={form.internalLocation}
            onChange={(e) => setForm((f) => ({ ...f, internalLocation: e.target.value }))}
            required
          />
        </div>
        <div className="space-y-2">
          <Label>Código entidad (opcional)</Label>
          <Input
            value={form.entityCode ?? ""}
            onChange={(e) => setForm((f) => ({ ...f, entityCode: e.target.value || null }))}
          />
        </div>
        {requiresRepresentative && (
          <div className="space-y-2">
            <Label>
              Representante <span className="text-destructive">*</span>
            </Label>
            <SearchableCombobox
              value={form.representativeId}
              onChange={(v) => setForm((f) => ({ ...f, representativeId: v }))}
              options={repOptions}
              isLoading={repLoading}
              onSearchChange={handleRepSearch}
              placeholder="Buscar persona natural..."
            />
            {requiresRepresentative && !form.representativeId && (
              <p className="text-xs text-destructive">El representante es obligatorio.</p>
            )}
          </div>
        )}
        {form.personType !== PersonType.Natural && (
          <div className="flex items-center gap-2">
            <div
              role="checkbox"
              aria-checked={form.reportFlag}
              tabIndex={0}
              onClick={() => setForm((f) => ({ ...f, reportFlag: !f.reportFlag }))}
              onKeyDown={(e) => {
                if (e.key === "Enter" || e.key === " ") {
                  e.preventDefault();
                  setForm((f) => ({ ...f, reportFlag: !f.reportFlag }));
                }
              }}
            >
              <Checkbox
                id="reportFlag"
                checked={form.reportFlag}
                className="pointer-events-none"
                tabIndex={-1}
              />
            </div>
            <Label htmlFor="reportFlag" className="cursor-pointer font-normal">
              Incluir como raíz en cálculo de acciones
            </Label>
          </div>
        )}
        <div className="mt-6 flex gap-2">
          <ActionButton
            type="submit"
            isLoading={isPending}
            disabled={
              isPending ||
              !!docNumError ||
              !CIIU_REGEX.test(form.ciiu) ||
              (requiresRepresentative && !form.representativeId)
            }
          >
            {isEdit ? "Guardar cambios" : "Crear persona"}
          </ActionButton>
          <Button type="button" variant="ghost" onClick={() => void navigate(-1)}>
            Cancelar
          </Button>
        </div>
      </form>
    </div>
  );
}

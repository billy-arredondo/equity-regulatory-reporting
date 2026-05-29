import { useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import { PageHeader } from "@/components/shared/PageHeader";
import { ActionButton } from "@/elements/ActionButton";
import { FieldTip } from "@/elements/FieldTip";
import { PERSON_TYPE_OPTIONS, personTypeLabel, type PersonTypeValue } from "@/lib/person-types";
import {
  useDocumentTypeDetailQuery,
  useCreateDocumentTypeMutation,
  useUpdateDocumentTypeMutation,
} from "@/hooks/useDocumentTypes";
import type { CreateDocumentTypeDto } from "@/types/document-type";

function emptyForm(): CreateDocumentTypeDto {
  return { name: "", abbreviation: "", validationRegex: null, allowedPersonTypes: [] };
}

export function DocumentTypeFormPage() {
  const { id } = useParams<{ id: string }>();
  const isEdit = !!id;
  const navigate = useNavigate();
  const [initialized, setInitialized] = useState(false);
  const [form, setForm] = useState<CreateDocumentTypeDto>(emptyForm());

  const { data } = useDocumentTypeDetailQuery(id ?? "");
  const { mutate: create, isPending: isCreating } = useCreateDocumentTypeMutation();
  const { mutate: update, isPending: isUpdating } = useUpdateDocumentTypeMutation();
  const isPending = isCreating || isUpdating;

  useEffect(() => {
    if (!isEdit || initialized || !data) return;
    setForm({
      name: data.name,
      abbreviation: data.abbreviation,
      validationRegex: data.validationRegex,
      allowedPersonTypes: data.allowedPersonTypes,
    });
    setInitialized(true);
  }, [isEdit, data, initialized]);

  function togglePersonType(value: PersonTypeValue) {
    setForm((f) => ({
      ...f,
      allowedPersonTypes: f.allowedPersonTypes.includes(value)
        ? f.allowedPersonTypes.filter((v) => v !== value)
        : [...f.allowedPersonTypes, value],
    }));
  }

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    const dto = { ...form, validationRegex: form.validationRegex || null };
    if (isEdit) {
      update({ id: id!, dto }, { onSuccess: () => void navigate(`/document-types/${id}`) });
    } else {
      create(dto, { onSuccess: (res) => void navigate(`/document-types/${res.id}`) });
    }
  }

  return (
    <div>
      <Link
        to={isEdit ? `/document-types/${id}` : "/document-types"}
        className="mb-5 inline-flex items-center gap-1 text-sm text-muted-foreground hover:text-foreground"
      >
        ← {isEdit ? "Volver al detalle" : "Volver a tipos de documento"}
      </Link>
      <PageHeader title={isEdit ? "Editar tipo de documento" : "Nuevo tipo de documento"} />
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
            onChange={(e) => setForm((f) => ({ ...f, abbreviation: e.target.value }))}
            required
          />
        </div>
        <div className="space-y-2">
          <Label htmlFor="validationRegex">Validación (expresión regular)</Label>
          <Input
            id="validationRegex"
            value={form.validationRegex ?? ""}
            placeholder="Ej. ^\d{8}$"
            onChange={(e) => setForm((f) => ({ ...f, validationRegex: e.target.value || null }))}
          />
          <FieldTip>Opcional. Expresión regular para validar el número de documento.</FieldTip>
        </div>
        <div className="space-y-2">
          <Label>Tipos de persona habilitados</Label>
          <div className="space-y-2">
            {PERSON_TYPE_OPTIONS.map((opt) => (
              <div key={opt.value} className="flex items-center gap-2">
                <div
                  role="checkbox"
                  aria-checked={form.allowedPersonTypes.includes(opt.value)}
                  tabIndex={0}
                  onClick={() => togglePersonType(opt.value)}
                  onKeyDown={(e) => {
                    if (e.key === "Enter" || e.key === " ") {
                      e.preventDefault();
                      togglePersonType(opt.value);
                    }
                  }}
                >
                  <Checkbox
                    id={`pt-${opt.value}`}
                    checked={form.allowedPersonTypes.includes(opt.value)}
                    className="pointer-events-none"
                    tabIndex={-1}
                  />
                </div>
                <Label htmlFor={`pt-${opt.value}`} className="cursor-pointer font-normal">
                  {personTypeLabel(opt.value)}
                </Label>
              </div>
            ))}
          </div>
        </div>
        <div className="mt-6 flex gap-2">
          <ActionButton type="submit" isLoading={isPending}>
            {isEdit ? "Guardar cambios" : "Crear tipo de documento"}
          </ActionButton>
          <Button type="button" variant="ghost" onClick={() => void navigate(-1)}>
            Cancelar
          </Button>
        </div>
      </form>
    </div>
  );
}

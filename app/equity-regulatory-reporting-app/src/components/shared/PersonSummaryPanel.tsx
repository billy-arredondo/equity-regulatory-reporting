import { Users } from "lucide-react";
import { personTypeLabel } from "@/lib/person-types";
import { SummaryPanel, SummaryRow } from "./SummaryPanel";
import { usePersonDetailQuery } from "@/hooks/usePersons";

interface Props {
  personId: string;
  open?: boolean;
  onOpenChange?: (open: boolean) => void;
}

export function PersonSummaryPanel({ personId, open, onOpenChange }: Props) {
  const { data, isLoading } = usePersonDetailQuery(personId);
  return (
    <SummaryPanel
      icon={<Users className="h-4 w-4 text-muted-foreground" />}
      title="Ficha de la persona"
      isLoading={isLoading}
      hasData={!!data}
      detailPath={`/persons/${personId}`}
      open={open}
      onOpenChange={onOpenChange}
    >
      {data && (
        <>
          <SummaryRow label="Nombre" value={data.name} />
          <SummaryRow label="Tipo" value={personTypeLabel(data.personType)} />
          <SummaryRow label="Documento" value={`${data.documentTypeName} ${data.documentNumber}`} />
          <SummaryRow label="País" value={data.countryName} />
          {data.representativeName && (
            <SummaryRow label="Representante" value={data.representativeName} />
          )}
        </>
      )}
    </SummaryPanel>
  );
}

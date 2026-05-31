import { FileText } from "lucide-react";
import { Badge } from "@/components/ui/badge";
import { personTypeLabel } from "@/lib/person-types";
import { SummaryPanel, SummaryRow } from "./SummaryPanel";
import { useDocumentTypeDetailQuery } from "@/hooks/useDocumentTypes";

interface Props {
  documentTypeId: string;
  open?: boolean;
  onOpenChange?: (open: boolean) => void;
}

export function DocumentTypeSummaryPanel({ documentTypeId, open, onOpenChange }: Props) {
  const { data, isLoading } = useDocumentTypeDetailQuery(documentTypeId);
  return (
    <SummaryPanel
      icon={<FileText className="h-4 w-4 text-muted-foreground" />}
      title="Ficha del tipo de documento"
      isLoading={isLoading}
      hasData={!!data}
      detailPath={`/document-types/${documentTypeId}`}
      open={open}
      onOpenChange={onOpenChange}
    >
      {data && (
        <>
          <SummaryRow label="Nombre" value={data.name} />
          <SummaryRow label="Abreviatura" value={data.abbreviation} />
          <SummaryRow
            label="Validación"
            value={data.validationRegex ?? "—"}
          />
          <div className="space-y-0.5">
            <p className="text-xs text-muted-foreground">Tipos de persona</p>
            <div className="flex flex-wrap gap-1">
              {data.allowedPersonTypes.map((t) => (
                <Badge key={t} variant="secondary">
                  {personTypeLabel(t)}
                </Badge>
              ))}
            </div>
          </div>
        </>
      )}
    </SummaryPanel>
  );
}

import { BookOpen } from "lucide-react";
import { SummaryPanel, SummaryRow } from "./SummaryPanel";
import { usePositionDetailQuery } from "@/hooks/usePositions";

interface Props {
  positionId: string;
  open?: boolean;
  onOpenChange?: (open: boolean) => void;
}

export function PositionSummaryPanel({ positionId, open, onOpenChange }: Props) {
  const { data, isLoading } = usePositionDetailQuery(positionId);
  return (
    <SummaryPanel
      icon={<BookOpen className="h-4 w-4 text-muted-foreground" />}
      title="Ficha del cargo"
      isLoading={isLoading}
      hasData={!!data}
      detailPath={`/positions/${positionId}`}
      open={open}
      onOpenChange={onOpenChange}
    >
      {data && (
        <>
          <SummaryRow label="Nombre" value={data.name} />
          <SummaryRow label="Código de reporte" value={data.reportCode} />
        </>
      )}
    </SummaryPanel>
  );
}

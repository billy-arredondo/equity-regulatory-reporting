import { Percent } from "lucide-react";
import { SummaryPanel, SummaryRow } from "./SummaryPanel";
import { useParticipationDetailQuery } from "@/hooks/useParticipations";

interface Props {
  participationId: string;
  basePath: string;
  open?: boolean;
  onOpenChange?: (open: boolean) => void;
}

function formatDate(d: string) {
  return new Date(d + "T00:00:00").toLocaleDateString("es-CO", {
    year: "numeric",
    month: "short",
    day: "numeric",
  });
}

export function ParticipationSummaryPanel({ participationId, basePath, open, onOpenChange }: Props) {
  const { data, isLoading } = useParticipationDetailQuery(participationId);
  return (
    <SummaryPanel
      icon={<Percent className="h-4 w-4 text-muted-foreground" />}
      title="Ficha de participación"
      isLoading={isLoading}
      hasData={!!data}
      detailPath={`${basePath}/${participationId}`}
      open={open}
      onOpenChange={onOpenChange}
    >
      {data && (
        <>
          <SummaryRow label="Empresa" value={data.companyName} />
          <SummaryRow label="Accionista" value={data.shareholderName} />
          <SummaryRow label="Porcentaje" value={`${data.percentage}%`} />
          <SummaryRow label="Desde" value={formatDate(data.effectiveFrom)} />
          <SummaryRow
            label="Hasta"
            value={data.effectiveTo ? formatDate(data.effectiveTo) : "Vigente"}
          />
        </>
      )}
    </SummaryPanel>
  );
}

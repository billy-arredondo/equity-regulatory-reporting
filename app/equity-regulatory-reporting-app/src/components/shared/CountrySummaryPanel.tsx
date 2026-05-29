import { Globe } from "lucide-react";
import { SummaryPanel, SummaryRow } from "./SummaryPanel";
import { useCountryDetailQuery } from "@/hooks/useCountries";

interface Props {
  countryId: string;
  open?: boolean;
  onOpenChange?: (open: boolean) => void;
}

export function CountrySummaryPanel({ countryId, open, onOpenChange }: Props) {
  const { data, isLoading } = useCountryDetailQuery(countryId);
  return (
    <SummaryPanel
      icon={<Globe className="h-4 w-4 text-muted-foreground" />}
      title="Ficha del país"
      isLoading={isLoading}
      hasData={!!data}
      detailPath={`/countries/${countryId}`}
      open={open}
      onOpenChange={onOpenChange}
    >
      {data && (
        <>
          <SummaryRow label="Nombre" value={data.name} />
          <SummaryRow label="Abreviatura" value={data.abbreviation} />
        </>
      )}
    </SummaryPanel>
  );
}

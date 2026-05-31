import { Link } from "react-router-dom";
import { Badge } from "@/components/ui/badge";
import { personTypeLabel } from "@/lib/person-types";
import type { PersonDetailDto } from "@/types/person";

interface Props {
  data: PersonDetailDto;
}

export function PersonDetailFields({ data }: Props) {
  return (
    <div className="mt-4 max-w-lg grid grid-cols-2 gap-3">
      <div className="space-y-1.5">
        <p className="text-xs text-muted-foreground">Tipo de persona</p>
        <p className="flex h-9 items-center text-sm font-medium">{personTypeLabel(data.personType)}</p>
      </div>
      <div className="space-y-1.5">
        <p className="text-xs text-muted-foreground">CIIU</p>
        <p className="flex h-9 items-center font-mono text-sm font-medium">{data.ciiu}</p>
      </div>
      <div className="space-y-1.5">
        <p className="text-xs text-muted-foreground">Tipo de documento</p>
        <p className="flex h-9 items-center text-sm font-medium">{data.documentTypeName}</p>
      </div>
      <div className="space-y-1.5">
        <p className="text-xs text-muted-foreground">Número de documento</p>
        <p className="flex h-9 items-center text-sm font-medium">{data.documentNumber}</p>
      </div>
      <div className="col-span-2 space-y-1.5">
        <p className="text-xs text-muted-foreground">Dirección</p>
        <p className="flex h-9 items-center text-sm font-medium">{data.address}</p>
      </div>
      <div className="space-y-1.5">
        <p className="text-xs text-muted-foreground">País</p>
        <p className="flex h-9 items-center text-sm font-medium">{data.countryName}</p>
      </div>
      <div className="space-y-1.5">
        <p className="text-xs text-muted-foreground">Ubicación interna</p>
        <p className="flex h-9 items-center text-sm font-medium">{data.internalLocation}</p>
      </div>
      {data.entityCode && (
        <div className="space-y-1.5">
          <p className="text-xs text-muted-foreground">Código entidad</p>
          <p className="flex h-9 items-center text-sm font-medium">{data.entityCode}</p>
        </div>
      )}
      {data.representativeName && (
        <div className="space-y-1.5">
          <p className="text-xs text-muted-foreground">Representante</p>
          <p className="flex h-9 items-center text-sm">
            <Link
              to={`/people/${data.representativeId}`}
              className="font-medium hover:underline"
            >
              {data.representativeName}
            </Link>
          </p>
        </div>
      )}
      <div className="space-y-1.5">
        <p className="text-xs text-muted-foreground">Incluir en reporte</p>
        <div className="flex h-9 items-center">
          <Badge variant={data.reportFlag ? "default" : "secondary"}>
            {data.reportFlag ? "Sí" : "No"}
          </Badge>
        </div>
      </div>
    </div>
  );
}

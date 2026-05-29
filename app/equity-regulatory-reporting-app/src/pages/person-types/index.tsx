import { DataTable, type Column } from "@/components/shared/DataTable";
import { PageHeader } from "@/components/shared/PageHeader";
import { usePersonTypesQuery } from "@/hooks/usePersonTypes";
import type { PersonTypeDto } from "@/types/person-type";

const columns: Column<PersonTypeDto>[] = [
  { key: "id", header: "ID", render: (r) => r.id, priority: "medium" },
  { key: "name", header: "Nombre", render: (r) => r.name, priority: "high" },
];

export function PersonTypesPage() {
  const { data = [], isLoading } = usePersonTypesQuery();

  return (
    <div>
      <PageHeader title="Tipos de persona" />
      <p className="mb-4 text-sm text-muted-foreground">
        Valores de enumeración del sistema. Solo lectura.
      </p>
      <DataTable
        data={data}
        columns={columns}
        isLoading={isLoading}
        page={1}
        pageSize={data.length || 3}
        totalCount={data.length}
        onPageChange={() => undefined}
        rowKey={(r) => String(r.id)}
      />
    </div>
  );
}

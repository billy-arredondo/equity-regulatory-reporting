import { useState } from "react";
import { Link } from "react-router-dom";
import { Plus } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Badge } from "@/components/ui/badge";
import { DataTable, type Column } from "@/components/shared/DataTable";
import { PageHeader } from "@/components/shared/PageHeader";
import { PersonSummaryPanel } from "@/components/shared/PersonSummaryPanel";
import { PermissionGuard } from "@/components/shared/PermissionGuard";
import { useMediaQuery } from "@/hooks/useMediaQuery";
import { usePersonsQuery } from "@/hooks/usePersons";
import { Permission } from "@/lib/permissions";
import { PERSON_TYPE_OPTIONS, personTypeLabel, type PersonTypeValue } from "@/lib/person-types";
import type { PersonDto } from "@/types/person";

const columns: Column<PersonDto>[] = [
  { key: "name", header: "Nombre", render: (r) => r.name, priority: "high" },
  {
    key: "personType",
    header: "Tipo",
    render: (r) => personTypeLabel(r.personType),
    priority: "medium",
  },
  {
    key: "documentNumber",
    header: "Documento",
    render: (r) => r.documentNumber,
    priority: "medium",
  },
  {
    key: "reportFlag",
    header: "En reporte",
    render: (r) => (
      <Badge variant={r.reportFlag ? "default" : "secondary"}>
        {r.reportFlag ? "Sí" : "No"}
      </Badge>
    ),
    priority: "low",
  },
];

const PAGE_SIZE = 25;

export function PersonListPage() {
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState("");
  const [personType, setPersonType] = useState<PersonTypeValue | "">("");
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const [panelOpen, setPanelOpen] = useState(false);
  const isDesktop = useMediaQuery("(min-width: 1024px)");

  const { data, isLoading } = usePersonsQuery({
    page,
    pageSize: PAGE_SIZE,
    search: search || undefined,
    personType: personType !== "" ? personType : undefined,
  });

  function handleRowClick(row: PersonDto) {
    setSelectedId(row.id);
    if (!isDesktop) setPanelOpen(true);
  }

  return (
    <div>
      <PageHeader
        title="Personas"
        actions={
          <PermissionGuard perm={Permission.PersonWrite}>
            <Button size="sm" render={<Link to="/persons/new" />}>
              <Plus className="mr-1 h-4 w-4" />
              <span className="hidden sm:inline">Nueva persona</span>
            </Button>
          </PermissionGuard>
        }
      />
      <div className="mb-4 flex flex-wrap gap-2">
        <Input
          placeholder="Buscar personas..."
          value={search}
          onChange={(e) => {
            setSearch(e.target.value);
            setPage(1);
          }}
          className="max-w-xs"
        />
        <select
          value={personType}
          onChange={(e) => {
            setPersonType(e.target.value as PersonTypeValue | "");
            setPage(1);
          }}
          className="h-9 rounded-md border border-input bg-background px-3 text-sm"
        >
          <option value="">Todos los tipos</option>
          {PERSON_TYPE_OPTIONS.map((o) => (
            <option key={o.value} value={o.value}>
              {personTypeLabel(o.value)}
            </option>
          ))}
        </select>
      </div>
      <div className="grid grid-cols-1 gap-6 lg:grid-cols-[1fr_288px] lg:items-start">
        <DataTable
          data={data?.items ?? []}
          columns={columns}
          isLoading={isLoading}
          page={page}
          pageSize={PAGE_SIZE}
          totalCount={data?.totalCount ?? 0}
          onPageChange={setPage}
          onRowClick={handleRowClick}
          isRowSelected={(row) => row.id === selectedId}
          rowKey={(row) => row.id}
        />
        {isDesktop ? (
          selectedId ? (
            <div className="lg:sticky lg:top-4">
              <PersonSummaryPanel personId={selectedId} />
            </div>
          ) : (
            <div className="lg:sticky lg:top-4 flex items-center justify-center rounded-md border border-dashed px-4 py-12 text-center text-sm text-muted-foreground">
              Selecciona un elemento para ver su ficha
            </div>
          )
        ) : (
          selectedId && (
            <PersonSummaryPanel
              personId={selectedId}
              open={panelOpen}
              onOpenChange={setPanelOpen}
            />
          )
        )}
      </div>
    </div>
  );
}

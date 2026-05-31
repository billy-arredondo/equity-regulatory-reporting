import { useCallback, useState } from "react";
import { Link } from "react-router-dom";
import { Plus } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { DataTable, type Column } from "@/components/shared/DataTable";
import { PageHeader } from "@/components/shared/PageHeader";
import { ParticipationSummaryPanel } from "@/components/shared/ParticipationSummaryPanel";
import { PermissionGuard } from "@/components/shared/PermissionGuard";
import { SearchableCombobox, type ComboboxOption } from "@/elements/SearchableCombobox";
import { useMediaQuery } from "@/hooks/useMediaQuery";
import { useParticipationsQuery } from "@/hooks/useParticipations";
import { usePersonsQuery } from "@/hooks/usePersons";
import { Permission } from "@/lib/permissions";
import { PersonType } from "@/lib/person-types";
import type { ParticipationDto } from "@/types/participation";

function formatDate(d: string) {
  return new Date(d + "T00:00:00").toLocaleDateString("es-CO", {
    year: "numeric",
    month: "short",
    day: "numeric",
  });
}

const columns: Column<ParticipationDto>[] = [
  { key: "companyName", header: "Empresa", render: (r) => r.companyName, priority: "high" },
  { key: "shareholderName", header: "Accionista", render: (r) => r.shareholderName, priority: "high" },
  {
    key: "percentage",
    header: "Porcentaje",
    render: (r) => `${r.percentage}%`,
    priority: "medium",
  },
  {
    key: "effectiveFrom",
    header: "Desde",
    render: (r) => formatDate(r.effectiveFrom),
    priority: "medium",
  },
  {
    key: "effectiveTo",
    header: "Hasta",
    render: (r) => (r.effectiveTo ? formatDate(r.effectiveTo) : "Vigente"),
    priority: "low",
  },
];

const PAGE_SIZE = 25;

export function ParticipationListPage() {
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState("");
  const [companyId, setCompanyId] = useState<string | null>(null);
  const [companySearch, setCompanySearch] = useState("");
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const [panelOpen, setPanelOpen] = useState(false);
  const isDesktop = useMediaQuery("(min-width: 1024px)");

  const { data: companies, isLoading: companiesLoading } = usePersonsQuery({
    page: 1,
    pageSize: PAGE_SIZE,
    search: companySearch || undefined,
    personType: PersonType.Legal,
  });

  const companyOptions: ComboboxOption[] = (companies?.items ?? []).map((p) => ({
    id: p.id,
    label: p.name,
    sublabel: p.documentNumber,
  }));

  const handleCompanySearch = useCallback((s: string) => setCompanySearch(s), []);

  const { data, isLoading } = useParticipationsQuery({
    page,
    pageSize: PAGE_SIZE,
    search: search || undefined,
    companyId: companyId ?? undefined,
  });

  function handleRowClick(row: ParticipationDto) {
    setSelectedId(row.id);
    if (!isDesktop) setPanelOpen(true);
  }

  return (
    <div>
      <PageHeader
        title="Participaciones"
        actions={
          <PermissionGuard perm={Permission.ParticipationWrite}>
            <Button size="sm" render={<Link to="/participations/new" />}>
              <Plus className="mr-1 h-4 w-4" />
              <span className="hidden sm:inline">Nueva participación</span>
            </Button>
          </PermissionGuard>
        }
      />
      <div className="mb-4 flex flex-wrap gap-2">
        <Input
          placeholder="Buscar participaciones..."
          value={search}
          onChange={(e) => {
            setSearch(e.target.value);
            setPage(1);
          }}
          className="max-w-xs"
        />
        <div className="w-64">
          <SearchableCombobox
            value={companyId}
            onChange={(v) => {
              setCompanyId(v);
              setPage(1);
            }}
            options={companyOptions}
            isLoading={companiesLoading}
            onSearchChange={handleCompanySearch}
            placeholder="Filtrar por empresa..."
          />
        </div>
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
              <ParticipationSummaryPanel participationId={selectedId} />
            </div>
          ) : (
            <div className="lg:sticky lg:top-4 flex items-center justify-center rounded-md border border-dashed px-4 py-12 text-center text-sm text-muted-foreground">
              Selecciona un elemento para ver su ficha
            </div>
          )
        ) : (
          selectedId && (
            <ParticipationSummaryPanel
              participationId={selectedId}
              open={panelOpen}
              onOpenChange={setPanelOpen}
            />
          )
        )}
      </div>
    </div>
  );
}

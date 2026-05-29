import { useState } from "react";
import { Link } from "react-router-dom";
import { Check, Plus } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { DataTable, type Column } from "@/components/shared/DataTable";
import { PageHeader } from "@/components/shared/PageHeader";
import { PersonSummaryPanel } from "@/components/shared/PersonSummaryPanel";
import { PermissionGuard } from "@/components/shared/PermissionGuard";
import { useMediaQuery } from "@/hooks/useMediaQuery";
import { usePersonsQuery } from "@/hooks/usePersons";
import { Permission } from "@/lib/permissions";
import { PersonType, type PersonTypeValue } from "@/lib/person-types";
import type { PersonDto } from "@/types/person";

interface Props {
  personType: PersonTypeValue;
  title: string;
  baseRoute: string;
  newLabel: string;
}

const PAGE_SIZE = 25;

export function PersonListPage({ personType, title, baseRoute, newLabel }: Props) {
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState("");
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const [panelOpen, setPanelOpen] = useState(false);
  const isDesktop = useMediaQuery("(min-width: 1024px)");

  const { data, isLoading } = usePersonsQuery({
    page,
    pageSize: PAGE_SIZE,
    search: search || undefined,
    personType,
  });

  const showReportFlag = personType !== PersonType.Natural;

  const columns: Column<PersonDto>[] = [
    { key: "name", header: "Nombre", render: (r) => r.name, priority: "high" },
    { key: "documentNumber", header: "Documento", render: (r) => r.documentNumber, priority: "medium" },
    ...(showReportFlag
      ? [{
          key: "reportFlag" as const,
          header: "En reporte",
          render: (r: PersonDto) => r.reportFlag ? <Check className="size-4" /> : null,
          priority: "low" as const,
        }]
      : []),
  ];

  function handleRowClick(row: PersonDto) {
    setSelectedId(row.id);
    if (!isDesktop) setPanelOpen(true);
  }

  return (
    <div>
      <PageHeader
        title={title}
        actions={
          <PermissionGuard perm={Permission.PersonWrite}>
            <Button size="sm" render={<Link to={`${baseRoute}/new`} />}>
              <Plus className="mr-1 h-4 w-4" />
              <span className="hidden sm:inline">{newLabel}</span>
            </Button>
          </PermissionGuard>
        }
      />
      <div className="mb-4">
        <Input
          placeholder={`Buscar ${title.toLowerCase()}...`}
          value={search}
          onChange={(e) => { setSearch(e.target.value); setPage(1); }}
          className="max-w-xs"
        />
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
              <PersonSummaryPanel personId={selectedId} baseRoute={baseRoute} />
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
              baseRoute={baseRoute}
              open={panelOpen}
              onOpenChange={setPanelOpen}
            />
          )
        )}
      </div>
    </div>
  );
}

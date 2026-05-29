import { useState } from "react";
import { Link } from "react-router-dom";
import { Plus } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Input } from "@/components/ui/input";
import { DataTable, type Column } from "@/components/shared/DataTable";
import { PageHeader } from "@/components/shared/PageHeader";
import { DocumentTypeSummaryPanel } from "@/components/shared/DocumentTypeSummaryPanel";
import { PermissionGuard } from "@/components/shared/PermissionGuard";
import { useMediaQuery } from "@/hooks/useMediaQuery";
import { useDocumentTypesQuery } from "@/hooks/useDocumentTypes";
import { Permission } from "@/lib/permissions";
import { personTypeLabel } from "@/lib/person-types";
import type { DocumentTypeDto } from "@/types/document-type";

const columns: Column<DocumentTypeDto>[] = [
  { key: "name", header: "Nombre", render: (r) => r.name, priority: "high" },
  { key: "abbreviation", header: "Abrev.", render: (r) => r.abbreviation, priority: "medium" },
  {
    key: "allowedPersonTypes",
    header: "Tipos habilitados",
    render: (r) => (
      <div className="flex flex-wrap gap-1">
        {r.allowedPersonTypes.map((t) => (
          <Badge key={t} variant="secondary" className="text-xs">
            {personTypeLabel(t)}
          </Badge>
        ))}
      </div>
    ),
    priority: "low",
  },
];

const PAGE_SIZE = 25;

export function DocumentTypeListPage() {
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState("");
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const [panelOpen, setPanelOpen] = useState(false);
  const isDesktop = useMediaQuery("(min-width: 1024px)");

  const { data, isLoading } = useDocumentTypesQuery({ page, pageSize: PAGE_SIZE, search: search || undefined });

  function handleRowClick(row: DocumentTypeDto) {
    setSelectedId(row.id);
    if (!isDesktop) setPanelOpen(true);
  }

  return (
    <div>
      <PageHeader
        title="Tipos de documento"
        actions={
          <PermissionGuard perm={Permission.DocumentTypeWrite}>
            <Button size="sm" render={<Link to="/document-types/new" />}>
              <Plus className="mr-1 h-4 w-4" />
              <span className="hidden sm:inline">Nuevo tipo</span>
            </Button>
          </PermissionGuard>
        }
      />
      <div className="mb-4">
        <Input
          placeholder="Buscar tipos de documento..."
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
              <DocumentTypeSummaryPanel documentTypeId={selectedId} />
            </div>
          ) : (
            <div className="lg:sticky lg:top-4 flex items-center justify-center rounded-md border border-dashed px-4 py-12 text-center text-sm text-muted-foreground">
              Selecciona un elemento para ver su ficha
            </div>
          )
        ) : (
          selectedId && (
            <DocumentTypeSummaryPanel
              documentTypeId={selectedId}
              open={panelOpen}
              onOpenChange={setPanelOpen}
            />
          )
        )}
      </div>
    </div>
  );
}

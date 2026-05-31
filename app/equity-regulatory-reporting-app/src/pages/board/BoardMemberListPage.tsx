import { useState } from "react";
import { Link } from "react-router-dom";
import { Plus } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { DataTable, type Column } from "@/components/shared/DataTable";
import { BoardMemberSummaryPanel } from "@/components/shared/BoardMemberSummaryPanel";
import { PermissionGuard } from "@/components/shared/PermissionGuard";
import { useMediaQuery } from "@/hooks/useMediaQuery";
import { useBoardMembersQuery } from "@/hooks/useBoardMembers";
import { Permission } from "@/lib/permissions";
import type { BoardMemberDto } from "@/types/board-member";

interface Props {
  companyId: string;
  basePath: string;
}

const columns: Column<BoardMemberDto>[] = [
  { key: "memberName", header: "Miembro", render: (r) => r.memberName, priority: "high" },
  {
    key: "primaryPositionName",
    header: "Cargo principal",
    render: (r) => r.primaryPositionName,
    priority: "medium",
  },
  {
    key: "secondaryPositionName",
    header: "Cargo secundario",
    render: (r) => r.secondaryPositionName,
    priority: "low",
  },
];

const PAGE_SIZE = 25;

export function BoardMemberListPage({ companyId, basePath }: Props) {
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState("");
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const [panelOpen, setPanelOpen] = useState(false);
  const isDesktop = useMediaQuery("(min-width: 1024px)");

  const { data, isLoading } = useBoardMembersQuery({
    page,
    pageSize: PAGE_SIZE,
    search: search || undefined,
    companyId,
  });

  function handleRowClick(row: BoardMemberDto) {
    setSelectedId(row.id);
    if (!isDesktop) setPanelOpen(true);
  }

  return (
    <div>
      <div className="mb-4 flex flex-wrap items-center justify-between gap-2">
        <Input
          placeholder="Buscar miembros..."
          value={search}
          onChange={(e) => {
            setSearch(e.target.value);
            setPage(1);
          }}
          className="max-w-xs"
        />
        <PermissionGuard perm={Permission.BoardWrite}>
          <Button size="sm" render={<Link to={`${basePath}/new`} />}>
            <Plus className="mr-1 h-4 w-4" />
            <span className="hidden sm:inline">Nuevo miembro</span>
          </Button>
        </PermissionGuard>
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
              <BoardMemberSummaryPanel
                boardMemberId={selectedId}
                basePath={basePath}
              />
            </div>
          ) : (
            <div className="lg:sticky lg:top-4 flex items-center justify-center rounded-md border border-dashed px-4 py-12 text-center text-sm text-muted-foreground">
              Selecciona un elemento para ver su ficha
            </div>
          )
        ) : (
          selectedId && (
            <BoardMemberSummaryPanel
              boardMemberId={selectedId}
              basePath={basePath}
              open={panelOpen}
              onOpenChange={setPanelOpen}
            />
          )
        )}
      </div>
    </div>
  );
}

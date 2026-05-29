import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Input } from "@/components/ui/input";
import { DataTable, type Column } from "@/components/shared/DataTable";
import { PageHeader } from "@/components/shared/PageHeader";
import { RoleBadge } from "@/components/shared/RoleBadge";
import { useUsersQuery } from "@/hooks/useUsers";
import type { UserDto } from "@/types/user";

const columns: Column<UserDto>[] = [
  {
    key: "name",
    header: "Nombre",
    render: (r) => `${r.firstName} ${r.lastName}`,
    priority: "high",
  },
  { key: "email", header: "Correo electrónico", render: (r) => r.email, priority: "high" },
  {
    key: "roles",
    header: "Roles",
    render: (r) => (
      <div className="flex flex-wrap gap-1">
        {r.roles.length === 0 ? (
          <span className="text-muted-foreground">—</span>
        ) : (
          r.roles.map((role) => <RoleBadge key={role} role={role} />)
        )}
      </div>
    ),
    priority: "medium",
  },
];

const PAGE_SIZE = 25;

export function UserListPage() {
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState("");
  const navigate = useNavigate();

  const { data, isLoading } = useUsersQuery({ page, pageSize: PAGE_SIZE, search: search || undefined });

  return (
    <div>
      <PageHeader title="Usuarios" />
      <div className="mb-4">
        <Input
          placeholder="Buscar usuarios..."
          value={search}
          onChange={(e) => {
            setSearch(e.target.value);
            setPage(1);
          }}
          className="max-w-xs"
        />
      </div>
      <DataTable
        data={data?.items ?? []}
        columns={columns}
        isLoading={isLoading}
        page={page}
        pageSize={PAGE_SIZE}
        totalCount={data?.totalCount ?? 0}
        onPageChange={setPage}
        onRowClick={(row) => void navigate(`/users/${row.id}`)}
        rowKey={(row) => row.id}
      />
    </div>
  );
}

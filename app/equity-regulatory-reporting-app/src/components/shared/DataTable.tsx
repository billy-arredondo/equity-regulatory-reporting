import { ChevronLeft, ChevronRight } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { cn } from "@/lib/utils";

export interface Column<T> {
  key: string;
  header: string;
  render: (row: T) => React.ReactNode;
  priority?: "high" | "medium" | "low";
}

interface DataTableProps<T> {
  data: T[];
  columns: Column<T>[];
  isLoading?: boolean;
  page: number;
  pageSize: number;
  totalCount: number;
  onPageChange: (page: number) => void;
  onRowClick?: (row: T) => void;
  isRowSelected?: (row: T) => boolean;
  rowKey: (row: T) => string;
}

const priorityClass: Record<string, string> = {
  high: "",
  medium: "hidden md:table-cell",
  low: "hidden lg:table-cell",
};

export function DataTable<T>({
  data,
  columns,
  isLoading,
  page,
  pageSize,
  totalCount,
  onPageChange,
  onRowClick,
  isRowSelected,
  rowKey,
}: DataTableProps<T>) {
  const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));
  const from = totalCount === 0 ? 0 : (page - 1) * pageSize + 1;
  const to = Math.min(page * pageSize, totalCount);

  return (
    <div className="space-y-3">
      <div className="overflow-x-auto rounded-md border">
        <Table>
          <TableHeader>
            <TableRow>
              {columns.map((col) => (
                <TableHead
                  key={col.key}
                  className={priorityClass[col.priority ?? "high"]}
                >
                  {col.header}
                </TableHead>
              ))}
            </TableRow>
          </TableHeader>
          <TableBody>
            {isLoading ? (
              Array.from({ length: pageSize > 5 ? 5 : pageSize }).map((_, i) => (
                <TableRow key={i}>
                  {columns.map((col) => (
                    <TableCell
                      key={col.key}
                      className={priorityClass[col.priority ?? "high"]}
                    >
                      <Skeleton className="h-4 w-full" />
                    </TableCell>
                  ))}
                </TableRow>
              ))
            ) : data.length === 0 ? (
              <TableRow>
                <TableCell
                  colSpan={columns.length}
                  className="h-24 text-center text-muted-foreground"
                >
                  Sin resultados.
                </TableCell>
              </TableRow>
            ) : (
              data.map((row) => {
                const selected = isRowSelected?.(row) ?? false;
                return (
                  <TableRow
                    key={rowKey(row)}
                    onClick={() => onRowClick?.(row)}
                    className={cn(
                      onRowClick && "cursor-pointer",
                      selected
                        ? "bg-accent text-accent-foreground border-l-2 border-l-primary"
                        : onRowClick && "hover:bg-accent/50",
                    )}
                  >
                    {columns.map((col) => (
                      <TableCell
                        key={col.key}
                        className={priorityClass[col.priority ?? "high"]}
                      >
                        {col.render(row)}
                      </TableCell>
                    ))}
                  </TableRow>
                );
              })
            )}
          </TableBody>
        </Table>
      </div>

      <div className="flex items-center justify-between text-sm text-muted-foreground">
        <span className="hidden sm:inline">
          {totalCount === 0
            ? "Sin resultados"
            : `${from}–${to} de ${totalCount}`}
        </span>
        <span className="sm:hidden">{page} / {totalPages}</span>
        <div className="flex items-center gap-1">
          <Button
            variant="outline"
            size="icon"
            className="h-7 w-7"
            disabled={page <= 1 || isLoading}
            onClick={() => onPageChange(page - 1)}
            aria-label="Página anterior"
          >
            <ChevronLeft className="h-4 w-4" />
          </Button>
          <Button
            variant="outline"
            size="icon"
            className="h-7 w-7"
            disabled={page >= totalPages || isLoading}
            onClick={() => onPageChange(page + 1)}
            aria-label="Página siguiente"
          >
            <ChevronRight className="h-4 w-4" />
          </Button>
        </div>
      </div>
    </div>
  );
}

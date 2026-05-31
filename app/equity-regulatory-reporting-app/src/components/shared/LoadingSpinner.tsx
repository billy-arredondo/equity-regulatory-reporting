import { cn } from "@/lib/utils";

interface Props {
  className?: string;
}

export function LoadingSpinner({ className }: Props) {
  return (
    <div
      className={cn(
        "h-6 w-6 animate-spin rounded-full border-2 border-muted border-t-primary",
        className,
      )}
      role="status"
      aria-label="Cargando"
    />
  );
}

export function PageLoading() {
  return (
    <div className="flex h-40 items-center justify-center">
      <LoadingSpinner className="h-8 w-8" />
    </div>
  );
}

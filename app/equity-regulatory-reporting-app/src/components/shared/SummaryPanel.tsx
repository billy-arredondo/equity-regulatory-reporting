import type { ReactNode } from "react";
import { ExternalLink } from "lucide-react";
import { Link } from "react-router-dom";
import { buttonVariants } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { cn } from "@/lib/utils";
import { Skeleton } from "@/components/ui/skeleton";
import { Sheet, SheetContent, SheetHeader, SheetTitle } from "@/components/ui/sheet";
import { useMediaQuery } from "@/hooks/useMediaQuery";

interface SummaryPanelProps {
  icon?: ReactNode;
  title: string;
  isLoading: boolean;
  hasData: boolean;
  detailPath?: string;
  children?: ReactNode;
  open?: boolean;
  onOpenChange?: (open: boolean) => void;
}

function PanelContent({
  icon,
  title,
  isLoading,
  hasData,
  detailPath,
  children,
}: Omit<SummaryPanelProps, "open" | "onOpenChange">) {
  return (
    <Card className="h-full">
      <CardHeader className="pb-3">
        <CardTitle className="flex items-center gap-2 text-sm font-medium">
          {icon}
          {title}
        </CardTitle>
      </CardHeader>
      <CardContent className="space-y-3">
        {isLoading ? (
          <div className="space-y-2">
            <Skeleton className="h-4 w-3/4" />
            <Skeleton className="h-4 w-1/2" />
            <Skeleton className="h-4 w-2/3" />
          </div>
        ) : hasData ? (
          <>
            {children}
            {detailPath && (
              <div className="border-t pt-3">
                <Link
                  to={detailPath}
                  className={cn(
                    buttonVariants({ variant: "outline", size: "sm" }),
                    "w-full justify-center",
                  )}
                >
                  <ExternalLink className="mr-2 h-3.5 w-3.5" />
                  Ver detalle completo
                </Link>
              </div>
            )}
          </>
        ) : (
          <p className="text-sm text-muted-foreground">Sin información.</p>
        )}
      </CardContent>
    </Card>
  );
}

export function SummaryPanel(props: SummaryPanelProps) {
  const isDesktop = useMediaQuery("(min-width: 1024px)");

  if (isDesktop) {
    return <PanelContent {...props} />;
  }

  return (
    <Sheet open={props.open} onOpenChange={props.onOpenChange}>
      <SheetContent side="right" className="w-80 overflow-y-auto">
        <SheetHeader>
          <SheetTitle className="flex items-center gap-2 text-sm">
            {props.icon}
            {props.title}
          </SheetTitle>
        </SheetHeader>
        <div className="mt-4">
          <PanelContent {...props} />
        </div>
      </SheetContent>
    </Sheet>
  );
}

export function SummaryRow({
  label,
  value,
}: {
  label: string;
  value: ReactNode;
}) {
  return (
    <div className="space-y-0.5">
      <p className="text-xs text-muted-foreground">{label}</p>
      <p className="text-sm font-medium">{value}</p>
    </div>
  );
}

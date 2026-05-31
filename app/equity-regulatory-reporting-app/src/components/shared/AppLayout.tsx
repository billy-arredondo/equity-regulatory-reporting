import { Menu, PanelLeftClose, PanelLeftOpen } from "lucide-react";
import { Outlet } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { Sheet, SheetContent } from "@/components/ui/sheet";
import { useMediaQuery } from "@/hooks/useMediaQuery";
import { useUiStore } from "@/stores/ui.store";
import { ThemeToggle } from "./ThemeToggle";
import { Sidebar } from "./Sidebar";
import { useState } from "react";

export function AppLayout() {
  const isDesktop = useMediaQuery("(min-width: 768px)");
  const { sidebarCollapsed, toggleSidebar } = useUiStore();
  const [mobileOpen, setMobileOpen] = useState(false);

  return (
    <div className="flex h-screen overflow-hidden bg-background">
      {isDesktop ? (
        <>
          <Sidebar />
          <div className="flex flex-1 flex-col overflow-hidden">
            <header className="flex h-14 items-center border-b px-4">
              <Button
                variant="ghost"
                size="icon"
                onClick={toggleSidebar}
                aria-label={sidebarCollapsed ? "Expandir sidebar" : "Colapsar sidebar"}
              >
                {sidebarCollapsed ? (
                  <PanelLeftOpen className="h-4 w-4" />
                ) : (
                  <PanelLeftClose className="h-4 w-4" />
                )}
              </Button>
            </header>
            <main className="flex-1 overflow-y-auto p-6">
              <Outlet />
            </main>
          </div>
        </>
      ) : (
        <div className="flex flex-1 flex-col overflow-hidden">
          <header className="flex h-14 items-center justify-between border-b px-4">
            <Button
              variant="ghost"
              size="icon"
              onClick={() => setMobileOpen(true)}
              aria-label="Abrir menú"
            >
              <Menu className="h-4 w-4" />
            </Button>
            <span className="text-sm font-semibold">Equity Reporting</span>
            <ThemeToggle />
          </header>

          <Sheet open={mobileOpen} onOpenChange={setMobileOpen}>
            <SheetContent side="left" className="w-60 p-0">
              <Sidebar onNavigate={() => setMobileOpen(false)} />
            </SheetContent>
          </Sheet>

          <main className="flex-1 overflow-y-auto p-4">
            <Outlet />
          </main>
        </div>
      )}
    </div>
  );
}

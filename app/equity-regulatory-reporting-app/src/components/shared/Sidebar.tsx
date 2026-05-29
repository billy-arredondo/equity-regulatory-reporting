import { useState } from "react";
import {
  BookOpen,
  Building2,
  ChevronDown,
  FileText,
  FolderOpen,
  Globe,
  LayoutDashboard,
  LogOut,
  TrendingUp,
  Users,
} from "lucide-react";
import { NavLink, useNavigate } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";
import { Permission } from "@/lib/permissions";
import { useAuthStore } from "@/stores/auth.store";
import { useUiStore } from "@/stores/ui.store";
import { useLogoutMutation } from "@/hooks/useAuth";
import { PermissionGuard } from "./PermissionGuard";
import { ThemeToggle } from "./ThemeToggle";

interface NavItem {
  to: string;
  icon: React.ReactNode;
  label: string;
  perm: number;
}

const topNavItems: NavItem[] = [
  { to: "/persons",       icon: <Users className="h-4 w-4" />,         label: "Personas",          perm: Permission.PersonRead },
  { to: "/person-types",  icon: <LayoutDashboard className="h-4 w-4" />, label: "Tipos de persona", perm: Permission.PersonRead },
  { to: "/participations",icon: <TrendingUp className="h-4 w-4" />,    label: "Participaciones",   perm: Permission.ParticipationRead },
  { to: "/board",         icon: <Building2 className="h-4 w-4" />,     label: "Junta directiva",   perm: Permission.BoardRead },
];

const generalItems: NavItem[] = [
  { to: "/document-types", icon: <FileText className="h-4 w-4" />, label: "Tipos de documento", perm: Permission.DocumentTypeRead },
  { to: "/countries",      icon: <Globe className="h-4 w-4" />,    label: "Países",              perm: Permission.CountryRead },
  { to: "/positions",      icon: <BookOpen className="h-4 w-4" />, label: "Cargos",              perm: Permission.PositionRead },
];

const bottomNavItems: NavItem[] = [
  { to: "/users", icon: <Users className="h-4 w-4" />, label: "Usuarios", perm: Permission.UserRead },
];

const navLinkClass = (isActive: boolean, collapsed: boolean) =>
  cn(
    "flex items-center gap-3 rounded-md px-2 py-2 text-sm transition-colors",
    "hover:bg-sidebar-accent hover:text-sidebar-accent-foreground",
    isActive
      ? "bg-sidebar-accent text-sidebar-accent-foreground font-medium"
      : "text-sidebar-foreground",
    collapsed && "justify-center px-2",
  );

interface SidebarProps {
  onNavigate?: () => void;
}

export function Sidebar({ onNavigate }: SidebarProps) {
  const collapsed = useUiStore((s) => s.sidebarCollapsed);
  const user = useAuthStore((s) => s.user);
  const { mutate: logout } = useLogoutMutation();
  const navigate = useNavigate();
  const [generalOpen, setGeneralOpen] = useState(false);

  function renderNavItem(item: NavItem) {
    return (
      <PermissionGuard key={item.to} perm={item.perm}>
        <NavLink
          to={item.to}
          onClick={onNavigate}
          className={({ isActive }) => navLinkClass(isActive, collapsed)}
          title={collapsed ? item.label : undefined}
        >
          {item.icon}
          {!collapsed && <span className="truncate">{item.label}</span>}
        </NavLink>
      </PermissionGuard>
    );
  }

  return (
    <div
      className={cn(
        "flex h-full flex-col border-r bg-sidebar text-sidebar-foreground transition-all duration-200",
        collapsed ? "w-16" : "w-60",
      )}
    >
      <div className="flex h-14 items-center border-b px-3">
        {!collapsed && (
          <span className="truncate text-sm font-semibold">Equity Reporting</span>
        )}
      </div>

      <nav className="flex-1 space-y-1 overflow-y-auto p-2">
        {topNavItems.map(renderNavItem)}

        {/* General group */}
        {!collapsed && (
          <button
            onClick={() => setGeneralOpen((o) => !o)}
            className={cn(
              "flex w-full items-center gap-3 rounded-md px-2 py-2 text-sm transition-colors",
              "hover:bg-sidebar-accent hover:text-sidebar-accent-foreground text-sidebar-foreground",
            )}
          >
            <FolderOpen className="h-4 w-4 shrink-0" />
            <span className="flex-1 truncate text-left">General</span>
            <ChevronDown
              className={cn("h-3.5 w-3.5 shrink-0 transition-transform duration-200", generalOpen && "rotate-180")}
            />
          </button>
        )}
        {(collapsed || generalOpen) && (
          <div className={cn("space-y-1", !collapsed && "pl-3")}>
            {generalItems.map(renderNavItem)}
          </div>
        )}

        {bottomNavItems.map(renderNavItem)}
      </nav>

      <div className="border-t p-2 space-y-1">
        <div className={cn("flex items-center gap-2 px-2 py-1", collapsed && "justify-center")}>
          <ThemeToggle />
          {!collapsed && (
            <span className="truncate text-xs text-muted-foreground">{user?.email}</span>
          )}
        </div>
        <Button
          variant="ghost"
          size={collapsed ? "icon" : "sm"}
          className={cn("w-full", collapsed ? "px-2" : "justify-start gap-2")}
          onClick={() => {
            logout();
            void navigate("/login");
          }}
          title={collapsed ? "Cerrar sesión" : undefined}
        >
          <LogOut className="h-4 w-4" />
          {!collapsed && "Cerrar sesión"}
        </Button>
      </div>
    </div>
  );
}

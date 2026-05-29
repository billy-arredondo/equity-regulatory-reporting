import {
  BookOpen,
  Building2,
  FileText,
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

const navItems: NavItem[] = [
  { to: "/persons", icon: <Users className="h-4 w-4" />, label: "Personas", perm: Permission.PersonRead },
  { to: "/person-types", icon: <LayoutDashboard className="h-4 w-4" />, label: "Tipos de persona", perm: Permission.PersonRead },
  { to: "/document-types", icon: <FileText className="h-4 w-4" />, label: "Tipos de documento", perm: Permission.DocumentTypeRead },
  { to: "/countries", icon: <Globe className="h-4 w-4" />, label: "Países", perm: Permission.CountryRead },
  { to: "/participations", icon: <TrendingUp className="h-4 w-4" />, label: "Participaciones", perm: Permission.ParticipationRead },
  { to: "/board", icon: <Building2 className="h-4 w-4" />, label: "Junta directiva", perm: Permission.BoardRead },
  { to: "/positions", icon: <BookOpen className="h-4 w-4" />, label: "Cargos", perm: Permission.PositionRead },
  { to: "/users", icon: <Users className="h-4 w-4" />, label: "Usuarios", perm: Permission.UserRead },
];

interface SidebarProps {
  onNavigate?: () => void;
}

export function Sidebar({ onNavigate }: SidebarProps) {
  const collapsed = useUiStore((s) => s.sidebarCollapsed);
  const user = useAuthStore((s) => s.user);
  const { mutate: logout } = useLogoutMutation();
  const navigate = useNavigate();

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
        {navItems.map((item) => (
          <PermissionGuard key={item.to} perm={item.perm}>
            <NavLink
              to={item.to}
              onClick={onNavigate}
              className={({ isActive }) =>
                cn(
                  "flex items-center gap-3 rounded-md px-2 py-2 text-sm transition-colors",
                  "hover:bg-sidebar-accent hover:text-sidebar-accent-foreground",
                  isActive
                    ? "bg-sidebar-accent text-sidebar-accent-foreground font-medium"
                    : "text-sidebar-foreground",
                  collapsed && "justify-center px-2",
                )
              }
              title={collapsed ? item.label : undefined}
            >
              {item.icon}
              {!collapsed && <span className="truncate">{item.label}</span>}
            </NavLink>
          </PermissionGuard>
        ))}
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

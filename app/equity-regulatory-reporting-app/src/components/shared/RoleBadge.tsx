import { Badge } from "@/components/ui/badge";

interface Props {
  role: string;
}

export function RoleBadge({ role }: Props) {
  return (
    <Badge variant={role === "Admin" ? "default" : "secondary"} className="font-mono text-xs">
      {role}
    </Badge>
  );
}

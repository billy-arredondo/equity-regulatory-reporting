import { useState } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from "@/components/ui/dialog";
import { Label } from "@/components/ui/label";
import { Button } from "@/components/ui/button";
import { ActionButton } from "@/elements/ActionButton";
import { SelectField } from "@/elements/SelectField";
import { useRolesQuery, useAssignRoleMutation } from "@/hooks/useUsers";

interface Props {
  userId: string;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function AssignRoleDialog({ userId, open, onOpenChange }: Props) {
  const [selectedRole, setSelectedRole] = useState<string>("");
  const { data: roles } = useRolesQuery();
  const { mutate: assign, isPending } = useAssignRoleMutation();

  const roleOptions = (roles ?? []).map((r) => ({ value: r, label: r }));

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!selectedRole) return;
    assign(
      { id: userId, dto: { userId, role: selectedRole } },
      {
        onSuccess: () => {
          setSelectedRole("");
          onOpenChange(false);
        },
      },
    );
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Asignar rol</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
            <Label>Rol</Label>
            <SelectField
              value={selectedRole || null}
              onChange={(v) => setSelectedRole(v)}
              options={roleOptions}
              placeholder="Seleccionar rol..."
            />
          </div>
          <DialogFooter>
            <Button
              type="button"
              variant="outline"
              onClick={() => onOpenChange(false)}
            >
              Cancelar
            </Button>
            <ActionButton type="submit" isLoading={isPending} disabled={!selectedRole || isPending}>
              Asignar
            </ActionButton>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}

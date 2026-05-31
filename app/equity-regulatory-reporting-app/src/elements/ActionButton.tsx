import type { ComponentPropsWithoutRef } from "react";
import { Button } from "@/components/ui/button";

interface Props extends ComponentPropsWithoutRef<typeof Button> {
  isLoading?: boolean;
}

export function ActionButton({ isLoading, children, disabled, ...props }: Props) {
  return (
    <Button {...props} disabled={disabled ?? isLoading}>
      {isLoading ? "..." : children}
    </Button>
  );
}

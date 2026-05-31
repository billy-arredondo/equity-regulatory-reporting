import type { ReactNode } from "react";

interface Props {
  title: string;
  actions?: ReactNode;
}

export function PageHeader({ title, actions }: Props) {
  return (
    <div className="mb-6 flex items-center justify-between">
      <h1 className="text-2xl font-semibold tracking-tight">{title}</h1>
      {actions && <div className="flex items-center gap-2">{actions}</div>}
    </div>
  );
}

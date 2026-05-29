interface Props {
  children: React.ReactNode;
}

export function FieldTip({ children }: Props) {
  return (
    <p className="text-xs text-muted-foreground">{children}</p>
  );
}

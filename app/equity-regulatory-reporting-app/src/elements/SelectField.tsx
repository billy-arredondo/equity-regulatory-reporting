import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

interface Option<T extends string | number> {
  value: T;
  label: string;
}

interface Props<T extends string | number> {
  value: T | null | undefined;
  onChange: (value: T) => void;
  options: Option<T>[];
  placeholder?: string;
  disabled?: boolean;
}

export function SelectField<T extends string | number>({
  value,
  onChange,
  options,
  placeholder = "Seleccionar...",
  disabled,
}: Props<T>) {
  const safeValue = value != null && value !== "" ? String(value) : null;
  const selectItems = options.map((o) => ({ value: String(o.value), label: o.label }));

  return (
    <Select
      value={safeValue}
      items={selectItems}
      onValueChange={(v) => {
        if (v == null) return;
        const matched = options.find((o) => String(o.value) === v);
        onChange((matched?.value ?? v) as T);
      }}
      disabled={disabled}
    >
      <SelectTrigger className="w-full">
        <SelectValue placeholder={placeholder} />
      </SelectTrigger>
      <SelectContent>
        {options.map((opt) => (
          <SelectItem key={String(opt.value)} value={String(opt.value)}>
            {opt.label}
          </SelectItem>
        ))}
      </SelectContent>
    </Select>
  );
}

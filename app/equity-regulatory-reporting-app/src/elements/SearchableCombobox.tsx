import { useEffect, useRef, useState } from "react";
import { Check, ChevronsUpDown, X } from "lucide-react";
import { buttonVariants } from "@/components/ui/button";
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
} from "@/components/ui/command";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { cn } from "@/lib/utils";

export interface ComboboxOption {
  id: string;
  label: string;
  sublabel?: string;
}

interface Props {
  value: string | null;
  onChange: (id: string | null) => void;
  options: ComboboxOption[];
  isLoading?: boolean;
  onSearchChange: (search: string) => void;
  placeholder?: string;
  disabled?: boolean;
  clearable?: boolean;
}

export function SearchableCombobox({
  value,
  onChange,
  options,
  isLoading,
  onSearchChange,
  placeholder = "Buscar...",
  disabled,
  clearable = true,
}: Props) {
  const [open, setOpen] = useState(false);
  const [search, setSearch] = useState("");
  const debounceRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  useEffect(() => {
    if (debounceRef.current) clearTimeout(debounceRef.current);
    debounceRef.current = setTimeout(() => onSearchChange(search), 300);
    return () => {
      if (debounceRef.current) clearTimeout(debounceRef.current);
    };
  }, [search, onSearchChange]);

  const selected = options.find((o) => o.id === value);

  return (
    <Popover open={open} onOpenChange={setOpen}>
      <PopoverTrigger
        role="combobox"
        aria-expanded={open}
        disabled={disabled}
        className={cn(
          buttonVariants({ variant: "outline" }),
          "w-full justify-between font-normal",
        )}
      >
        <span className="truncate">
          {selected ? selected.label : <span className="text-muted-foreground">{placeholder}</span>}
        </span>
        <div className="ml-2 flex shrink-0 items-center gap-1">
          {clearable && value && (
            <span
              role="button"
              tabIndex={0}
              aria-label="Limpiar selección"
              className="text-muted-foreground hover:text-foreground"
              onClick={(e) => {
                e.stopPropagation();
                onChange(null);
              }}
              onKeyDown={(e) => {
                if (e.key === "Enter" || e.key === " ") {
                  e.preventDefault();
                  e.stopPropagation();
                  onChange(null);
                }
              }}
            >
              <X className="h-3.5 w-3.5" />
            </span>
          )}
          <ChevronsUpDown className="h-3.5 w-3.5 text-muted-foreground" />
        </div>
      </PopoverTrigger>
      <PopoverContent className="w-[var(--radix-popover-trigger-width)] p-0" align="start">
        <Command shouldFilter={false}>
          <CommandInput
            placeholder={placeholder}
            value={search}
            onValueChange={setSearch}
          />
          <CommandList>
            {isLoading ? (
              <CommandEmpty>Buscando...</CommandEmpty>
            ) : options.length === 0 ? (
              <CommandEmpty>Sin resultados.</CommandEmpty>
            ) : (
              <CommandGroup>
                {options.map((opt) => (
                  <CommandItem
                    key={opt.id}
                    value={opt.id}
                    onSelect={() => {
                      onChange(opt.id);
                      setOpen(false);
                      setSearch("");
                    }}
                  >
                    <Check
                      className={cn(
                        "mr-2 h-4 w-4",
                        value === opt.id ? "opacity-100" : "opacity-0",
                      )}
                    />
                    <div>
                      <p className="text-sm">{opt.label}</p>
                      {opt.sublabel && (
                        <p className="text-xs text-muted-foreground">{opt.sublabel}</p>
                      )}
                    </div>
                  </CommandItem>
                ))}
              </CommandGroup>
            )}
          </CommandList>
        </Command>
      </PopoverContent>
    </Popover>
  );
}

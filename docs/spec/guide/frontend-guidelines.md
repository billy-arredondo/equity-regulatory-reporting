# Frontend Guidelines — React / TypeScript / Vite

Guía de referencia para iniciar y mantener el frontend de una aplicación nueva siguiendo los patrones establecidos en este proyecto.

---

## Stack tecnológico

| Responsabilidad | Tecnología            |
| --------------- | --------------------- |
| Framework UI    | React 19              |
| Lenguaje        | TypeScript 5 (strict) |
| Bundler         | Vite                  |
| Estilos         | Tailwind CSS v4       |
| Componentes     | shadcn/ui (Radix UI)  |
| Server state    | TanStack Query v5     |
| Client state    | Zustand               |
| Routing         | React Router v7       |
| Iconos          | Lucide React          |
| HTTP            | Fetch nativo / axios  |

---

## Estructura de `app/src/`

```
src/
├── api/              # Funciones de servicio — una por entidad
│   └── buildings.api.ts
├── components/
│   ├── shared/       # Componentes reutilizables de negocio
│   │   ├── SummaryPanel.tsx
│   │   └── BuildingSummaryPanel.tsx
│   └── ui/           # Primitivas generadas por shadcn (no editar manualmente)
├── elements/         # Primitivas propias: ActionButton, FieldTip, SelectField
├── hooks/            # Custom React Query hooks — uno por entidad
│   └── useBuildings.ts
├── pages/            # Páginas por entidad
│   └── buildings/
│       ├── BuildingListPage.tsx
│       ├── BuildingDetailPage.tsx
│       └── BuildingFormPage.tsx
├── stores/           # Zustand stores
│   ├── auth.store.ts
│   └── ui.store.ts
├── types/            # Interfaces TypeScript que reflejan los VMs del backend
│   └── building.ts
├── lib/              # Utilidades puras (sin dependencias de React)
│   └── format.ts
└── router/
    └── index.tsx     # Definición de rutas
```

---

## Types — interfaces que reflejan VMs del backend

```typescript
// types/building.ts
export interface BuildingListVm {
  buildingId: string;
  name: string;
  address: string | null;
  floorCount: number;
}

export interface BuildingDetailVm extends BuildingListVm {
  floors: FloorListVm[];
  createdAt: string;
}

export interface CreateBuildingDto {
  name: string;
  address: string | null;
}

export interface UpdateBuildingDto {
  name: string;
  address: string | null;
}
```

- Usar `string` para GUIDs (JSON los serializa como strings).
- Usar `string` para fechas recibidas del backend; transformar a `Date` solo si se necesita.
- Nunca usar `any`. Si el tipo es genuinamente desconocido, usar `unknown`.

---

## API — funciones de servicio

```typescript
// api/buildings.api.ts
import type {
  BuildingDetailVm,
  BuildingListVm,
  CreateBuildingDto,
  UpdateBuildingDto,
} from "@/types/building";

const BASE = "/api/buildings";

export async function getBuildings(): Promise<BuildingListVm[]> {
  const res = await fetch(BASE);
  if (!res.ok) throw new Error("Error al cargar edificios.");
  return res.json();
}

export async function getBuildingById(id: string): Promise<BuildingDetailVm> {
  const res = await fetch(`${BASE}/${id}`);
  if (!res.ok) throw new Error("Edificio no encontrado.");
  return res.json();
}

export async function createBuilding(
  dto: CreateBuildingDto,
): Promise<BuildingDetailVm> {
  const res = await fetch(BASE, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(dto),
  });
  if (!res.ok) throw new Error("Error al crear el edificio.");
  return res.json();
}

export async function updateBuilding(
  id: string,
  dto: UpdateBuildingDto,
): Promise<void> {
  const res = await fetch(`${BASE}/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(dto),
  });
  if (!res.ok) throw new Error("Error al actualizar el edificio.");
}

export async function deleteBuilding(id: string): Promise<void> {
  const res = await fetch(`${BASE}/${id}`, { method: "DELETE" });
  if (!res.ok) throw new Error("Error al eliminar el edificio.");
}
```

---

## Hooks — TanStack Query

### Query Keys tipadas

```typescript
// hooks/useBuildings.ts
export const buildingKeys = {
  all: ["buildings"] as const,
  detail: (id: string) => ["buildings", id] as const,
};
```

### Hooks de consulta

```typescript
export function useBuildingsQuery() {
  return useQuery({
    queryKey: buildingKeys.all,
    queryFn: getBuildings,
  });
}

export function useBuildingDetailQuery(id: string) {
  return useQuery({
    queryKey: buildingKeys.detail(id),
    queryFn: () => getBuildingById(id),
    enabled: !!id,
  });
}
```

### Mutations con invalidación de caché

```typescript
export function useCreateBuildingMutation() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: createBuilding,
    onSuccess: () => {
      void qc.invalidateQueries({ queryKey: buildingKeys.all });
      toast.success("Edificio creado");
    },
    onError: () => toast.error("Error al crear el edificio"),
  });
}

export function useDeleteBuildingMutation() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: deleteBuilding,
    onSuccess: (_data, id) => {
      // removeQueries primero: evita refetch de detail que devolvería 404
      qc.removeQueries({ queryKey: buildingKeys.detail(id) });
      void qc.invalidateQueries({ queryKey: buildingKeys.all });
      toast.success("Edificio eliminado");
    },
    onError: () => toast.error("Error al eliminar el edificio"),
  });
}
```

> **Regla importante al eliminar/promover:** llamar `qc.removeQueries` para la clave de detalle **antes** de `qc.invalidateQueries` para la lista. `invalidateQueries` con prefijo marca también las detail queries como stale y provoca un refetch inmediato que recibiría 404.

---

## Rutas — patrón por entidad

| Path                    | Componente           | Guard   |
| ----------------------- | -------------------- | ------- |
| `/edificios`            | `BuildingListPage`   | —       |
| `/edificios/nuevo`      | `BuildingFormPage`   | Manager |
| `/edificios/:id`        | `BuildingDetailPage` | —       |
| `/edificios/:id/editar` | `BuildingFormPage`   | Manager |

Create y edit comparten un único `*FormPage`. La presencia de `:id` determina el modo.

```tsx
// router/index.tsx
<Route path="/edificios">
  <Route index element={<BuildingListPage />} />
  <Route
    path="nuevo"
    element={
      <RoleGuard role="Manager">
        <BuildingFormPage />
      </RoleGuard>
    }
  />
  <Route path=":id" element={<BuildingDetailPage />} />
  <Route
    path=":id/editar"
    element={
      <RoleGuard role="Manager">
        <BuildingFormPage />
      </RoleGuard>
    }
  />
</Route>
```

---

## Páginas de lista

### Layout con panel de detalle lateral

```tsx
export function BuildingListPage() {
  const { data: buildings = [], isLoading } = useBuildingsQuery();
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const [search, setSearch] = useState("");

  const filtered = buildings.filter((b) =>
    b.name.toLowerCase().includes(search.toLowerCase()),
  );

  return (
    <div>
      <PageHeader
        title="Edificios"
        actions={<CreateButton to="/edificios/nuevo" />}
      />

      <div className="grid grid-cols-1 gap-6 lg:grid-cols-[1fr_288px] lg:items-start">
        <DataTable
          data={filtered}
          columns={buildingColumns}
          isLoading={isLoading}
          onRowClick={(row) => setSelectedId(row.buildingId)}
          isRowSelected={(row) => row.buildingId === selectedId}
        />

        <div className="lg:sticky lg:top-4">
          {selectedId ? (
            <BuildingSummaryPanel buildingId={selectedId} />
          ) : (
            <div className="flex items-center justify-center rounded-md border border-dashed px-4 py-12 text-center text-sm text-muted-foreground">
              Selecciona un elemento para ver su ficha
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
```

- `DataTable` gestiona internamente el estilo de hover y fila activa. Nunca aplicar clases de selección manualmente con `getRowClassName`.
- Hover: `bg-accent/50`. Fila activa: `bg-accent text-accent-foreground border-l-2 border-l-primary`.

---

## Summary Panel

```tsx
// components/shared/BuildingSummaryPanel.tsx
export function BuildingSummaryPanel({ buildingId }: { buildingId: string }) {
  const { data, isLoading } = useBuildingDetailQuery(buildingId);

  return (
    <SummaryPanel
      icon={<BuildingIcon className="h-4 w-4 text-muted-foreground" />}
      title="Ficha del edificio"
      isLoading={isLoading}
      hasData={!!data}
      detailPath={`/edificios/${buildingId}`}
    >
      {data && (
        <>
          <SummaryRow label="Nombre" value={data.name} />
          <SummaryRow label="Dirección" value={data.address ?? "—"} />

          <div className="space-y-3 border-t pt-3">
            <SummaryRow label="Plantas" value={String(data.floorCount)} />
          </div>
        </>
      )}
    </SummaryPanel>
  );
}
```

`SummaryPanel` renderiza el Card shell, el estado de carga y el botón "Ver detalle completo". No duplicar ese boilerplate.

---

## Form Pages

### Forma corta (sin sidebar)

```tsx
export function BuildingFormPage() {
  const { id } = useParams<{ id: string }>();
  const isEdit = !!id;
  const navigate = useNavigate();
  const [initialized, setInitialized] = useState(false);
  const [form, setForm] = useState<CreateBuildingDto>({
    name: "",
    address: null,
  });

  const { data } = useBuildingDetailQuery(id ?? "");
  const { mutate: create, isPending: isCreating } = useCreateBuildingMutation();
  const { mutate: update, isPending: isUpdating } = useUpdateBuildingMutation();
  const isPending = isCreating || isUpdating;

  useEffect(() => {
    if (!isEdit || initialized || !data) return;
    setForm({ name: data.name, address: data.address });
    setInitialized(true);
  }, [isEdit, data, initialized]);

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (isEdit) {
      update(
        { id: id!, dto: form },
        { onSuccess: () => navigate(`/edificios/${id}`) },
      );
    } else {
      create(form, { onSuccess: () => navigate("/edificios") });
    }
  }

  return (
    <div>
      <Link
        to={isEdit ? `/edificios/${id}` : "/edificios"}
        className="mb-5 inline-flex items-center gap-1 text-sm text-muted-foreground hover:text-foreground"
      >
        ← {isEdit ? "Volver al detalle" : "Volver a edificios"}
      </Link>

      <PageHeader title={isEdit ? "Editar edificio" : "Nuevo edificio"} />

      <form onSubmit={handleSubmit} className="mt-4 max-w-lg space-y-4">
        <div className="space-y-2">
          <Label htmlFor="name">Nombre</Label>
          <Input
            id="name"
            value={form.name}
            onChange={(e) => setForm((f) => ({ ...f, name: e.target.value }))}
            required
          />
        </div>

        <div className="mt-6 flex gap-2">
          <Button type="submit" disabled={isPending}>
            {isPending ? "..." : isEdit ? "Guardar cambios" : "Crear edificio"}
          </Button>
          <Button type="button" variant="ghost" onClick={() => navigate(-1)}>
            Cancelar
          </Button>
        </div>
      </form>
    </div>
  );
}
```

**Reglas de formularios:**

- Siempre incluir back link (`← Volver a ...`) sobre el `PageHeader`.
- `max-w-lg` para limitar el ancho.
- Orden de botones: primario → cancelar (`variant="ghost"`).
- Crear → navegar a lista. Editar → navegar a detalle.

---

## Detail Pages

El detalle debe usar **el mismo layout de grid** que el formulario para que cambiar entre ver y editar se sienta como un cambio de modo.

```tsx
// Campo de detalle — altura h-9 igual que un Input de shadcn
<div className="space-y-1.5">
  <p className="text-xs text-muted-foreground">Nombre</p>
  <p className="flex h-9 items-center text-sm font-medium">{building.name}</p>
</div>
```

- Mismo `max-w-lg` y `grid grid-cols-2 gap-3` que el formulario.
- `h-9` iguala la altura de un `Input` / `SelectField`.
- El orden y los column spans deben ser idénticos al formulario.
- Contexto adicional de solo lectura va debajo en un `Card` con `mt-6`.

---

## Zustand — stores

```typescript
// stores/ui.store.ts
interface UiState {
  sidebarOpen: boolean;
  toggleSidebar: () => void;
}

export const useUiStore = create<UiState>((set) => ({
  sidebarOpen: true,
  toggleSidebar: () => set((s) => ({ sidebarOpen: !s.sidebarOpen })),
}));
```

- Un store por dominio de estado (auth, ui). No mezclar estado de negocio con estado de UI global.
- State de server (datos del backend) siempre en TanStack Query, nunca en Zustand.

---

## Radix UI / shadcn — pitfalls frecuentes

### No anidar botones

`Checkbox`, `Switch` y similares renderizan como `<button>`. Nunca colocarlos dentro de un `<button>`:

```tsx
// ✗ HTML inválido
<button onClick={...}>
  <Checkbox checked={isSelected} />
</button>

// ✓ Correcto
<div
  role="option"
  aria-selected={isSelected}
  tabIndex={0}
  onClick={() => onToggle(id)}
  onKeyDown={(e) => {
    if (e.key === 'Enter' || e.key === ' ') { e.preventDefault(); onToggle(id) }
  }}
>
  <Checkbox checked={isSelected} className="pointer-events-none" tabIndex={-1} />
</div>
```

### Select.Item no acepta `value=""`

Radix lanza un error en runtime si un `<SelectItem>` recibe `value=""`. Convertir `''` a `undefined` para mostrar el placeholder:

```tsx
// ✗
<SelectField value={form.status} options={[{ value: '', label: 'Sin definir' }]} />

// ✓
<SelectField
  value={form.status || undefined}
  placeholder="Sin definir"
  options={STATUS_OPTIONS}
/>
```

### TableBody border — `border-b-0` no `border-0`

El override de shadcn `TableBody` usa `[&_tr:last-child]:border-b-0` (no `border-0`) para que `border-l-*` de filas activas no se elimine en la última fila.

---

## UX / UI — reglas generales

- Todo el texto visible para el usuario en **español**.
- Toasts: éxito → acción realizada ("Edificio creado"), error → acción fallida ("Error al crear el edificio").
- Acciones destructivas siempre requieren `ConfirmDialog` antes de ejecutarse.
- Write operations (crear, actualizar, eliminar) protegidas con `RoleGuard role="Manager"`.
- `LoadingSpinner` mientras cargan datos primarios; `...` inline para estados de mutation pending.

---

## Convenciones TypeScript — resumen rápido

- Tipos primitivos en minúscula: `string`, `number`, `boolean` — nunca `String`, `Number`.
- Nunca `any`. Usar `unknown` cuando el tipo es genuinamente desconocido.
- `void` como return type de callbacks cuyo valor de retorno se ignora.
- Path alias `@/` para todos los imports internos.
- Imports agrupados: librerías externas primero, luego módulos internos.
- Hooks solo al nivel superior de un componente o custom hook — nunca dentro de condiciones o bucles.
- Custom hooks con prefijo `use`.

---

## Checklist para una entidad nueva

```
□ types/{entity}.ts                        — interfaces ListVm, DetailVm, CreateDto, UpdateDto
□ api/{entity}.api.ts                      — getAll, getById, create, update, delete
□ hooks/use{Entity}s.ts                    — query keys + useQuery + useMutation hooks
□ pages/{entity}/
│   ├── {Entity}ListPage.tsx               — tabla + panel lateral
│   ├── {Entity}DetailPage.tsx             — detalle alineado con el form
│   └── {Entity}FormPage.tsx               — create/edit unificado
├── components/shared/{Entity}SummaryPanel.tsx
□ router/index.tsx                         — agregar rutas con RoleGuard donde corresponda
```

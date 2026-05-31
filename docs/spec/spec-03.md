# Spec 03 - Move Participations and Board Members into Legal Person Detail

## 1. Context

Currently, `Participaciones` and `Junta directiva` are exposed as first-level sidebar entries and as global modules with their own standalone routes:

- `/participations/*`
- `/board/*`

However, both concepts are relationship data. They only make sense in the context of a specific legal person or legal entity:

- `Participaciones` describe the ownership/capital structure of a company or legal entity.
- `Junta directiva` describes the governance structure of a company or legal entity.

The current UI forces the user to leave the company context, navigate to a global module, filter by company, and then perform maintenance operations. This introduces unnecessary friction for the main use case of the application.

The application is not intended for cross-company audit analysis or stock analysis. Its goal is to help prepare regulatory reporting for a specific company/entity. Therefore, participation and board member maintenance must be performed inside the company/entity detail view.

## 2. Objective

Remove the global `Participaciones` and `Junta directiva` modules and move their full management experience into the detail view of legal persons and legal entities using nested tabs/routes.

The target route structure is:

```txt
/companies/:id                     -> General
/companies/:id/participations      -> Participaciones
/companies/:id/board               -> Junta directiva

/entities/:id                      -> General
/entities/:id/participations       -> Participaciones
/entities/:id/board                -> Junta directiva
```

Natural persons must remain unchanged:

```txt
/people/:id
```

Natural persons must not display participation or board tabs in this iteration.

## 3. Confirmed Decisions

The following decisions are confirmed:

- Use nested sub-routes instead of query parameters.
- Remove global routes completely.
- Remove `Participaciones` and `Junta directiva` from the sidebar.
- Apply tabs only to:
  - Personas Jurídicas: `/companies/:id`
  - Entes Jurídicos: `/entities/:id`
- Do not apply tabs to:
  - Personas Naturales: `/people/:id`
- The global audit/cross-company view is intentionally removed.
- If a global report is needed in the future, it must be implemented under a reporting module, not as a CRUD sidebar module.

## 4. Scope

### In scope

- Sidebar cleanup.
- Global route removal.
- New nested route structure for companies and entities.
- New shared detail layout for legal persons/legal entities.
- Tabs for:
  - General
  - Participaciones
  - Junta directiva
- Scoped participation CRUD inside company/entity detail.
- Scoped board member CRUD inside company/entity detail.
- Reuse existing queries, mutations, components, and hooks where possible.
- Update navigation paths and back links.
- Preserve current backend API behavior.

### Out of scope

- Backend changes.
- New reporting module.
- Cross-company audit views.
- Participation calculations.
- Ownership tree visualization.
- Natural person participation tabs.
- Regulatory report generation logic.

## 5. Backend Impact

No backend changes are required.

The existing endpoints already support scoping by company:

```txt
GET /api/Participations?companyId={companyId}
GET /api/BoardMembers?companyId={companyId}
```

The existing frontend hooks already support passing `companyId`:

```ts
useParticipationsQuery;
useBoardMembersQuery;
```

Mutations for create/update/delete should remain unchanged unless route navigation requires a new `basePath` or `lockedCompanyId` prop in the frontend.

## 6. New Routing Model

### 6.1 Companies routes

The `companies/index.tsx` route definition should support nested detail routes.

Recommended shape:

```tsx
<Routes>
  <Route index element={<LegalPersonListPage />} />
  <Route path="new" element={<LegalPersonFormPage />} />
  <Route path=":id/edit" element={<LegalPersonFormPage />} />
  <Route
    path=":id/*"
    element={<LegalPersonDetailLayout baseRoute="/companies" />}
  />
</Routes>
```

### 6.2 Entities routes

The `entities/index.tsx` route definition should mirror the company route structure.

Recommended shape:

```tsx
<Routes>
  <Route index element={<LegalPersonListPage />} />
  <Route path="new" element={<LegalPersonFormPage />} />
  <Route path=":id/edit" element={<LegalPersonFormPage />} />
  <Route
    path=":id/*"
    element={<LegalPersonDetailLayout baseRoute="/entities" />}
  />
</Routes>
```

### 6.3 Shared nested detail layout

Create a shared layout:

```txt
src/pages/persons/LegalPersonDetailLayout.tsx
```

This layout must be reused by both:

```txt
/companies/:id/*
/entities/:id/*
```

The layout is responsible for:

- Reading `id` from route params.
- Loading the legal person/entity detail.
- Rendering the common page header.
- Rendering edit/delete actions.
- Rendering the tab navigation.
- Rendering the nested route outlet.
- Passing required data to tab sections.
- Preserving the existing behavior of the current detail page.

The nested routes inside the layout should be:

```tsx
<Routes>
  <Route index element={<PersonGeneralTab />} />
  <Route path="participations/*" element={<PersonParticipationsSection />} />
  <Route path="board/*" element={<PersonBoardSection />} />
</Routes>
```

## 7. Tabs

The legal person detail layout must display the following tabs:

```txt
General
Participaciones
Junta directiva
```

### 7.1 General tab

The `General` tab must contain the current legal person detail information.

The existing detail fields/grid should be extracted into a presentational component to avoid duplication.

Recommended new component:

```txt
src/pages/persons/PersonDetailFields.tsx
```

Responsibilities:

- Render the current detail information.
- Receive `data` as prop.
- Avoid owning page-level layout concerns.
- Avoid rendering page title, back link, delete action, or edit action.

The current `PersonDetailPage` used by natural persons should continue to work without tabs.

### 7.2 Participaciones tab

The `Participaciones` tab must show only participations for the current company/entity.

It must support:

- List participations scoped by `companyId`.
- Create a new participation with the company pre-filled and locked.
- View participation detail.
- Edit participation.
- Delete participation.
- Navigate back to the scoped participation list.

Target nested paths:

```txt
/companies/:id/participations
/companies/:id/participations/new
/companies/:id/participations/:participationId
/companies/:id/participations/:participationId/edit

/entities/:id/participations
/entities/:id/participations/new
/entities/:id/participations/:participationId
/entities/:id/participations/:participationId/edit
```

### 7.3 Junta directiva tab

The `Junta directiva` tab must show only board members for the current company/entity.

It must support:

- List board members scoped by `companyId`.
- Create a new board member with the company pre-filled and locked.
- View board member detail.
- Edit board member.
- Delete board member.
- Navigate back to the scoped board list.

Target nested paths:

```txt
/companies/:id/board
/companies/:id/board/new
/companies/:id/board/:boardMemberId
/companies/:id/board/:boardMemberId/edit

/entities/:id/board
/entities/:id/board/new
/entities/:id/board/:boardMemberId
/entities/:id/board/:boardMemberId/edit
```

## 8. Sidebar Changes

Update:

```txt
src/components/shared/Sidebar.tsx
```

Remove the following top-level sidebar entries:

```txt
Participaciones
Junta directiva
```

Also remove unused imports/icons if they become orphaned after the sidebar cleanup.

The sidebar should keep the main navigation focused on primary entities and configuration.

Recommended resulting structure:

```txt
Personas
  - Personas Naturales
  - Personas Jurídicas
  - Entes Jurídicos

General
  - Tipos de persona
  - Tipos de documento
  - Países
  - Cargos

Usuarios
```

## 9. Remove Global Routes

Update:

```txt
src/router/index.tsx
```

Remove global routes:

```tsx
<Route path="participations/*" ... />
<Route path="board/*" ... />
```

Also remove unused imports:

```ts
ParticipationsPage;
BoardPage;
```

The following URLs should no longer resolve:

```txt
/participations
/participations/:id
/participations/:id/edit
/participations/new

/board
/board/:id
/board/:id/edit
/board/new
```

Expected behavior for removed routes:

```txt
404 / Not Found
```

No redirects are required.

## 10. Scoped Participation Section

Existing participation pages must be repurposed as scoped components.

### 10.1 ParticipationListPage

Update the component to accept:

```ts
type Props = {
  companyId: string;
  basePath: string;
};
```

Behavior changes:

- Remove company filter/search from the list.
- Remove internal `companyId` state.
- Use the provided `companyId` prop when calling `useParticipationsQuery`.
- Remove the `Empresa` column because all rows already belong to the current company.
- Keep `Accionista`, percentage, start date, and end date columns.
- The `Nueva participación` button must navigate to:

```ts
`${basePath}/new`;
```

- Row detail navigation must use:

```ts
`${basePath}/${id}`;
```

- Remove large page-level header if the parent layout already provides the company/entity title.

### 10.2 ParticipationFormPage

Update the component to accept:

```ts
type Props = {
  lockedCompanyId: string;
  basePath: string;
};
```

Behavior changes:

- Initialize the form with the provided `lockedCompanyId`.
- Disable or remove the company selector.
- Display the current company/entity name as read-only context.
- Keep shareholder selection editable.
- On create success, navigate to:

```ts
`${basePath}/${createdId}`;
```

- On update success, navigate to:

```ts
`${basePath}/${participationId}`;
```

- Back navigation must go to:

```ts
basePath;
```

### 10.3 ParticipationDetailPage

Update the component to accept:

```ts
type Props = {
  basePath: string;
};
```

Behavior changes:

- Back link must navigate to:

```ts
basePath;
```

- Edit button must navigate to:

```ts
`${basePath}/${id}/edit`;
```

- On delete success, navigate to:

```ts
basePath;
```

- Keep link to the shareholder if applicable.
- Avoid linking to the current company detail from inside its own participation tab unless needed.

### 10.4 PersonParticipationsSection

Create a scoped section component:

```txt
src/pages/persons/PersonParticipationsSection.tsx
```

Responsibilities:

- Read current legal person/entity id from route params or props.
- Define the scoped `basePath`.
- Render nested participation routes.

Recommended route shape:

```tsx
<Routes>
  <Route
    index
    element={<ParticipationListPage companyId={id} basePath={basePath} />}
  />
  <Route
    path="new"
    element={<ParticipationFormPage lockedCompanyId={id} basePath={basePath} />}
  />
  <Route
    path=":participationId"
    element={<ParticipationDetailPage basePath={basePath} />}
  />
  <Route
    path=":participationId/edit"
    element={<ParticipationFormPage lockedCompanyId={id} basePath={basePath} />}
  />
</Routes>
```

## 11. Scoped Board Section

The board module must mirror the participation module changes.

### 11.1 BoardMemberListPage

Update the component to accept:

```ts
type Props = {
  companyId: string;
  basePath: string;
};
```

Behavior changes:

- Remove company filter/search from the list.
- Remove internal `companyId` state.
- Use provided `companyId` when calling `useBoardMembersQuery`.
- Remove company column.
- The `Nuevo miembro` button must navigate to:

```ts
`${basePath}/new`;
```

- Row detail navigation must use:

```ts
`${basePath}/${id}`;
```

### 11.2 BoardMemberFormPage

Update the component to accept:

```ts
type Props = {
  lockedCompanyId: string;
  basePath: string;
};
```

Behavior changes:

- Initialize the form with the provided `lockedCompanyId`.
- Disable or remove the company selector.
- Display the company/entity name as read-only context.
- Keep person/member and position fields editable.
- On create/update success, navigate using the scoped `basePath`.

### 11.3 BoardMemberDetailPage

Update the component to accept:

```ts
type Props = {
  basePath: string;
};
```

Behavior changes:

- Back link must navigate to `basePath`.
- Edit button must navigate to `${basePath}/${id}/edit`.
- On delete success, navigate to `basePath`.

### 11.4 PersonBoardSection

Create a scoped section component:

```txt
src/pages/persons/PersonBoardSection.tsx
```

Responsibilities:

- Read current legal person/entity id.
- Define the scoped `basePath`.
- Render nested board member routes.

Recommended route shape:

```tsx
<Routes>
  <Route
    index
    element={<BoardMemberListPage companyId={id} basePath={basePath} />}
  />
  <Route
    path="new"
    element={<BoardMemberFormPage lockedCompanyId={id} basePath={basePath} />}
  />
  <Route
    path=":boardMemberId"
    element={<BoardMemberDetailPage basePath={basePath} />}
  />
  <Route
    path=":boardMemberId/edit"
    element={<BoardMemberFormPage lockedCompanyId={id} basePath={basePath} />}
  />
</Routes>
```

## 12. Summary Panels

Update existing summary panels if they contain hardcoded global detail links.

Affected components:

```txt
src/components/shared/ParticipationSummaryPanel.tsx
src/components/shared/BoardMembersSummaryPanel.tsx
```

Add a prop such as:

```ts
detailPath?: string;
basePath?: string;
```

The link `Ver detalle completo` must point to the scoped path:

```txt
${basePath}/${id}
```

It must no longer point to:

```txt
/participations/:id
/board/:id
```

## 13. Permissions

Existing permission behavior must be preserved.

Tabs must be hidden or disabled based on read permissions:

```txt
Permission.ParticipationRead
Permission.BoardRead
```

Create/edit/delete buttons must continue to respect their existing permissions through current `PermissionGuard` usage.

Expected behavior:

- If the user lacks `ParticipationRead`, the `Participaciones` tab must not be visible.
- If the user lacks `BoardRead`, the `Junta directiva` tab must not be visible.
- If the user has read but not write/delete permission, the tab is visible but mutation actions remain hidden.

## 14. Empty States

When a company/entity has no participations, the tab must show a clear empty state:

```txt
Esta empresa todavía no tiene participaciones registradas.
```

Primary CTA:

```txt
Agregar primera participación
```

When a company/entity has no board members, the tab must show:

```txt
Esta empresa todavía no tiene miembros de junta directiva registrados.
```

Primary CTA:

```txt
Agregar primer miembro
```

The empty state must not look like a broken table.

## 15. UX Requirements

- The user must remain inside the company/entity context while managing participations and board members.
- The company/entity name must remain visible in the detail layout.
- Create forms must pre-fill and lock the company/entity.
- The user must not need to search/filter for the same company/entity they are already editing.
- Back links must return to the current tab list, not to a global module.
- Tabs must use route-based active state.
- Browser back/forward navigation must work correctly.
- URLs must be shareable and deep-linkable.

## 16. Reuse Strategy

Do not rewrite the participation or board modules from scratch.

Reuse existing:

- `usePersonDetailQuery`
- `useParticipationsQuery`
- `useBoardMembersQuery`
- create/update/delete mutations
- `personBaseRoute`
- `PageHeader`
- `ConfirmDialog`
- `PermissionGuard`
- `DataTable`
- `SummaryPanel`
- `useMediaQuery`
- existing UI tab styles/components
- existing person type checks

The implementation should be a routing/layout refactor, not a backend or business logic rewrite.

## 17. Edge Cases

- Route order must keep `:id/edit` before `:id/*`.
- Natural persons under `/people/:id` must continue using the current detail page without tabs.
- Companies and entities must show tabs.
- If the user lacks read permission for a tab, the tab must not appear.
- If a user manually navigates to a removed global route, the app must show Not Found.
- If a company/entity has no participations or board members, show an empty state with a primary CTA.
- If a created participation/board member succeeds, navigation must stay inside the scoped company/entity route.
- If a participation/board member is deleted, navigation must return to the scoped list tab.

## 18. Files to Change

### Sidebar

```txt
src/components/shared/Sidebar.tsx
```

Changes:

- Remove `Participaciones`.
- Remove `Junta directiva`.
- Remove unused icons/imports.

### Global router

```txt
src/router/index.tsx
```

Changes:

- Remove global `/participations/*` route.
- Remove global `/board/*` route.
- Remove unused imports.

### Companies routes

```txt
src/pages/persons/companies/index.tsx
```

Changes:

- Add nested route support for `:id/*`.
- Use `LegalPersonDetailLayout`.

### Entities routes

```txt
src/pages/persons/entities/index.tsx
```

Changes:

- Mirror company routing.
- Use `LegalPersonDetailLayout`.

### Legal person detail layout

```txt
src/pages/persons/LegalPersonDetailLayout.tsx
```

New file.

Responsibilities:

- Shared layout for companies and entities.
- Header, actions, tabs, nested routes.

### General detail fields

```txt
src/pages/persons/PersonDetailFields.tsx
```

New file.

Responsibilities:

- Presentational detail fields extracted from current `PersonDetailPage`.

### Person general tab

```txt
src/pages/persons/PersonGeneralTab.tsx
```

New file or internal component.

Responsibilities:

- Render general detail content inside the tabbed layout.

### Scoped participations section

```txt
src/pages/persons/PersonParticipationsSection.tsx
```

New file.

Responsibilities:

- Define nested scoped participation routes.

### Scoped board section

```txt
src/pages/persons/PersonBoardSection.tsx
```

New file.

Responsibilities:

- Define nested scoped board routes.

### Participation pages

```txt
src/pages/participations/ParticipationListPage.tsx
src/pages/participations/ParticipationFormPage.tsx
src/pages/participations/ParticipationDetailPage.tsx
```

Changes:

- Accept `companyId`, `lockedCompanyId`, and/or `basePath`.
- Remove global company filtering.
- Lock company field in forms.
- Update navigation paths.

### Board pages

```txt
src/pages/board/BoardMemberListPage.tsx
src/pages/board/BoardMemberFormPage.tsx
src/pages/board/BoardMemberDetailPage.tsx
```

Changes:

- Mirror participation page changes.
- Scope all behavior to current company/entity.

### Summary panels

```txt
src/components/shared/ParticipationSummaryPanel.tsx
src/components/shared/BoardMembersSummaryPanel.tsx
```

Changes:

- Replace hardcoded global paths with configurable scoped paths.

## 19. Verification

### Automated verification

Run:

```bash
cd app/equity-regulatory-reporting-app
pnpm lint
pnpm build
pnpm test
```

Expected result:

- Lint passes.
- TypeScript build passes.
- Vite build passes.
- No orphan imports.
- No broken routes due to removed global modules.

### Manual verification

With the backend running:

```bash
dotnet run
```

Verify:

1. Sidebar no longer shows `Participaciones`.
2. Sidebar no longer shows `Junta directiva`.
3. `/participations` shows Not Found.
4. `/board` shows Not Found.
5. `/companies/:id` shows tabs:
   - General
   - Participaciones
   - Junta directiva
6. `/companies/:id/participations` lists only participations for that company.
7. `Nueva participación` opens a form with the company pre-filled and locked.
8. Creating, editing, viewing, and deleting a participation returns to the scoped route.
9. `/companies/:id/board` behaves the same for board members.
10. `/entities/:id` shows the same tab behavior.
11. `/people/:id` for natural persons remains unchanged and does not show tabs.
12. Permission behavior still works:
    - no `ParticipationRead` means no `Participaciones` tab.
    - no `BoardRead` means no `Junta directiva` tab.

## 20. Acceptance Criteria

The implementation is complete when:

- `Participaciones` and `Junta directiva` are removed from the sidebar.
- Global participation and board routes are removed.
- Companies and entities expose nested tabs for general info, participations, and board members.
- Natural persons remain unchanged.
- Participation and board CRUD work only within the company/entity context.
- Company/entity is pre-filled and locked when creating scoped records.
- All navigation remains within the scoped company/entity detail.
- Existing backend endpoints and hooks continue to be reused.
- The app builds successfully.
- The UI better reflects the regulatory reporting workflow.

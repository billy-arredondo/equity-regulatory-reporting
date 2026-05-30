# SPEC 02: Frontend Testing - Phase 1

## Objective

Set up an initial frontend testing foundation to validate critical application logic without depending on the backend or end-to-end tests.

## Scope

- Configure Vitest as the test runner.
- Integrate Testing Library, jest-dom, and jsdom.
- Add testing scripts to `package.json`.
- Create the base testing configuration.
- Add the first unit tests for critical reusable logic:
  - permissions
  - person types
  - utilities
  - auth store
  - HTTP wrapper

## Out of Scope

- Playwright.
- MSW.
- End-to-end tests.
- Visual regression tests.
- 100% test coverage.
- Exhaustive testing of all UI components.

## Acceptance Criteria

- The project can run frontend tests using `pnpm`.
- A stable frontend testing configuration exists.
- The first tests cover critical reusable logic.
- The backend is not required to run the tests.
- The phase is ready to be extended in future testing phases.

## Technical Notes

- Do not use `npm`.
- Use `pnpm` exclusively.
- Avoid fragile tests that are too tightly coupled to implementation details.
- Mock `fetch`, `localStorage`, and Zustand state only when necessary.
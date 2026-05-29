export const PersonType = {
  Natural: 1,
  Legal: 2,
  LegalEntity: 3,
} as const;

export type PersonTypeValue = (typeof PersonType)[keyof typeof PersonType];

const labels: Record<PersonTypeValue, string> = {
  [PersonType.Natural]: "Persona natural",
  [PersonType.Legal]: "Persona jurídica",
  [PersonType.LegalEntity]: "Ente jurídico",
};

export function personTypeLabel(type: PersonTypeValue): string {
  return labels[type];
}

export const PERSON_TYPE_OPTIONS = (
  Object.values(PersonType) as PersonTypeValue[]
).map((v) => ({ value: v, label: labels[v] }));

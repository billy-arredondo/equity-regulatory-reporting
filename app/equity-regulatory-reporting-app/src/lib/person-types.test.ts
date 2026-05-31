import { describe, it, expect } from "vitest";
import { PersonType, personTypeLabel, PERSON_TYPE_OPTIONS } from "./person-types";

describe("PersonType", () => {
  it("has the expected numeric values", () => {
    expect(PersonType.Natural).toBe(1);
    expect(PersonType.Legal).toBe(2);
    expect(PersonType.LegalEntity).toBe(3);
  });
});

describe("personTypeLabel", () => {
  it("returns the correct Spanish label for each type", () => {
    expect(personTypeLabel(PersonType.Natural)).toBe("Persona natural");
    expect(personTypeLabel(PersonType.Legal)).toBe("Persona jurídica");
    expect(personTypeLabel(PersonType.LegalEntity)).toBe("Ente jurídico");
  });
});

describe("PERSON_TYPE_OPTIONS", () => {
  it("has exactly 3 options", () => {
    expect(PERSON_TYPE_OPTIONS).toHaveLength(3);
  });

  it("each option has value and label properties", () => {
    for (const option of PERSON_TYPE_OPTIONS) {
      expect(option).toHaveProperty("value");
      expect(option).toHaveProperty("label");
    }
  });

  it("options match the PersonType values with correct labels", () => {
    expect(PERSON_TYPE_OPTIONS).toEqual([
      { value: PersonType.Natural, label: "Persona natural" },
      { value: PersonType.Legal, label: "Persona jurídica" },
      { value: PersonType.LegalEntity, label: "Ente jurídico" },
    ]);
  });
});

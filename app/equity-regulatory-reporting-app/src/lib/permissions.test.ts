import { describe, it, expect } from "vitest";
import { Permission } from "./permissions";

describe("Permission", () => {
  it("None is 0", () => {
    expect(Permission.None).toBe(0);
  });

  it("PersonRead is bit 0 (1)", () => {
    expect(Permission.PersonRead).toBe(1);
  });

  it("PersonWrite is bit 1 (2)", () => {
    expect(Permission.PersonWrite).toBe(2);
  });

  it("PersonDelete is bit 2 (4)", () => {
    expect(Permission.PersonDelete).toBe(4);
  });

  it("PositionDelete is bit 22", () => {
    expect(Permission.PositionDelete).toBe(1 << 22);
  });

  it("Admin equals ~0 (-1, all bits set)", () => {
    expect(Permission.Admin).toBe(~0);
  });

  it("Admin grants any individual permission via bitwise AND", () => {
    const flags = [
      Permission.PersonRead,
      Permission.ParticipationWrite,
      Permission.BoardDelete,
      Permission.UserRead,
      Permission.PositionDelete,
    ] as const;

    for (const flag of flags) {
      expect((Permission.Admin & flag) !== 0).toBe(true);
    }
  });

  it("each permission is a unique power of two (except None and Admin)", () => {
    const skip = new Set([Permission.None, Permission.Admin]);
    const values = Object.values(Permission).filter((v) => !skip.has(v));
    const unique = new Set(values);
    expect(unique.size).toBe(values.length);

    for (const v of values) {
      // power of two: exactly one bit set
      expect(v & (v - 1)).toBe(0);
    }
  });
});

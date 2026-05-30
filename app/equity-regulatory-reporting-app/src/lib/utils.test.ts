import { describe, it, expect } from "vitest";
import { cn } from "./utils";

describe("cn", () => {
  it("merges class strings", () => {
    expect(cn("foo", "bar")).toBe("foo bar");
  });

  it("deduplicates conflicting Tailwind classes (last one wins)", () => {
    expect(cn("p-2", "p-4")).toBe("p-4");
    expect(cn("text-sm", "text-lg")).toBe("text-lg");
  });

  it("filters out falsy values", () => {
    // Use a typed boolean (not a literal false) to avoid the
    // no-constant-binary-expression lint rule while still testing falsy input.
    const show = false as boolean;
    expect(cn("a", show && "b", null, undefined, "c")).toBe("a c");
  });

  it("handles conditional class objects", () => {
    expect(cn({ foo: true, bar: false, baz: true })).toBe("foo baz");
  });

  it("handles arrays of classes", () => {
    expect(cn(["a", "b"], "c")).toBe("a b c");
  });

  it("returns empty string when no classes are provided", () => {
    expect(cn()).toBe("");
  });
});

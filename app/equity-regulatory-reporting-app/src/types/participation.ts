import type { PersonTypeValue } from "@/lib/person-types";

export interface ParticipationDto {
  id: string;
  companyId: string;
  companyName: string;
  companyPersonType: PersonTypeValue;
  shareholderId: string;
  shareholderName: string;
  shareholderPersonType: PersonTypeValue;
  percentage: number;
  effectiveFrom: string;
  effectiveTo: string | null;
}

export interface CreateParticipationDto {
  companyId: string;
  shareholderId: string;
  percentage: number;
  effectiveFrom: string;
  effectiveTo: string | null;
}

export type UpdateParticipationDto = CreateParticipationDto;

export interface ParticipationDto {
  id: string;
  companyId: string;
  companyName: string;
  shareholderId: string;
  shareholderName: string;
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

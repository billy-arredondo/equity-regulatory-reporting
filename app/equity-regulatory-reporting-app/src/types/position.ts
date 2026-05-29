export interface PositionDto {
  id: string;
  name: string;
  reportCode: string;
}

export interface CreatePositionDto {
  name: string;
  reportCode: string;
}

export type UpdatePositionDto = CreatePositionDto;

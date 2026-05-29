export interface PositionDto {
  id: string;
  name: string;
}

export interface CreatePositionDto {
  name: string;
}

export type UpdatePositionDto = CreatePositionDto;

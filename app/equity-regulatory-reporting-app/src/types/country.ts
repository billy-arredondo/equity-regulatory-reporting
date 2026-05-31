export interface CountryDto {
  id: string;
  name: string;
  abbreviation: string;
}

export interface CreateCountryDto {
  name: string;
  abbreviation: string;
}

export type UpdateCountryDto = CreateCountryDto;

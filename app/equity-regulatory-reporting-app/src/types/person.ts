import type { PersonTypeValue } from "@/lib/person-types";

export interface PersonDto {
  id: string;
  name: string;
  personType: PersonTypeValue;
  documentTypeName: string;
  documentNumber: string;
  countryName: string;
  reportFlag: boolean;
}

export interface PersonDetailDto extends PersonDto {
  ciiu: string;
  address: string;
  documentTypeId: string;
  entityCode: string | null;
  representativeId: string | null;
  representativeName: string | null;
  countryId: string;
  internalLocation: string;
}

export interface CreatePersonDto {
  name: string;
  personType: PersonTypeValue;
  ciiu: string;
  address: string;
  documentTypeId: string;
  documentNumber: string;
  entityCode: string | null;
  representativeId: string | null;
  reportFlag: boolean;
  countryId: string;
  internalLocation: string;
}

export type UpdatePersonDto = CreatePersonDto;

import type { PersonTypeValue } from "@/lib/person-types";

export interface DocumentTypeDto {
  id: string;
  name: string;
  abbreviation: string;
  validationRegex: string | null;
  allowedPersonTypes: PersonTypeValue[];
}

export interface CreateDocumentTypeDto {
  name: string;
  abbreviation: string;
  validationRegex: string | null;
  allowedPersonTypes: PersonTypeValue[];
}

export type UpdateDocumentTypeDto = CreateDocumentTypeDto;

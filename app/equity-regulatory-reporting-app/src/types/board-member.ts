export interface BoardMemberDto {
  id: string;
  companyId: string;
  companyName: string;
  memberId: string;
  memberName: string;
  primaryPositionId: string;
  primaryPositionName: string;
  secondaryPositionId: string;
  secondaryPositionName: string;
}

export interface CreateBoardMemberDto {
  companyId: string;
  memberId: string;
  primaryPositionId: string;
  secondaryPositionId: string | null;
}

export type UpdateBoardMemberDto = CreateBoardMemberDto;

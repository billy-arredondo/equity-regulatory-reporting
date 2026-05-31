import { Route, Routes } from "react-router-dom";
import { BoardMemberListPage } from "./BoardMemberListPage";
import { BoardMemberDetailPage } from "./BoardMemberDetailPage";
import { BoardMemberFormPage } from "./BoardMemberFormPage";

interface Props {
  companyId: string;
  companyName: string;
  basePath: string;
}

export function PersonBoardSection({ companyId, companyName, basePath }: Props) {
  return (
    <Routes>
      <Route
        index
        element={<BoardMemberListPage companyId={companyId} basePath={basePath} />}
      />
      <Route
        path="new"
        element={
          <BoardMemberFormPage
            lockedCompanyId={companyId}
            lockedCompanyName={companyName}
            basePath={basePath}
          />
        }
      />
      <Route
        path=":boardMemberId"
        element={<BoardMemberDetailPage basePath={basePath} />}
      />
      <Route
        path=":boardMemberId/edit"
        element={
          <BoardMemberFormPage
            lockedCompanyId={companyId}
            lockedCompanyName={companyName}
            basePath={basePath}
          />
        }
      />
    </Routes>
  );
}

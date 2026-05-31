import { Route, Routes } from "react-router-dom";
import { ParticipationListPage } from "./ParticipationListPage";
import { ParticipationDetailPage } from "./ParticipationDetailPage";
import { ParticipationFormPage } from "./ParticipationFormPage";

interface Props {
  companyId: string;
  companyName: string;
  basePath: string;
}

export function PersonParticipationsSection({ companyId, companyName, basePath }: Props) {
  return (
    <Routes>
      <Route
        index
        element={<ParticipationListPage companyId={companyId} basePath={basePath} />}
      />
      <Route
        path="new"
        element={
          <ParticipationFormPage
            lockedCompanyId={companyId}
            lockedCompanyName={companyName}
            basePath={basePath}
          />
        }
      />
      <Route
        path=":participationId"
        element={<ParticipationDetailPage basePath={basePath} />}
      />
      <Route
        path=":participationId/edit"
        element={
          <ParticipationFormPage
            lockedCompanyId={companyId}
            lockedCompanyName={companyName}
            basePath={basePath}
          />
        }
      />
    </Routes>
  );
}

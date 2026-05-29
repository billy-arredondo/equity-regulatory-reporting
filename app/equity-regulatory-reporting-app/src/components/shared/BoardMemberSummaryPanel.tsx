import { Users } from "lucide-react";
import { SummaryPanel, SummaryRow } from "./SummaryPanel";
import { useBoardMemberDetailQuery } from "@/hooks/useBoardMembers";

interface Props {
  boardMemberId: string;
  open?: boolean;
  onOpenChange?: (open: boolean) => void;
}

export function BoardMemberSummaryPanel({ boardMemberId, open, onOpenChange }: Props) {
  const { data, isLoading } = useBoardMemberDetailQuery(boardMemberId);
  return (
    <SummaryPanel
      icon={<Users className="h-4 w-4 text-muted-foreground" />}
      title="Ficha de miembro"
      isLoading={isLoading}
      hasData={!!data}
      detailPath={`/board/${boardMemberId}`}
      open={open}
      onOpenChange={onOpenChange}
    >
      {data && (
        <>
          <SummaryRow label="Empresa" value={data.companyName} />
          <SummaryRow label="Miembro" value={data.memberName} />
          <SummaryRow label="Cargo principal" value={data.primaryPositionName} />
          <SummaryRow label="Cargo secundario" value={data.secondaryPositionName} />
        </>
      )}
    </SummaryPanel>
  );
}

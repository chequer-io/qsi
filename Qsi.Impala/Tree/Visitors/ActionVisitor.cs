using System;
using Qsi.Impala.Internal;
using Qsi.Tree;

namespace Qsi.Impala.Tree.Visitors
{
    using static ImpalaParserInternal;

    internal static class ActionVisitor
    {
        public static IQsiTreeNode VisitUseStmt(Use_stmtContext context)
        {
            throw new NotImplementedException();
        }

        public static IQsiTreeNode VisitUpsertStmt(Upsert_stmtContext context)
        {
            throw new NotImplementedException();
        }

        public static IQsiTreeNode VisitUpdateStmt(Update_stmtContext context)
        {
            throw new NotImplementedException();
        }

        public static IQsiTreeNode VisitInsertStmt(Insert_stmtContext context)
        {
            throw new NotImplementedException();
        }

        public static IQsiTreeNode VisitDeleteStmt(Delete_stmtContext context)
        {
            throw new NotImplementedException();
        }
    }
}

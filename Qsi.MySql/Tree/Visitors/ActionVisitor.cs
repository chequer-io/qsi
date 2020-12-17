using Qsi.Tree;
using static Qsi.MySql.Internal.MySqlParserInternal;

namespace Qsi.MySql.Tree
{
    internal static class ActionVisitor
    {
        public static QsiActionNode VisitDeleteStatement(DeleteStatementContext context)
        {
            throw new System.NotImplementedException();
        }

        public static QsiActionNode VisitReplaceStatement(ReplaceStatementContext context)
        {
            throw new System.NotImplementedException();
        }

        public static QsiActionNode VisitUpdateStatement(UpdateStatementContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}

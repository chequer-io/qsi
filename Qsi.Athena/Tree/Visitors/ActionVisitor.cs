using Qsi.Athena.Internal;
using Qsi.Tree;

namespace Qsi.Athena.Tree.Visitors
{
    using static SqlBaseParser;

    internal static class ActionVisitor
    {
        public static IQsiTreeNode VisitStatementDefault(StatementDefaultContext context)
        {
            return TableVisitor.VisitQuery(context.query());
        }
    }
}

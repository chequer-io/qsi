using Antlr4.Runtime.Tree;
using Qsi.Tree.Base;
using static Qsi.MySql.Internal.MySqlParser;

namespace Qsi.MySql.Tree
{
    internal static class TableVisitor
    {
        public static QsiTableNode Visit(IParseTree tree)
        {
            throw new System.NotImplementedException();
        }

        public static QsiTableNode VisitSelectStatement(SelectStatementContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}

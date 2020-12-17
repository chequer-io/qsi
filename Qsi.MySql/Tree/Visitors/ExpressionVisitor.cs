using Qsi.Tree;
using static Qsi.MySql.Internal.MySqlParserInternal;

namespace Qsi.MySql.Tree
{
    internal static class ExpressionVisitor
    {
        public static QsiMultipleOrderExpressionNode VisitOrderClause(OrderClauseContext context)
        {
            throw new System.NotImplementedException();
        }

        public static QsiLimitExpressionNode VisitLimitClause(LimitClauseContext context)
        {
            throw new System.NotImplementedException();
        }

        public static QsiExpressionNode VisitExpr(ExprContext context)
        {
            throw new System.NotImplementedException();
        }

        public static QsiWhereExpressionNode VisitWhereClause(WhereClauseContext context)
        {
            throw new System.NotImplementedException();
        }

        public static QsiGroupingExpressionNode VisitGroupByClause(GroupByClauseContext context)
        {
            throw new System.NotImplementedException();
        }

        public static QsiExpressionNode VisitHavingClause(HavingClauseContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}

using System;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using static Qsi.Hana.Internal.HanaParserInternal;

namespace Qsi.Hana.Tree.Visitors
{
    internal static class TableVisitor
    {
        public static HanaDerivedTableNode VisitSelectStatement(SelectStatementContext context)
        {
            var withClause = context.withClause();
            var subquery = context.subquery();
            var forClause = context.forClause();
            var timeTravel = context.timeTravel();
            var hintClause = context.hintClause();

            var subqueryNode = VisitSubquery(subquery);

            if (withClause is not null)
                subqueryNode.Directives.SetValue(VisitWithClause(withClause));

            if (forClause is not null)
                subqueryNode.Behavior.SetValue(VisitForClause(forClause));

            if (timeTravel is not null)
                subqueryNode.TimeTravel = timeTravel.GetInputText();

            if (hintClause is not null)
                subqueryNode.Hint = hintClause.GetInputText();

            return subqueryNode;
        }

        private static QsiTableDirectivesNode VisitWithClause(WithClauseContext context)
        {
            throw new NotImplementedException();
        }

        private static HanaDerivedTableNode VisitSubquery(SubqueryContext context)
        {
            if (context.inner != null)
                return VisitSelectStatement(context.inner);

            var node = new HanaDerivedTableNode();

            node.Columns.SetValue(VisitSelectClause(context.select));
            node.Source.SetValue(VisitFromClause(context.from));

            if (context.where != null)
                node.Where.SetValue(VisitWhereClause(context.where));

            if (context.groupBy != null)
                node.Grouping.SetValue(VisitGroupByClause(context.groupBy));

            // TODO: set

            if (context.orderBy != null)
                node.Order.SetValue(VisitOrderByClause(context.orderBy));

            if (context.limit != null)
                node.Limit.SetValue(VisitLimitClause(context.limit));

            HanaTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiColumnsDeclarationNode VisitSelectClause(SelectClauseContext context)
        {
            throw new NotImplementedException();
        }

        private static QsiTableNode VisitFromClause(FromClauseContext context)
        {
            throw new NotImplementedException();
        }

        private static QsiWhereExpressionNode VisitWhereClause(WhereClauseContext context)
        {
            throw new NotImplementedException();
        }

        private static QsiGroupingExpressionNode VisitGroupByClause(GroupByClauseContext context)
        {
            throw new NotImplementedException();
        }

        private static QsiMultipleOrderExpressionNode VisitOrderByClause(TableOrderByClauseContext context)
        {
            throw new NotImplementedException();
        }

        private static QsiLimitExpressionNode VisitLimitClause(LimitClauseContext context)
        {
            throw new NotImplementedException();
        }

        private static HanaTableBehaviorNode VisitForClause(ForClauseContext context)
        {
            throw new NotImplementedException();
        }
    }
}

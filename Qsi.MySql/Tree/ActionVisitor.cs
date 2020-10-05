using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;
using static Qsi.MySql.Internal.MySqlParser;

namespace Qsi.MySql.Tree
{
    internal static class ActionVisitor
    {
        public static IEnumerable<QsiActionNode> Visit(IParseTree tree)
        {
            switch (tree)
            {
                case RootContext rootContext:
                    return VisitRoot(rootContext);

                case SqlStatementContext statementContext:
                    return new[] { VisitSqlStatement(statementContext) };

                case PreparedStatementContext preparedStatementContext:
                    return new[] { VisitPreparedStatement(preparedStatementContext) };
            }

            return Enumerable.Empty<QsiActionNode>();
        }

        public static IEnumerable<QsiActionNode> VisitRoot(RootContext context)
        {
            if (context.sqlStatements() == null)
                yield break;

            foreach (var statementContext in context.sqlStatements().sqlStatement())
            {
                var r = VisitSqlStatement(statementContext);

                if (r != null)
                    yield return r;
            }
        }

        public static QsiActionNode VisitSqlStatement(SqlStatementContext context)
        {
            if (context.children.Count == 0)
                return null;

            switch (context.children[0])
            {
                case PreparedStatementContext preparedStatementContext:
                    return VisitPreparedStatement(preparedStatementContext);
            }

            return null;
        }

        public static QsiActionNode VisitPreparedStatement(PreparedStatementContext context)
        {
            switch (context.children[0])
            {
                case PrepareStatementContext prepareStatementContext:
                    return VisitPrepareStatement(prepareStatementContext);

                case ExecuteStatementContext executeStatementContext:
                    return VisitExecuteStatement(executeStatementContext);

                case DeallocatePrepareContext deallocatePrepareContext:
                    return VisitDeallocatePrepare(deallocatePrepareContext);
            }

            return null;
        }

        public static QsiActionNode VisitPrepareStatement(PrepareStatementContext context)
        {
            return TreeHelper.Create<QsiPrepareActionNode>(n =>
            {
                n.Identifier = IdentifierVisitor.Visit(context.uid());

                if (context.query != null)
                {
                    n.Query.SetValue(new QsiLiteralExpressionNode
                    {
                        Value = IdentifierUtility.Unescape(context.query.Text),
                        Type = QsiDataType.String
                    });
                }
                else if (context.variable != null)
                {
                    n.Query.SetValue(ExpressionVisitor.VisitLocalId(context.LOCAL_ID()));
                }
            });
        }

        public static QsiActionNode VisitDeallocatePrepare(DeallocatePrepareContext context)
        {
            return TreeHelper.Create<QsiDropPrepareActionNode>(n =>
            {
                n.Identifier = IdentifierVisitor.Visit(context.uid());
            });
        }

        public static QsiActionNode VisitExecuteStatement(ExecuteStatementContext context)
        {
            return TreeHelper.Create<QsiExecutePrepareActionNode>(n =>
            {
                n.Identifier = IdentifierVisitor.Visit(context.uid());

                if (context.userVariables() != null)
                {
                    n.Variables.SetValue(ExpressionVisitor.VisitUserVariables(context.userVariables()));
                }
            });
        }
    }
}

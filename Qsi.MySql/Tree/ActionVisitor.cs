using System;
using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.MySql.Tree.Common;
using Qsi.Tree;
using Qsi.Utilities;
using static Qsi.MySql.Internal.MySqlParser;

namespace Qsi.MySql.Tree
{
    internal static class ActionVisitor
    {
        #region Prepared
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
                    var literal = TreeHelper.CreateLiteral(IdentifierUtility.Unescape(context.query.Text));
                    MySqlTree.PutContextSpan(literal, context.query);

                    n.Query.SetValue(literal);
                }
                else if (context.variable != null)
                {
                    n.Query.SetValue(ExpressionVisitor.VisitLocalId(context.LOCAL_ID()));
                }

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiActionNode VisitDeallocatePrepare(DeallocatePrepareContext context)
        {
            return TreeHelper.Create<QsiDropPrepareActionNode>(n =>
            {
                n.Identifier = IdentifierVisitor.Visit(context.uid());

                MySqlTree.PutContextSpan(n, context);
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

                MySqlTree.PutContextSpan(n, context);
            });
        }
        #endregion

        #region Insert, Replace
        public static QsiActionNode VisitInsertStatement(InsertStatementContext context)
        {
            return VisitInsertStatement(new CommonInsertStatementContext(context));
        }

        public static QsiActionNode VisitReplaceStatement(ReplaceStatementContext context)
        {
            return VisitInsertStatement(new CommonInsertStatementContext(context));
        }

        private static QsiActionNode VisitInsertStatement(CommonInsertStatementContext context)
        {
            var node = new QsiDataInsertActionNode
            {
                ConflictBehavior = context.ConflictBehavior
            };

            if (context.TableName == null)
                throw new QsiException(QsiError.Syntax);

            node.Target.SetValue(TableVisitor.VisitTableName(context.TableName));

            if (!ListUtility.IsNullOrEmpty(context.Partitions))
            {
                node.Partitions = context.Partitions
                    .Select(uid => new QsiQualifiedIdentifier(IdentifierVisitor.VisitUid(uid)))
                    .ToArray();
            }

            if (!ListUtility.IsNullOrEmpty(context.Columns))
            {
                node.Columns = context.Columns.Select(IdentifierVisitor.VisitUid).ToArray();
            }

            if (context.InsertStatementValue != null)
            {
                var value = context.InsertStatementValue;

                if (value.selectStatement() != null)
                {
                    node.ValueTable.SetValue(TableVisitor.VisitSelectStatement(value.selectStatement()));
                }
                else
                {
                    IEnumerable<QsiRowValueExpressionNode> rows = value.expressionsWithDefaults()
                        .Select(ExpressionVisitor.VisitExpressionsWithDefaults);

                    node.Values.AddRange(rows);
                }
            }
            else if (!ListUtility.IsNullOrEmpty(context.SetElements))
            {
                node.SetValues.AddRange(context.SetElements.Select(ExpressionVisitor.VisitUpdatedElement));
            }
            else
            {
                throw new QsiException(QsiError.Syntax);
            }

            if (!ListUtility.IsNullOrEmpty(context.DuplicateSetElements))
            {
                var action = new QsiDataConflictActionNode();
                action.SetValues.AddRange(context.DuplicateSetElements.Select(ExpressionVisitor.VisitUpdatedElement));

                var insertContext = (InsertStatementContext)context.Context;
                MySqlTree.PutContextSpan(action, insertContext.ON().Symbol, insertContext.Stop);

                node.ConflictAction.SetValue(action);
            }

            MySqlTree.PutContextSpan(node, context.Context);

            return node;
        }
        #endregion

        #region Delete
        public static QsiDataDeleteActionNode VisitDeleteStatement(DeleteStatementContext context)
        {
            switch (context.children[0])
            {
                case SingleDeleteStatementContext singleDeleteStatement:
                    return VisitSingleDeleteStatement(singleDeleteStatement);

                case MultipleDeleteStatementContext multipleDeleteStatement:
                    return VisitMultipleDeleteStatement(multipleDeleteStatement);

                default:
                    throw new QsiException(QsiError.Syntax);
            }
        }

        private static QsiDataDeleteActionNode VisitSingleDeleteStatement(SingleDeleteStatementContext context)
        {
            var table = (QsiTableNode)TableVisitor.VisitTableName(context.tableName());
            var where = context.expression();
            var order = context.orderByClause();
            var limitClause = context.limitClauseAtom();

            if (where != null || order != null || limitClause != null)
            {
                var derivedTable = new QsiDerivedTableNode();

                derivedTable.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
                derivedTable.Source.SetValue(table);

                if (where != null)
                {
                    var whereContext = new CommonWhereContext(where, context.FROM().Symbol, where.Stop);
                    derivedTable.WhereExpression.SetValue(ExpressionVisitor.VisitCommonWhere(whereContext));
                }

                if (order != null)
                {
                    derivedTable.OrderExpression.SetValue(ExpressionVisitor.VisitOrderByClause(order));
                }

                if (limitClause != null)
                {
                    var limitContext = new CommonLimitClauseContext(null, limitClause, context.LIMIT().Symbol, limitClause.Stop);
                    derivedTable.LimitExpression.SetValue(ExpressionVisitor.VisitCommonLimitClause(limitContext));
                }

                table = derivedTable;
            }

            var node = new QsiDataDeleteActionNode();

            node.Target.SetValue(table);
            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiDataDeleteActionNode VisitMultipleDeleteStatement(MultipleDeleteStatementContext context)
        {
            var derivedTable = TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                var columnsDeclaration = new QsiColumnsDeclarationNode();

                columnsDeclaration.Columns.AddRange(context.tableName()
                    .Select(name => new QsiAllColumnNode
                    {
                        Path = IdentifierVisitor.VisitFullId(name.fullId())
                    }));

                n.Columns.SetValue(columnsDeclaration);
                n.Source.SetValue(TableVisitor.VisitTableSources(context.tableSources()));

                if (context.expression() != null)
                {
                    var whereContext = new CommonWhereContext(context.expression(), context.WHERE().Symbol, context.Stop);
                    n.WhereExpression.SetValue(ExpressionVisitor.VisitCommonWhere(whereContext));
                }
            });

            var node = new QsiDataDeleteActionNode();

            node.Target.SetValue(derivedTable);
            MySqlTree.PutContextSpan(node, context);

            return node;
        }
        #endregion

        #region Update
        public static QsiDataUpdateActionNode VisitUpdateStatement(UpdateStatementContext context)
        {
            switch (context.children[0])
            {
                case SingleUpdateStatementContext singleUpdateStatement:
                    return VisitSingleUpdateStatement(singleUpdateStatement);

                case MultipleUpdateStatementContext multipleUpdateStatement:
                    return VisitMultipleUpdateStatement(multipleUpdateStatement);

                default:
                    throw new QsiException(QsiError.Syntax);
            }
        }

        private static QsiDataUpdateActionNode VisitSingleUpdateStatement(SingleUpdateStatementContext singleUpdateStatement)
        {
            throw new NotImplementedException();
        }

        private static QsiDataUpdateActionNode VisitMultipleUpdateStatement(MultipleUpdateStatementContext multipleUpdateStatement)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}

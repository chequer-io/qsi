using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;
using static PrimarSql.Internal.PrimarSqlParser;

namespace Qsi.PrimarSql.Tree
{
    internal static class ActionVisitor
    {
        #region Insert
        public static QsiActionNode VisitInsertStatement(InsertStatementContext context)
        {
            var node = new QsiDataInsertActionNode
            {
                ConflictBehavior = context.IGNORE() != null ? QsiDataConflictBehavior.Ignore : QsiDataConflictBehavior.None
            };

            if (context.tableName() == null)
                throw new QsiException(QsiError.Syntax);

            node.Target.SetValue(TableVisitor.VisitTableName(context.tableName()));

            switch (context.insertStatementValue())
            {
                case SubqueryInsertStatementContext _:
                {
                    throw TreeHelper.NotSupportedFeature("subquery insert");
                }

                case ExpressionInsertStatementContext expressionContext:
                {
                    if (context.columns != null)
                    {
                        node.Columns = context.columns.uid()
                            .Select(i => new QsiQualifiedIdentifier(IdentifierVisitor.VisitUid(i)))
                            .ToArray();
                    }

                    IEnumerable<QsiRowValueExpressionNode> rows = expressionContext
                        .expressionsWithDefaults()
                        .Select(ExpressionVisitor.VisitExpressionsWithDefaults);

                    node.Values.AddRange(rows);
                    break;
                }

                case JsonInsertStatementContext jsonContext:
                {
                    (JObject, JsonObjectContext x)[] objects = jsonContext.jsonObject()
                        .Select(x => (JObject.Parse(x.GetText()), x))
                        .ToArray();

                    string[] columns = objects
                        .SelectMany(x => x.Item1.Properties().Select(y => y.Name))
                        .Distinct()
                        .ToArray();

                    node.Columns = columns
                        .Select(x => new QsiQualifiedIdentifier(new QsiIdentifier(x, false)))
                        .ToArray();

                    foreach (var (obj, jsonObjectContext) in objects)
                    {
                        node.Values.Add(ExpressionVisitor.GetRowValueFromJObject(obj, jsonObjectContext, columns));
                    }

                    break;
                }
            }

            PrimarSqlTree.PutContextSpan(node, context);

            return node;
        }
        #endregion

        #region Delete
        public static QsiDataDeleteActionNode VisitDeleteStatement(DeleteStatementContext context)
        {
            var table = TableVisitor.VisitTableName(context.tableName());
            var where = context.expression();
            var limitClause = context.limitClause();

            var derivedTable = new QsiDerivedTableNode();

            derivedTable.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
            derivedTable.Source.SetValue(table);

            if (where != null)
            {
                derivedTable.Where.SetValue(TreeHelper.Create<QsiWhereExpressionNode>(n =>
                {
                    n.Expression.SetValue(ExpressionVisitor.VisitExpression(where));
                    PrimarSqlTree.PutContextSpan(n, context.whereKeyword, where.Stop);
                }));
            }

            if (limitClause != null)
            {
                derivedTable.Limit.SetValue(ExpressionVisitor.VisitLimitClause(limitClause));
            }

            var node = new QsiDataDeleteActionNode
            {
                Target = { Value = derivedTable }
            };

            PrimarSqlTree.PutContextSpan(derivedTable, context);

            return node;
        }
        #endregion

        #region Update
        public static QsiDataUpdateActionNode VisitUpdateStatement(UpdateStatementContext context)
        {
            var table = TableVisitor.VisitTableName(context.tableName());

            var where = context.expression();
            var limitClause = context.limitClause();

            var derivedTable = new QsiDerivedTableNode();

            derivedTable.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
            derivedTable.Source.SetValue(table);

            if (where != null)
            {
                derivedTable.Where.SetValue(TreeHelper.Create<QsiWhereExpressionNode>(n =>
                {
                    n.Expression.SetValue(ExpressionVisitor.VisitExpression(where));
                    PrimarSqlTree.PutContextSpan(n, context.whereKeyword, where.Stop);
                }));
            }

            if (limitClause != null)
            {
                derivedTable.Limit.SetValue(ExpressionVisitor.VisitLimitClause(limitClause));
            }

            var node = new QsiDataUpdateActionNode();

            node.Target.SetValue(derivedTable);
            node.SetValues.AddRange(ExpressionVisitor.VisitUpdateItem(context.updateItem()));

            PrimarSqlTree.PutContextSpan(derivedTable, context);

            return node;
        }
        #endregion
    }
}

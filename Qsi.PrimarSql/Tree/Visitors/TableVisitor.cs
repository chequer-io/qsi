﻿using System.Linq;
using Qsi.Tree;
using Qsi.Utilities;
using static PrimarSql.Internal.PrimarSqlParser;

namespace Qsi.PrimarSql.Tree;

internal static class TableVisitor
{
    public static QsiTableNode VisitRoot(RootContext context)
    {
        return VisitSqlStatement(context.sqlStatement());
    }

    public static QsiTableNode VisitSqlStatement(SqlStatementContext context)
    {
        if (context.dmlStatement() != null)
            return VisitDmlStatement(context.dmlStatement());

        return null;
    }

    public static QsiTableNode VisitDmlStatement(DmlStatementContext context)
    {
        if (context.selectStatement() != null)
            return VisitSelectStatement(context.selectStatement());

        return null;
    }

    public static QsiTableNode VisitSelectStatement(SelectStatementContext context)
    {
        switch (context)
        {
            case SimpleSelectContext simpleSelectContext:
                return VisitQuerySpecification(simpleSelectContext.querySpecification());

            case ParenthesisSelectContext parenthesisSelectContext:
                return VisitQueryExpression(parenthesisSelectContext.queryExpression());
        }

        return null;
    }

    public static QsiTableNode VisitQueryExpression(QueryExpressionContext context)
    {
        if (context.querySpecification() != null)
            return VisitQuerySpecification(context.querySpecification());

        return VisitQueryExpression(context.queryExpression());
    }

    public static QsiTableNode VisitQuerySpecification(QuerySpecificationContext context)
    {
        var node = new PrimarSqlDerivedTableNode
        {
            SelectSpec = VisitSelectSpec(context.selectSpec())
        };

        node.Columns.SetValue(VisitSelectElements(context.selectElements()));

        if (context.fromClause() != null)
        {
            node.Source.SetValue(VisitFromCluase(context.fromClause()));

            if (context.fromClause().whereExpr != null)
            {
                node.Where.SetValue(VisitWhereExpression(context.fromClause()));
            }
        }

        if (context.orderClause() != null)
        {
            node.Order.SetValue(TreeHelper.Create<QsiMultipleOrderExpressionNode>(n =>
            {
                n.Orders.Add(ExpressionVisitor.VisitOrderCluase(context.orderClause()));

                PrimarSqlTree.PutContextSpan(n, context.orderClause());
            }));
        }

        if (context.limitClause() != null)
            node.Limit.SetValue(ExpressionVisitor.VisitLimitClause(context.limitClause()));

        if (context.startKeyClause() != null)
            node.StartKey.SetValue(ExpressionVisitor.VisitStartKeyClause(context.startKeyClause()));

        PrimarSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static SelectSpec VisitSelectSpec(SelectSpecContext context)
    {
        if (context == null)
            return SelectSpec.Empty;

        if (context.STRONGLY() != null)
            return SelectSpec.Strongly;

        if (context.EVENTUALLY() != null)
            return SelectSpec.Eventually;

        return SelectSpec.Empty;
    }

    public static QsiColumnsDeclarationNode VisitSelectElements(SelectElementsContext context)
    {
        return TreeHelper.Create<QsiColumnsDeclarationNode>(n =>
        {
            if (context.star != null)
                n.Columns.Add(new QsiAllColumnNode());

            n.Columns.AddRange(context.selectElement().Select(VisitSelectElement));
        });
    }

    public static QsiColumnNode VisitSelectElement(SelectElementContext context)
    {
        QsiColumnNode node;

        switch (context)
        {
            case SelectColumnElementContext columnElementContext:
            {
                node = VisitSelectColumnElement(columnElementContext);
                break;
            }

            case SelectFunctionElementContext selectFunctionElementContext:
            {
                if (selectFunctionElementContext.builtInFunctionCall() is CountFunctionCallContext countFunctionCallContext)
                {
                    return TreeHelper.Create<QsiDerivedColumnNode>(n =>
                    {
                        n.Expression.SetValue(TreeHelper.Create<QsiInvokeExpressionNode>(en =>
                        {
                            en.Member.SetValue(TreeHelper.CreateFunction("count"));
                        }));

                        if (selectFunctionElementContext.alias != null)
                            n.Alias.SetValue(CreateAliasNode(selectFunctionElementContext.alias));

                        PrimarSqlTree.PutContextSpan(n, countFunctionCallContext);
                    });
                }

                throw TreeHelper.NotSupportedFeature("Select Element Function");
            }

            case SelectExpressionElementContext _:
                throw TreeHelper.NotSupportedFeature("Select Element Expression");

            default:
            {
                node = null;
                break;
            }
        }

        return node;
    }

    public static QsiColumnNode VisitSelectColumnElement(SelectColumnElementContext context)
    {
        var nameContext = context.fullColumnName();
        var column = IdentifierVisitor.VisitFullColumnName(nameContext);

        if (context.alias == null)
            return column;

        return TreeHelper.Create<QsiDerivedColumnNode>(n =>
        {
            n.Column.SetValue(column);
            n.Alias.SetValue(CreateAliasNode(context.alias));
        });
    }

    public static QsiTableNode VisitFromCluase(FromClauseContext context)
    {
        return VisitTableSource(context.tableSource());
    }

    public static QsiTableReferenceNode VisitTableName(TableNameContext context)
    {
        return TreeHelper.Create<QsiTableReferenceNode>(n =>
        {
            n.Identifier = IdentifierVisitor.VisitFullId(context.fullId());

            PrimarSqlTree.PutContextSpan(n, context);
        });
    }

    public static QsiTableNode VisitTableSource(TableSourceContext context)
    {
        switch (context)
        {
            case TableSourceBaseContext tableSourceBaseContext:
                return VisitTableSourceItem(tableSourceBaseContext.tableSourceItem());

            case TableSourceNestedContext tableSourceNestedContext:
                return VisitTableSourceItem(tableSourceNestedContext.tableSourceItem());
        }

        throw TreeHelper.NotSupportedTree(context);
    }

    public static QsiTableNode VisitTableSourceItem(TableSourceItemContext context)
    {
        return VisitTableName(context.tableName());
    }

    public static QsiWhereExpressionNode VisitWhereExpression(FromClauseContext context)
    {
        return TreeHelper.Create<QsiWhereExpressionNode>(n =>
        {
            n.Expression.SetValue(ExpressionVisitor.VisitExpression(context.whereExpr));
            PrimarSqlTree.PutContextSpan(n, context.whereKeyword, context.whereExpr.Stop);
        });
    }

    #region Alias
    public static QsiAliasNode CreateAliasNode(UidContext context)
    {
        var node = new QsiAliasNode
        {
            Name = IdentifierVisitor.VisitUid(context)
        };

        PrimarSqlTree.PutContextSpan(node, context);

        return node;
    }
    #endregion
}
using System;
using Qsi.Analyzers.Expression;
using Qsi.Analyzers.Expression.Models;
using Qsi.Analyzers.Table.Context;
using Qsi.MySql.Tree;
using Qsi.Tree;

namespace Qsi.MySql.Analyzers;

public class MySqlExpressionAnalyzer : QsiExpressionAnalyzer
{
    protected override QsiExpression ResolveSetColumnExpression(TableCompileContext context, IQsiSetColumnExpressionNode node)
    {
        return WithIndex(base.ResolveSetColumnExpression(context, node), node);
    }

    protected override QsiExpression ResolveColumnExpression(TableCompileContext context, IQsiColumnExpressionNode node)
    {
        return WithIndex(base.ResolveColumnExpression(context, node), node);
    }

    protected override QsiExpression ResolveColumn(TableCompileContext context, IQsiColumnNode node)
    {
        return WithIndex(base.ResolveColumn(context, node), node);
    }

    protected override QsiExpression ResolveColumnReference(TableCompileContext context, IQsiColumnReferenceNode node)
    {
        return WithIndex(base.ResolveColumnReference(context, node), node);
    }

    protected override QsiExpression ResolveExpressionFragment(TableCompileContext context, QsiExpressionFragmentNode node)
    {
        return WithIndex(base.ResolveExpressionFragment(context, node), node);
    }

    protected override QsiExpression ResolveLiteralExpression(TableCompileContext context, IQsiLiteralExpressionNode node)
    {
        return WithIndex(base.ResolveLiteralExpression(context, node), node);
    }

    protected override QsiExpression ResolveBinaryExpression(TableCompileContext context, IQsiBinaryExpressionNode node)
    {
        return WithIndex(base.ResolveBinaryExpression(context, node), node);
    }

    protected override TableExpression ResolveTableExpression(TableCompileContext context, IQsiTableExpressionNode node)
    {
        return WithIndex(base.ResolveTableExpression(context, node), node);
    }

    protected override DerivedExpression ResolveUnaryExpression(TableCompileContext context, IQsiUnaryExpressionNode node)
    {
        return WithIndex(base.ResolveUnaryExpression(context, node), node);
    }

    private static T WithIndex<T>(T expr, IQsiTreeNode node) where T : QsiExpression
    {
        var span = MySqlTree.Span[node];

        if (!Equals(span, default(Range)))
            expr.SetIndex(span.Start.Value, span.End.Value);

        return expr;
    }
}

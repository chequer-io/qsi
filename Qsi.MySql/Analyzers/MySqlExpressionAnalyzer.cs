using System;
using Qsi.Analyzers.Expression;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.MySql.Tree;
using Qsi.Tree;

namespace Qsi.MySql.Analyzers;

public class MySqlExpressionAnalyzer : QsiExpressionAnalyzer
{
    protected override IQsiExpression ResolveSetColumnExpression(TableCompileContext context, IQsiSetColumnExpressionNode node)
    {
        return WithIndex(base.ResolveSetColumnExpression(context, node), node);
    }

    protected override IQsiExpression ResolveSetVariableExpression(TableCompileContext context, IQsiSetVariableExpressionNode node)
    {
        return WithIndex(base.ResolveSetVariableExpression(context, node), node);
    }

    protected override IQsiExpression ResolveInvokeExpression(TableCompileContext context, IQsiInvokeExpressionNode node)
    {
        return WithIndex(base.ResolveInvokeExpression(context, node), node);
    }

    protected override IQsiExpression ResolveLiteralExpression(TableCompileContext context, IQsiLiteralExpressionNode node)
    {
        return WithIndex(base.ResolveLiteralExpression(context, node), node);
    }

    protected override IQsiExpression ResolveBinaryExpression(TableCompileContext context, IQsiBinaryExpressionNode node)
    {
        return WithIndex(base.ResolveBinaryExpression(context, node), node);
    }

    protected override IQsiExpression ResolveAllColumnExpression(TableCompileContext context, IQsiAllColumnNode node)
    {
        return WithIndex(base.ResolveAllColumnExpression(context, node), node);
    }

    protected override IQsiExpression ResolveColumnReferenceExpression(TableCompileContext context, IQsiColumnReferenceNode node)
    {
        return WithIndex(base.ResolveColumnReferenceExpression(context, node), node);
    }

    private static IQsiExpression WithIndex(IQsiExpression expr, IQsiTreeNode node)
    {
        var span = MySqlTree.Span[node];

        if (!Equals(span, default(Range)))
        {
            expr.StartIndex = span.Start.Value;
            expr.EndIndex = span.End.Value;
        }

        return expr;
    }
}

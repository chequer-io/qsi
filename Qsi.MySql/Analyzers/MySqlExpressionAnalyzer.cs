using System;
using Qsi.Analyzers.Expression;
using Qsi.Analyzers.Expression.Models;
using Qsi.Analyzers.Table.Context;
using Qsi.MySql.Tree;
using Qsi.Tree;

namespace Qsi.MySql.Analyzers;

public class MySqlExpressionAnalyzer : QsiExpressionAnalyzer
{
    protected override QsiExpression ResolveExpressionCore(TableCompileContext context, IQsiExpressionNode node)
    {
        if (ResolveInternal() is { } expr)
            return WithIndex(expr, node);

        return null;

        QsiExpression ResolveInternal()
        {
            switch (node)
            {
                case MySqlAliasedExpressionNode aliasedExpressionNode:
                    return ResolveAliasedExpression(context, aliasedExpressionNode);

                case MySqlCollationExpressionNode collationExpressionNode:
                    return ResolveCollationExpression(context, collationExpressionNode);

                default:
                    return base.ResolveExpressionCore(context, node);
            }
        }
    }

    private QsiExpression ResolveAliasedExpression(TableCompileContext context, MySqlAliasedExpressionNode node)
    {
        return new DerivedExpression(Resolve(context, node.Expression.Value));
    }

    private QsiExpression ResolveCollationExpression(TableCompileContext context, MySqlCollationExpressionNode node)
    {
        return new DerivedExpression(Resolve(context, node.Expression.Value));
    }

    private static T WithIndex<T>(T expr, IQsiTreeNode node) where T : QsiExpression
    {
        var span = MySqlTree.Span[node];

        if (!Equals(span, default(Range)))
            expr.SetIndex(span.Start.Value, span.End.Value);

        return expr;
    }
}

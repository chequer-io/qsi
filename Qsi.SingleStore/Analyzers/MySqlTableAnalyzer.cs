using System.Collections.Generic;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Engines;
using Qsi.SingleStore.Tree;
using Qsi.Tree;

namespace Qsi.SingleStore.Analyzers;

public class SingleStoreTableAnalyzer : QsiTableAnalyzer
{
    public SingleStoreTableAnalyzer(QsiEngine engine) : base(engine)
    {
    }

    protected override IEnumerable<QsiTableColumn> ResolveColumnsInExpression(TableCompileContext context, IQsiExpressionNode expression)
    {
        switch (expression)
        {
            case SingleStoreAliasedExpressionNode aliasedExpressionNode:
                return ResolveColumnsInExpression(context, aliasedExpressionNode.Expression.Value);

            case SingleStoreCollationExpressionNode collationExpressionNode:
                return ResolveColumnsInExpression(context, collationExpressionNode.Expression.Value);
        }

        return base.ResolveColumnsInExpression(context, expression);
    }
}

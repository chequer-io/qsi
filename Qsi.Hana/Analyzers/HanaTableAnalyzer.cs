using System.Collections.Generic;
using System.Linq;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Hana.Tree;
using Qsi.Tree;

namespace Qsi.Hana.Analyzers
{
    public class HanaTableAnalyzer : QsiTableAnalyzer
    {
        public HanaTableAnalyzer(QsiEngine engine) : base(engine)
        {
        }

        protected override IEnumerable<QsiTableColumn> ResolveColumnsInExpression(TableCompileContext context, IQsiExpressionNode expression)
        {
            switch (expression)
            {
                case HanaOrderByExpressionNode:
                case HanaCollateExpressionNode:
                case HanaLambdaExpressionNode:
                    foreach (var column in expression.Children
                        .OfType<IQsiExpressionNode>()
                        .SelectMany(x => ResolveColumnsInExpression(context, x)))
                    {
                        yield return column;
                    }

                    break;
            }

            foreach (var column in base.ResolveColumnsInExpression(context, expression))
            {
                yield return column;
            }
        }
    }
}

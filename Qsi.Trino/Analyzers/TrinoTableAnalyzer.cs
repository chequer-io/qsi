using System.Collections.Generic;
using System.Linq;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Engines;
using Qsi.Tree;
using Qsi.Trino.Tree;

namespace Qsi.Trino.Analyzers
{
    public sealed class TrinoTableAnalyzer : QsiTableAnalyzer
    {
        public TrinoTableAnalyzer(QsiEngine engine) : base(engine)
        {
        }

        protected override IEnumerable<QsiTableColumn> ResolveColumnsInExpression(TableCompileContext context, IQsiExpressionNode expression)
        {
            switch (expression)
            {
                case TrinoExistsExpressionNode:
                    break;

                case TrinoInvokeExpressionNode e:
                {
                    foreach (var column in base.ResolveColumnsInExpression(context, expression))
                        yield return column;

                    foreach (var column in ResolveColumnsInExpression(context, e.OrderBy.Value))
                        yield return column;

                    foreach (var column in ResolveColumnsInExpression(context, e.Filter.Value))
                        yield return column;

                    foreach (var column in ResolveColumnsInExpression(context, e.Over.Value))
                        yield return column;

                    break;
                }

                case TrinoWindowExpressionNode e:
                {
                    foreach (var item in e.Items)
                    {
                        foreach (var column in item.Children
                            .OfType<IQsiExpressionNode>()
                            .SelectMany(x => ResolveColumnsInExpression(context, x)))
                        {
                            yield return column;
                        }
                    }

                    break;
                }
                
                case TrinoIntervalExpressionNode:
                case TrinoLambdaExpressionNode:
                case TrinoMeasureExpressionNode:
                case TrinoSubscriptExpressionNode:
                {
                    foreach (var column in expression.Children
                        .OfType<IQsiExpressionNode>()
                        .SelectMany(x => ResolveColumnsInExpression(context, x)))
                    {
                        yield return column;
                    }

                    break;
                }

                default:
                    foreach (var c in base.ResolveColumnsInExpression(context, expression))
                        yield return c;

                    break;
            }
        }
    }
}

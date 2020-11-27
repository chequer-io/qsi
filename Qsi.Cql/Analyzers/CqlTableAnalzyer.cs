using System.Collections.Generic;
using System.Linq;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Cql.Tree;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Cql.Analyzers
{
    public class CqlTableAnalzyer : QsiTableAnalyzer
    {
        public CqlTableAnalzyer(QsiEngine engine) : base(engine)
        {
        }

        protected override IEnumerable<QsiTableColumn> ResolveColumnsInExpression(TableCompileContext context, IQsiExpressionNode expression)
        {
            switch (expression)
            {
                case CqlIndexExpressionNode _:
                case CqlMultipleUsingExpressionNode _:
                case CqlUsingExpressionNode _:
                    break;

                case CqlIndexerExpressionNode _:
                case CqlRangeExpressionNode _:
                case CqlSetColumnExpressionNode _:
                {
                    return expression.Children
                        .Cast<IQsiExpressionNode>()
                        .SelectMany(n => ResolveColumnsInExpression(context, n));
                }
            }

            return base.ResolveColumnsInExpression(context, expression);
        }
    }
}

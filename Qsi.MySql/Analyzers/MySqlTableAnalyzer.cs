using System.Collections.Generic;
using Qsi.Analyzers.Expression;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Engines;
using Qsi.MySql.Tree;
using Qsi.Tree;

namespace Qsi.MySql.Analyzers
{
    public class MySqlTableAnalyzer : QsiTableAnalyzer
    {
        public override QsiExpressionAnalyzer ExpressionAnalyzer => new MySqlExpressionAnalyzer();

        public MySqlTableAnalyzer(QsiEngine engine) : base(engine)
        {
        }

        protected override IEnumerable<QsiTableColumn> ResolveColumnsInExpression(TableCompileContext context, IQsiExpressionNode expression)
        {
            switch (expression)
            {
                case MySqlAliasedExpressionNode aliasedExpressionNode:
                    return ResolveColumnsInExpression(context, aliasedExpressionNode.Expression.Value);

                case MySqlCollationExpressionNode collationExpressionNode:
                    return ResolveColumnsInExpression(context, collationExpressionNode.Expression.Value);
            }

            return base.ResolveColumnsInExpression(context, expression);
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
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

        protected override async ValueTask<QsiTableStructure> BuildTableReferenceStructure(TableCompileContext context, IQsiTableReferenceNode table)
        {
            var structure = await base.BuildTableReferenceStructure(context, table);

            if (table is MySqlExplicitTableReferenceNode)
                return structure.CloneVisibleOnly();

            return structure;
        }
    }
}

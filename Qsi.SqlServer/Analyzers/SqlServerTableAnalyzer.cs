using System.Collections.Generic;
using System.Linq;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Engines;
using Qsi.SqlServer.Tree;
using Qsi.Tree;

namespace Qsi.SqlServer.Analyzers;

public class SqlServerTableAnalyzer : QsiTableAnalyzer
{
    public SqlServerTableAnalyzer(QsiEngine engine) : base(engine)
    {
    }

    protected override IEnumerable<QsiTableColumn> ResolveColumnsInExpression(TableCompileContext context, IQsiExpressionNode expression)
    {
        switch (expression)
        {
            case SqlServerPhyslocExpressionNode:
                return Enumerable.Empty<QsiTableColumn>();
        }

        return base.ResolveColumnsInExpression(context, expression);
    }
}

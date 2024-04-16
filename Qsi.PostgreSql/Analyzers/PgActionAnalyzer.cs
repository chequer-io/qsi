using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Qsi.Analyzers.Action;
using Qsi.Analyzers.Action.Context;
using Qsi.Analyzers.Action.Models;
using Qsi.Analyzers.Context;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Engines;
using Qsi.PostgreSql.Tree.Nodes;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PostgreSql.Analyzers;

public class PgActionAnalyzer : QsiActionAnalyzer
{
    public PgActionAnalyzer(QsiEngine engine) : base(engine)
    {
    }

    protected override async ValueTask<ColumnTarget[]> ResolveColumnTargetsFromDataInsertActionAsync(IAnalyzerContext context, QsiTableStructure table, IQsiDataInsertActionNode action)
    {
        if (action.ValueTable is PgDerivedTableNode pgDerivedTableNode)
        {
            var tableAnalyzer = context.Engine.GetAnalyzer<QsiTableAnalyzer>();
            using var tableContext = new TableCompileContext(context);
            var valueTable = (await tableAnalyzer.BuildTableStructure(tableContext, pgDerivedTableNode)).CloneVisibleOnly();
            return ResolveColumnTargetsFromTable(context, table, valueTable);
        }

        return await base.ResolveColumnTargetsFromDataInsertActionAsync(context, table, action);
    }

    protected ColumnTarget[] ResolveColumnTargetsFromTable(IAnalyzerContext context, QsiTableStructure table, QsiTableStructure valueTable)
    {
        return ResolveColumnTargetsFromTable(context, table)
            .Take(valueTable.Columns.Count)
            .ToArray();
    }
}

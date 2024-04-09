using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Qsi.Analyzers.Action;
using Qsi.Analyzers.Action.Context;
using Qsi.Analyzers.Action.Models;
using Qsi.Analyzers.Context;
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

    protected override ColumnTarget[] ResolveColumnTargetsFromDataInsertAction(IAnalyzerContext context, QsiTableStructure table, IQsiDataInsertActionNode action)
    {
        if (!ListUtility.IsNullOrEmpty(action.Columns) || !ListUtility.IsNullOrEmpty(action.SetValues))
            return base.ResolveColumnTargetsFromDataInsertAction(context, table, action);

        return ResolveColumnTargetsFromTable(context, table, (PgDerivedTableNode)action.ValueTable);
    }

    protected ColumnTarget[] ResolveColumnTargetsFromTable(IAnalyzerContext context, QsiTableStructure table, PgDerivedTableNode valueTable)
    {
        return ResolveColumnTargetsFromTable(context, table)
            .Take(valueTable.Columns.Value.Count)
            .ToArray();
    }
}

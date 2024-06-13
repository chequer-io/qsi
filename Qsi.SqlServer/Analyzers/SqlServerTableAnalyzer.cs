using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Data.Object;
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

    protected override Task<QsiTableStructure> BuildTableFunctionStructure(TableCompileContext context, IQsiTableFunctionNode table)
    {
        context.ThrowIfCancellationRequested();

        var identifier = table.Member.Identifier;
        var function = LookupFunctions(context, identifier).FirstOrDefault();

        if (function is null)
            throw new QsiException(QsiError.UnableResolveFunction, identifier);

        var structure = new QsiTableStructure
        {
            Identifier = identifier,
            Type = QsiTableType.Inline
        };

        foreach (var outParams in function.OutParameters)
        {
            var column = structure.NewColumn();
            column.Name = new QsiIdentifier(outParams.Name, false);
        }

        return Task.FromResult(structure);
    }
}

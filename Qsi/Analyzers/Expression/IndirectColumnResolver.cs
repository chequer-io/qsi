using System.Collections.Generic;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Services;
using Qsi.Tree;

namespace Qsi.Analyzers.Expression;

public sealed class IndirectColumnResolver : ColumnResolver
{
    public IndirectColumnResolver(IQsiLanguageService languageService, BuildTableStructureDelegate buildTableStructureDelegate)
        : base(languageService, buildTableStructureDelegate)
    {
    }

    protected override IEnumerable<QsiTableColumn> ResolveTableExpression(TableCompileContext context, IQsiTableExpressionNode expression)
    {
        using var scopedContext = new TableCompileContext(context);
        var structure = _buildTableStructureDelegate(scopedContext, expression.Table).Result;

        foreach (var c in structure.Columns)
            yield return c;

        if (structure.IndirectColumns is null)
            yield break;

        foreach (var c in structure.IndirectColumns)
            yield return c;
    }
}

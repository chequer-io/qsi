using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qsi.Analyzers;
using Qsi.Analyzers.Context;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Engines;
using Qsi.Extensions;
using Qsi.PrimarSql.Data;
using Qsi.PrimarSql.Tree;
using Qsi.Shared.Extensions;
using Qsi.Tree;

namespace Qsi.PrimarSql.Analyzers;

public class PrimarSqlTableAnalyzer : QsiTableAnalyzer
{
    public PrimarSqlTableAnalyzer(QsiEngine engine) : base(engine)
    {
    }

    protected override async ValueTask<IQsiAnalysisResult[]> OnExecute(IAnalyzerContext context)
    {
        var identifierSet = new HashSet<QsiIdentifier>(IdentifierComparer);
        QsiQualifiedIdentifier tableIdentifier = null;
        QsiTableNode node = (context.Tree as QsiDerivedTableNode);
        bool isCountFunction = false;

        while (node is QsiDerivedTableNode tableNode)
        {
            IEnumerable<QsiQualifiedIdentifier> columns = tableNode
                .Columns.Value
                .FindAscendants<QsiColumnReferenceNode>()
                .Select(c => c.Name);

            var hasCountFunction = tableNode
                .Columns.Value
                .FindAscendants<QsiDerivedColumnNode>()
                .Any(c => c.Expression.Value is QsiInvokeExpressionNode invokeExpressionNode &&
                          invokeExpressionNode.Member.Value.Identifier[^1].Value.EqualsIgnoreCase("COUNT"));

            if (hasCountFunction)
                isCountFunction = true;

            foreach (var column in columns)
            {
                identifierSet.Add(column[^1]);
            }

            if (tableNode.Source.Value is QsiTableReferenceNode tableReferenceNode)
                tableIdentifier = tableReferenceNode.Identifier;

            node = tableNode.Source.Value;
        }

        if (identifierSet.Count == 0 && tableIdentifier != null)
        {
            var jsonTable = new QsiTableStructure
            {
                Identifier = tableIdentifier,
                Type = QsiTableType.Table,
                IsSystem = false
            };

            if (!isCountFunction)
            {
                var documentColumn = jsonTable.NewColumn();
                documentColumn.Name = new QsiIdentifier("Document", false);
            }

            return new PrimarSqlJsonTableResult(jsonTable).ToSingleArray();
        }

        return await base.OnExecute(context);
    }

    protected override QsiTableColumn[] ResolveColumnReference(TableCompileContext context, IQsiColumnReferenceNode column, out QsiQualifiedIdentifier implicitTableWildcardTarget)
    {
        context.ThrowIfCancellationRequested();
        implicitTableWildcardTarget = default;

        if (column is not PrimarSqlColumnReferenceNode columnReferenceNode)
            throw new ArgumentException(nameof(column));

        var source = context.SourceTable;
        var columnName = columnReferenceNode.Name[^1];
        object[] path = columnReferenceNode.Accessors.Select(AccessorToValue).ToArray();

        if (source?.Type != QsiTableType.Table)
            throw new QsiException(QsiError.UnknownColumn, columnName);

        PrimarSqlTableColumn[] columns = source
            .Columns
            .Where(c => c is PrimarSqlTableColumn)
            .Cast<PrimarSqlTableColumn>()
            .Where(c => Match(c.Name, columnName) && c.Path.SequenceEqual(path))
            .ToArray();

        if (columns.Length == 0)
        {
            var newColumn = new PrimarSqlTableColumn
            {
                Name = columnName,
                Path = path
            };

            source.Columns.Add(newColumn);
            return new QsiTableColumn[] { newColumn };
        }

        return new QsiTableColumn[] { columns[0] };
    }

    protected override IEnumerable<QsiTableColumn> ResolveColumnsInExpression(TableCompileContext context, IQsiExpressionNode expression)
    {
        switch (expression)
        {
            case PrimarSqlSetColumnExpressionNode _:
            case PrimarSqlIndexerExpressionNode _:
            case PrimarSqlAppendArrayExpressionNode _:
                return expression.Children
                    .Cast<IQsiExpressionNode>()
                    .SelectMany(n => ResolveColumnsInExpression(context, n));
        }

        return base.ResolveColumnsInExpression(context, expression);
    }

    private object AccessorToValue(QsiExpressionNode accessor)
    {
        switch (accessor)
        {
            case PrimarSqlIndexerExpressionNode indexerExpressionNode:
            {
                if (indexerExpressionNode.Indexer.Value is not QsiLiteralExpressionNode literalExpressionNode)
                    throw new ArgumentException(nameof(accessor));

                return (long)Convert.ChangeType(literalExpressionNode.Value, TypeCode.Int64)!;
            }

            case QsiFieldExpressionNode fieldExpressionNode:
            {
                return fieldExpressionNode.Identifier.ToString();
            }
        }

        throw new ArgumentException(nameof(accessor));
    }
}
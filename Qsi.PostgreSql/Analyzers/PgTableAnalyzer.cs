using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PgQuery;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Data.Object;
using Qsi.Data.Object.Function;
using Qsi.Engines;
using Qsi.PostgreSql.Extensions;
using Qsi.PostgreSql.Tree.Nodes;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PostgreSql.Analyzers;

public class PgTableAnalyzer : QsiTableAnalyzer
{
    public PgTableAnalyzer(QsiEngine engine) : base(engine)
    {
    }

    public override async ValueTask<QsiTableStructure> BuildTableStructure(TableCompileContext context, IQsiTableNode table)
    {
        context.ThrowIfCancellationRequested();

        switch (table)
        {
            case PgXmlTableNode xmlTable:
                return await BuildXmlTableStructure(context, xmlTable);

            default:
                return await base.BuildTableStructure(context, table);
        }
    }

    protected Task<QsiTableStructure> BuildXmlTableStructure(TableCompileContext context, PgXmlTableNode node)
    {
        context.ThrowIfCancellationRequested();

        var structure = new QsiTableStructure
        {
            Identifier = new QsiQualifiedIdentifier(new QsiIdentifier("xmltable", false)),
            Type = QsiTableType.Inline,
        };

        foreach (var colNode in node.Columns.Value.OfType<PgXmlColumnNode>())
        {
            var column = structure.NewColumn();
            column.Name = colNode.Name;
        }

        return Task.FromResult(structure);
    }

    protected override async Task<QsiTableStructure> BuildTableFunctionStructure(TableCompileContext context, IQsiTableFunctionNode table)
    {
        context.ThrowIfCancellationRequested();

        if (table is not PgTableFunctionNode pgTable)
            return await base.BuildTableFunctionStructure(context, table);

        switch (pgTable.Function)
        {
            case PgSqlValueInvokeExpressionNode { IsBuiltIn: true } sqlValue:
            {
                var structure = new QsiTableStructure
                {
                    Identifier = sqlValue.Member.Value.Identifier,
                    Type = QsiTableType.Inline,
                    // IsSystem = true // NOTE: Is it System Table?
                };

                var column = structure.NewColumn();
                column.Name = structure.Identifier[^1];

                return structure;
            }

            case PgInvokeExpressionNode invoke:
            {
                var identifier = invoke.Member.Value.Identifier;

                QsiFunctionObject[] functions = LookupFunctions(context, identifier);

                if (functions is { Length: 0 })
                    throw new QsiException(QsiError.UnableResolveFunction, identifier);

                foreach (var func in functions)
                {
                    var node = context.Engine.TreeParser.Parse(new QsiScript(func.Definition, QsiScriptType.Create));

                    if (node is not PgFunctionDefinitionNode funcDef)
                        throw TreeHelper.NotSupportedTree(node);

                    var structure = new QsiTableStructure
                    {
                        Identifier = identifier,
                        Type = QsiTableType.Inline,
                        // IsSystem = true // NOTE: Is it System Table?
                    };

                    if (!funcDef.ReturnType.Value.Setof &&
                        (invoke.Parameters.Count < func.ArgumentsCount - func.DefaultArgumentsCount ||
                         invoke.Parameters.Count > func.ArgumentsCount))
                    {
                        continue;
                    }

                    #region Special Functions
                    // pg_catalog.unnest: same return count with parameter count
                    if (funcDef.Name[^2].Value.EqualsIgnoreCase("pg_catalog") &&
                        funcDef.Name[^1].Value.EqualsIgnoreCase("unnest"))
                    {
                        foreach (var _ in invoke.Parameters.WhereNotNull())
                        {
                            var column = structure.NewColumn();
                            column.Name = new QsiIdentifier($"{identifier[^1]}.unnest", false);
                        }

                        return structure;
                    }
                    #endregion

                    var outParams = funcDef.Parameters.WhereNotNull().Where(p => p.Mode is FunctionParameterMode.FuncParamOut);

                    int count = 0;

                    foreach (var outParam in outParams)
                    {
                        count++;

                        var column = structure.NewColumn();
                        column.Name = outParam.Name;
                    }

                    var tableParams = funcDef.Parameters.WhereNotNull().Where(p => p.Mode is FunctionParameterMode.FuncParamTable);

                    foreach (var tableParam in tableParams)
                    {
                        count++;

                        var column = structure.NewColumn();
                        column.Name = tableParam.Name;
                    }

                    if (count == 0)
                    {
                        var returnType = funcDef.ReturnType.Value;

                        // NOTE: If SETOF enabled, QSI doesn't know about return column counts.
                        if (returnType.Setof)
                        {
                            // If System Type, like int2, int4, int8.. it will returns one column. 
                            if (returnType.Identifier.Level is 2 &&
                                returnType.Identifier[0].Value.EqualsIgnoreCase("pg_catalog"))
                            {
                                var typeName = returnType.Identifier[1].Value;

                                if (typeName is
                                    "int2" or "int4" or "int8" or
                                    "float4" or "float8" or "numeric" or
                                    "bool" or "varbit" or "bit" or
                                    "timestamptz" or "timestamp" or "timetz" or "time" or "interval")
                                {
                                    structure.NewColumn();
                                }
                            }
                        }
                        else
                        {
                            structure.NewColumn();
                        }
                    }

                    return structure;
                }

                break;
            }
        }

        return await base.BuildTableFunctionStructure(context, table);
    }

    private QsiFunctionObject[] LookupFunctions(TableCompileContext context, QsiQualifiedIdentifier identifier)
    {
        var provider = context.Engine.RepositoryProvider;
        var funcIdentifier = ResolveQualifiedIdentifier(context, identifier);
        var func = provider.LookupObject(funcIdentifier, QsiObjectType.Function);

        // Check System Functions
        if (func is QsiFunctionList { Functions.Length: 0 } && identifier.Level == 1)
        {
            var fallBackIdentifier = new QsiQualifiedIdentifier(
                funcIdentifier[0],
                new QsiIdentifier("pg_catalog", false),
                identifier[0]
            );

            func = provider.LookupObject(fallBackIdentifier, QsiObjectType.Function);
        }

        switch (func)
        {
            case QsiFunctionList funcList:
                return funcList.Functions;

            case QsiFunctionObject funcObj:
                return new[] { funcObj };

            default:
                return Array.Empty<QsiFunctionObject>();
        }
    }

    protected override async ValueTask<QsiTableStructure> BuildCompositeTableStructure(TableCompileContext context, IQsiCompositeTableNode table)
    {
        context.ThrowIfCancellationRequested();

        var structure = await base.BuildCompositeTableStructure(context, table);

        if (table is PgRoutineTableNode { Ordinality: true })
        {
            var ordinality = structure.NewColumn();
            ordinality.Name = new QsiIdentifier("ordinality", false);
        }

        return structure;
    }

    protected override IEnumerable<QsiTableColumn> ResolveColumnsInExpression(TableCompileContext context, IQsiExpressionNode expression)
    {
        context.ThrowIfCancellationRequested();

        switch (expression)
        {
            case PgInvokeExpressionNode e:
            {
                foreach (var c in ResolveColumnsInPgInvokeExpression(context, e))
                    yield return c;

                break;
            }

            case PgBooleanTestExpressionNode e:
            {
                if (e.Target.IsEmpty)
                    break;

                foreach (var c in ResolveColumnsInExpression(context, e.Target.Value))
                    yield return c;

                break;
            }

            case PgCastExpressionNode e:
            {
                if (!e.Source.IsEmpty)
                {
                    foreach (var c in ResolveColumnsInExpression(context, e.Source.Value))
                        yield return c;
                }

                if (!e.Type.IsEmpty)
                {
                    foreach (var c in ResolveColumnsInExpression(context, e.Type.Value))
                        yield return c;
                }

                break;
            }

            case PgCollateExpressionNode e:
            {
                if (!e.Expression.IsEmpty)
                {
                    foreach (var c in ResolveColumnsInExpression(context, e.Expression.Value))
                        yield return c;
                }

                break;
            }

            case PgGroupingSetExpressionNode e:
            {
                foreach (var c in e.Expressions.SelectMany(x => ResolveColumnsInExpression(context, x!)))
                    yield return c;

                break;
            }

            case PgIndexExpressionNode e:
            {
                if (!e.Index.IsEmpty)
                {
                    foreach (var c in ResolveColumnsInExpression(context, e.Index.Value))
                        yield return c;
                }

                if (!e.IndexEnd.IsEmpty)
                {
                    foreach (var c in ResolveColumnsInExpression(context, e.IndexEnd.Value))
                        yield return c;
                }

                break;
            }

            case PgIndirectionExpressionNode e:
            {
                foreach (var c in e.Indirections.SelectMany(x => ResolveColumnsInExpression(context, x)))
                    yield return c;

                break;
            }

            case PgNamedParameterExpressionNode e:
            {
                if (!e.Expression.IsEmpty)
                {
                    foreach (var c in ResolveColumnsInExpression(context, e.Expression.Value))
                        yield return c;
                }

                break;
            }

            case PgNullTestExpressionNode e:
            {
                if (!e.Target.IsEmpty)
                {
                    foreach (var c in ResolveColumnsInExpression(context, e.Target.Value))
                        yield return c;
                }

                break;
            }

            case PgSubLinkExpressionNode e:
            {
                if (!e.Expression.IsEmpty)
                {
                    foreach (var c in ResolveColumnsInExpression(context, e.Expression.Value))
                        yield return c;
                }

                break;
            }

            case PgWindowDefExpressionNode e:
            {
                foreach (var c in e.PartitionClause.SelectMany(x => ResolveColumnsInExpression(context, x!)))
                    yield return c;

                foreach (var c in e.OrderClause.SelectMany(x => ResolveColumnsInExpression(context, x!)))
                    yield return c;

                if (!e.StartOffset.IsEmpty)
                {
                    foreach (var c in ResolveColumnsInExpression(context, e.StartOffset.Value))
                        yield return c;
                }

                if (!e.EndOffset.IsEmpty)
                {
                    foreach (var c in ResolveColumnsInExpression(context, e.EndOffset.Value))
                        yield return c;
                }

                break;
            }

            case PgDefaultExpressionNode:
            case PgDefinitionElementNode:
                break;

            case PgInferExpressionNode e:
            {
                if (!e.Where.IsEmpty)
                    foreach (var c in ResolveColumnsInExpression(context, e.Where.Value))
                        yield return c;

                foreach (var c in e.IndexElems.SelectMany(x => ResolveColumnsInExpression(context, x!)))
                    yield return c;

                break;
            }

            case QsiRowValueExpressionNode e:
            {
                foreach (var c in e.ColumnValues.SelectMany(x => ResolveColumnsInExpression(context, x!)))
                    yield return c;

                break;
            }

            default:
                foreach (var qsiTableColumn in base.ResolveColumnsInExpression(context, expression))
                    yield return qsiTableColumn;

                break;
        }
    }

    protected IEnumerable<QsiTableColumn> ResolveColumnsInPgInvokeExpression(TableCompileContext context, PgInvokeExpressionNode expression)
    {
        var identifier = expression.Member.Value.Identifier;

        // NOTE: COUNT(~~) ignore columns
        if (identifier[0].Value.EqualsIgnoreCase("COUNT"))
            return Enumerable.Empty<QsiTableColumn>();

        if (identifier.Level is 1 && expression.Parameters.Count is 1 &&
            expression.Parameters[0] is QsiColumnExpressionNode { Column.Value: QsiColumnReferenceNode columnNode })
            return ResolveColumnsInColumnAsTableInvokeExpression(context, identifier[0], columnNode);

        return base.ResolveColumnsInExpression(context, expression);
    }

    // check func_name(table_name) or column_name(table_name)
    protected IEnumerable<QsiTableColumn> ResolveColumnsInColumnAsTableInvokeExpression(
        TableCompileContext context,
        QsiIdentifier identifier,
        QsiColumnReferenceNode columnNode)
    {
        var columnAsTable = context.GetAllSourceTables()
            .FirstOrDefault(t =>
                t.HasIdentifier &&
                IdentifierComparer.Equals(t.Identifier[^1], columnNode.Name[0])
            );

        // NOTE: column_name(table_name) is same table_name.column_name
        // https://www.postgresql.org/docs/current/sql-expressions.html#SQL-EXPRESSIONS-FUNCTION-CALLS
        // 4.2.6. Function Calls > Note
        if (columnAsTable?.Columns.Any(c => IdentifierComparer.Equals(c.Name, identifier)) ?? false)
            throw TreeHelper.NotSupportedFeature("Field selection written in functional style");

        try
        {
            return ResolveColumnReference(context, columnNode, out _);
        }
        catch (QsiException ex) when (ex.Error is QsiError.UnknownColumnIn)
        {
            if (columnAsTable is not { })
                throw;

            // func_name(table_name)
            return columnAsTable.Columns.ToArray();
        }
    }
}

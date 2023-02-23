using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Data.Object;
using Qsi.Data.Object.Function;
using Qsi.Engines;
using Qsi.PostgreSql.Tree.Nodes;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PostgreSql.Analyzers
{
    public class PgTableAnalyzer : QsiTableAnalyzer
    {
        public PgTableAnalyzer(QsiEngine engine) : base(engine)
        {
        }

        protected override async Task<QsiTableStructure> BuildTableFunctionStructure(TableCompileContext context, IQsiTableFunctionNode table)
        {
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
                    var provider = context.Engine.RepositoryProvider;
                    var identifier = invoke.Member.Value.Identifier;

                    var func = provider.LookupObject(ResolveQualifiedIdentifier(context, identifier), QsiObjectType.Function);

                    // Check System Functions
                    if (func is not { } && identifier.Level == 1)
                        func = provider.LookupObject(
                            new QsiQualifiedIdentifier(
                                new QsiIdentifier("pg_catalog", false),
                                identifier[0]
                            ),
                            QsiObjectType.Function
                        );

                    if (func is not QsiFunctionObject funcObj)
                        throw new QsiException(QsiError.UnableResolveFunction, identifier);

                    var node = context.Engine.TreeParser.Parse(new QsiScript(funcObj.Definition, QsiScriptType.Create));

                    if (node is not PgFunctionDefinitionNode funcDef)
                        throw TreeHelper.NotSupportedTree(node);

                    // TODO: Analyze columns from function definition
                    break;
                }
            }

            return await base.BuildTableFunctionStructure(context, table);
        }

        protected override async ValueTask<QsiTableStructure> BuildCompositeTableStructure(TableCompileContext context, IQsiCompositeTableNode table)
        {
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
            switch (expression)
            {
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
                {
                    break;
                }

                case PgDefinitionElementNode e:
                {
                    if (!e.Expression.IsEmpty)
                    {
                        foreach (var c in ResolveColumnsInExpression(context, e.Expression.Value))
                            yield return c;
                    }

                    break;
                }

                case PgInferExpressionNode e:
                {
                    if (!e.Where.IsEmpty)
                        foreach (var c in ResolveColumnsInExpression(context, e.Where.Value))
                            yield return c;

                    foreach (var c in e.IndexElems.SelectMany(x => ResolveColumnsInExpression(context, x!)))
                        yield return c;

                    break;
                }

                case PgRowValueExpressionNode e:
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

        protected override QsiTableColumn[] ResolveColumnReference(TableCompileContext context, IQsiColumnReferenceNode column, out QsiQualifiedIdentifier implicitTableWildcardTarget)
        {
            try
            {
                return base.ResolveColumnReference(context, column, out implicitTableWildcardTarget);
            }
            catch (QsiException e)
            {
                if (e.Error is QsiError.UnknownColumnIn)
                {
                    if (context.SourceTable?.Columns.Count == 0)
                    {
                        implicitTableWildcardTarget = default;

                        return new[]
                        {
                            new QsiTableColumn
                            {
                                Name = column.Name[^1],
                                IsVisible = true,
                                Parent = context.SourceTable
                            }
                        };
                    }

                    var table = context.JoinedSouceTables.FirstOrDefault(s => s.Columns.Count == 0);

                    if (table is not null)
                    {
                        implicitTableWildcardTarget = default;

                        return new[]
                        {
                            new QsiTableColumn
                            {
                                Name = column.Name[^1],
                                IsVisible = true,
                                Parent = table
                            }
                        };
                    }
                }

                throw;
            }
        }
    }
}

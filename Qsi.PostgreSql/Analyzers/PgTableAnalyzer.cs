using System.Collections.Generic;
using System.Linq;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Engines;
using Qsi.PostgreSql.NewTree.Nodes;
using Qsi.Tree;

namespace Qsi.PostgreSql.Analyzers
{
    public class PgTableAnalyzer : QsiTableAnalyzer
    {
        public PgTableAnalyzer(QsiEngine engine) : base(engine)
        {
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

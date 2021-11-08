using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Data.Object;
using Qsi.Engines;
using Qsi.Oracle.Internal;
using Qsi.Oracle.Tree;
using Qsi.Tree;

namespace Qsi.Oracle.Analyzers
{
    public sealed class OracleTableAnalyzer : QsiTableAnalyzer
    {
        // TODO: Build Sequence: => seq.NEXTVAL, seq.CURRVAL

        public OracleTableAnalyzer(QsiEngine engine) : base(engine)
        {
        }

        protected override ValueTask<QsiTableStructure> BuildDerivedTableStructure(TableCompileContext context, IQsiDerivedTableNode table)
        {
            if (table.Source == null)
                throw new QsiException(QsiError.NoFromClause);

            return base.BuildDerivedTableStructure(context, table);
        }

        protected override IEnumerable<QsiTableColumn> ResolveColumnsInExpression(TableCompileContext context, IQsiExpressionNode expression)
        {
            switch (expression)
            {
                case OracleColumnOuterJoinExpressionNode e:
                {
                    yield return ResolveColumnReference(context, e.Column.Value);

                    break;
                }

                case OracleMiningAttributeExpressionNode e:
                {
                    if (e.Columns.IsEmpty)
                        yield break;

                    foreach (var c in e.Columns.Value.SelectMany(x => ResolveColumns(context, x)))
                        yield return c;

                    break;
                }

                case OracleTypeCastFunctionExpressionNode e:
                {
                    if (!e.DefaultExpressionOnError.IsEmpty)
                        foreach (var c in ResolveColumnsInExpression(context, e.DefaultExpressionOnError.Value))
                            yield return c;

                    // QsiInvokeExpressionNode
                    foreach (var c in base.ResolveColumnsInExpression(context, e))
                        yield return c;

                    break;
                }

                case OracleWindowExpressionNode e:
                {
                    foreach (var column in e.Items
                        .SelectMany(x => x.Children
                            .OfType<IQsiExpressionNode>()
                            .SelectMany(y => ResolveColumnsInExpression(context, y))))
                    {
                        yield return column;
                    }

                    break;
                }

                case OracleWindowingExpressionNode e:
                {
                    foreach (var column in e.Items
                        .SelectMany(x => x.Children
                            .OfType<IQsiExpressionNode>()
                            .SelectMany(y => ResolveColumnsInExpression(context, y))))
                    {
                        yield return column;
                    }

                    if (!e.Exclude.IsEmpty)
                        foreach (var c in base.ResolveColumnsInExpression(context, e.Exclude.Value))
                            yield return c;

                    break;
                }

                case OracleAggregateFunctionExpressionNode:
                case OracleAnalyticFunctionExpressionNode:
                case OracleCostMatrixExpressionNode:
                case OracleDatetimeExpressionNode:
                case OracleIntervalExpressionNode:
                case OracleLimitExpressionNode:
                case OracleNamedParameterExpressionNode:
                case OracleJsonKeepOperationExpressionNode:
                case OracleXmlAttributesExpressionNode:
                case OracleXmlFunctionExpressionNode:
                case OracleXmlColumnAttributeItemNode:
                {
                    foreach (var column in expression.Children
                        .OfType<IQsiExpressionNode>()
                        .SelectMany(x => ResolveColumnsInExpression(context, x)))
                    {
                        yield return column;
                    }

                    break;
                }

                case OracleJsonColumnExpressionNode e:
                    foreach (var c in e.Columns.SelectMany(x => ResolveColumns(context, x)))
                        yield return c;

                    break;

                case OracleXmlColumnDefinitionNode e:
                    foreach (var c in ResolveColumns(context, e.Column.Value))
                        yield return c;

                    break;

                default:
                    foreach (var c in base.ResolveColumnsInExpression(context, expression))
                        yield return c;

                    break;
            }
        }

        protected override QsiTableColumn ResolveColumnReference(TableCompileContext context, IQsiColumnReferenceNode column)
        {
            try
            {
                return base.ResolveColumnReference(context, column);
            }
            catch (QsiException e) when (e.Error is QsiError.UnknownColumn or QsiError.UnknownColumnIn)
            {
                if (OraclePseudoColumn.TryGetColumn(column.Name[0].Value, out var tableColumn))
                    return tableColumn;

                // TODO: Work
                // if (ResolveColumnFromObject(context, context))

                throw;
            }
        }

        private QsiTableColumn ResolveColumnFromObject(TableCompileContext context, QsiQualifiedIdentifier identifier)
        {
            var lastName = identifier[^1];

            if (lastName.Value != "CURRVAL" && lastName.Value != "NEXTVAL")
                return null;

            var qsiObject = LookupObject(context, new QsiQualifiedIdentifier(identifier[..^1]), QsiObjectType.Sequence);

            if (qsiObject is null)
                return null;

            // TODO: Add Parent
            return new QsiTableColumn
            {
                Name = qsiObject.Identifier[^1],
                ObjectReferences = { qsiObject }
            };
        }

        private QsiObject LookupObject(TableCompileContext context, QsiQualifiedIdentifier identifier, QsiObjectType type)
        {
            return context.Engine.RepositoryProvider.LookupObject(ResolveQualifiedIdentifier(context, identifier), type);
        }

        protected override IEnumerable<QsiTableColumn> ResolveDerivedColumns(TableCompileContext context, IQsiDerivedColumnNode column)
        {
            if (column is OracleJsonColumnNode { NestedColumn: { } } node)
                return ResolveColumnsInExpression(context, node.NestedColumn.Value);

            return base.ResolveDerivedColumns(context, column);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Extensions;
using Qsi.Hana.Tree;
using Qsi.Tree;
using Qsi.Tree.Immutable;

namespace Qsi.Hana.Analyzers
{
    public class HanaTableAnalyzer : QsiTableAnalyzer
    {
        public HanaTableAnalyzer(QsiEngine engine) : base(engine)
        {
        }

        public override ValueTask<QsiTableStructure> BuildTableStructure(TableCompileContext context, IQsiTableNode table)
        {
            switch (table)
            {
                case HanaCaseJoinTableNode caseJoinTableNode:
                    return BuildHanaCaseJoinTableStructure(context, caseJoinTableNode);

                case HanaCaseJoinItemTableNode caseJoinItemTableNode:
                    return BuildHanaCaseJoinItemTableStructure(context, caseJoinItemTableNode);

                case HanaXmlTableNode xmlTableNode:
                    return BuildHanaXmlTableStructure(context, xmlTableNode);

                case HanaJsonTableNode jsonTableNode:
                    return BuildHanaJsonTableNode(context, jsonTableNode);

                default:
                    return base.BuildTableStructure(context, table);
            }
        }

        private async ValueTask<QsiTableStructure> BuildHanaCaseJoinTableStructure(TableCompileContext context, HanaCaseJoinTableNode table)
        {
            HanaCaseJoinItemTableNode[] items = table.Children.OfType<HanaCaseJoinItemTableNode>().ToArray();
            var sources = new QsiTableStructure[items.Length + 1];

            sources[0] = await BuildTableStructure(context, table.Source.Value);

            for (int i = 0; i < items.Length; i++)
            {
                using var tempContext = new TableCompileContext(context);
                sources[i + 1] = await BuildTableStructure(tempContext, items[i]);
            }

            int columnCount = sources[0].Columns.Count;

            if (sources.Skip(1).Any(s => s.Columns.Count != columnCount))
                throw new QsiException(QsiError.DifferentColumnsCount);

            var structure = new QsiTableStructure
            {
                Type = QsiTableType.Union
            };

            for (int i = 0; i < columnCount; i++)
            {
                var baseColumn = sources[0].Columns[i];
                var column = structure.NewColumn();

                column.Name = baseColumn.Name;
                column.References.AddRange(sources.Select(s => s.Columns[i]));
            }

            return structure;
        }

        private ValueTask<QsiTableStructure> BuildHanaCaseJoinItemTableStructure(TableCompileContext context, HanaCaseJoinItemTableNode table)
        {
            var derivedNode = new ImmutableDerivedTableNode(
                table.Parent,
                null,
                table.Columns.Value,
                table.Source.Value,
                null, null, null, null, null, null
            );

            return BuildDerivedTableStructure(context, derivedNode);
        }

        private async ValueTask<QsiTableStructure> BuildHanaXmlTableStructure(TableCompileContext context, HanaXmlTableNode table)
        {
            var structure = new QsiTableStructure
            {
                Type = QsiTableType.Inline
            };

            if (table.Identifier != null)
                structure.Identifier = new QsiQualifiedIdentifier(table.Identifier);

            foreach (var xmlColumn in table.Columns)
            {
                structure.Columns.Add(new QsiTableColumn
                {
                    Name = xmlColumn.Identifier
                });
            }

            var argColumnRef = table.ArgumentColumnReference;

            if (argColumnRef != null)
            {
                var refStructure = await LookupTableColumnReference(context, argColumnRef);

                foreach (var column in structure.Columns)
                    column.References.AddRange(refStructure.Columns);
            }

            return structure;
        }

        private async ValueTask<QsiTableStructure> BuildHanaJsonTableNode(TableCompileContext context, HanaJsonTableNode table)
        {
            var structure = new QsiTableStructure
            {
                Type = QsiTableType.Inline
            };

            if (table.Identifier != null)
                structure.Identifier = new QsiQualifiedIdentifier(table.Identifier);

            foreach (var namedColumnDef in table.FindAscendants<IHanaJsonNamedColumnDefinitionNode>())
            {
                structure.Columns.Add(new QsiTableColumn
                {
                    Name = namedColumnDef.Identifier
                });
            }

            var argColumnRef = table.ArgumentColumnReference;

            if (argColumnRef != null)
            {
                var refStructure = await LookupTableColumnReference(context, argColumnRef);

                foreach (var column in structure.Columns)
                    column.References.AddRange(refStructure.Columns);
            }

            return structure;
        }

        private async ValueTask<QsiTableStructure> LookupTableColumnReference(TableCompileContext context, QsiQualifiedIdentifier columnRef)
        {
            if (columnRef.Level < 2)
                throw new QsiException(QsiError.NoTablesUsed);

            var refTableNode = new HanaDerivedTableNode
            {
                Columns =
                {
                    Value = new QsiColumnsDeclarationNode
                    {
                        Columns =
                        {
                            new QsiColumnReferenceNode
                            {
                                Name = columnRef.SubIdentifier(^1)
                            }
                        }
                    }
                },
                Source =
                {
                    Value = new HanaTableReferenceNode
                    {
                        Identifier = columnRef.SubIdentifier(..^1)
                    }
                }
            };

            using var refContext = new TableCompileContext(context);
            return await BuildTableStructure(refContext, refTableNode);
        }

        protected override IEnumerable<QsiTableColumn> ResolveColumnsInExpression(TableCompileContext context, IQsiExpressionNode expression)
        {
            switch (expression)
            {
                case HanaOrderByExpressionNode:
                case HanaCollateExpressionNode:
                case HanaLambdaExpressionNode:
                    foreach (var column in expression.Children
                        .OfType<IQsiExpressionNode>()
                        .SelectMany(x => ResolveColumnsInExpression(context, x)))
                    {
                        yield return column;
                    }

                    break;
            }

            foreach (var column in base.ResolveColumnsInExpression(context, expression))
            {
                yield return column;
            }
        }
    }
}

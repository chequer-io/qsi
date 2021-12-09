using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qsi.Analyzers;
using Qsi.Analyzers.Action;
using Qsi.Analyzers.Context;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Cql.Analyzers.Selection;
using Qsi.Cql.Tree;
using Qsi.Data;
using Qsi.Engines;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Cql.Analyzers
{
    public class CqlActionAnalyzer : QsiActionAnalyzer
    {
        public CqlActionAnalyzer(QsiEngine engine) : base(engine)
        {
        }

        private IEnumerable<ColumnPlan> CompileColumnPlan(IAnalyzerContext context, QsiTableStructure table, IQsiColumnsDeclarationNode columns)
        {
            if (columns.Columns.All(c => c is IQsiAllColumnNode))
                return table.Columns.Select(c => new ColumnPlan(c.Name));

            var plans = new Dictionary<QsiIdentifier, ColumnPlan>(IdentifierComparer);

            foreach (var column in columns)
            {
                QsiIdentifier identifier;
                ISelector[] selectors = null;

                switch (column)
                {
                    case IQsiColumnReferenceNode columnReference:
                    {
                        identifier = columnReference.Name[^1];
                        break;
                    }

                    case IQsiDerivedColumnNode { IsExpression: true, Expression: IQsiMemberAccessExpressionNode memberAccess }:
                    {
                        var selectorList = new List<ISelector>();

                        do
                        {
                            switch (memberAccess.Member)
                            {
                                case IQsiFieldExpressionNode fieldExpression:
                                {
                                    selectorList.Add(new FieldSelector(fieldExpression.Identifier[^1].Value));
                                    break;
                                }

                                case CqlIndexerExpressionNode indexerExpression:
                                {
                                    var indexerNode = indexerExpression.Indexer.Value;

                                    switch (indexerNode)
                                    {
                                        case IQsiLiteralExpressionNode literalExpression:
                                        {
                                            var index = (int)Convert.ChangeType(literalExpression.Value, TypeCode.Int32)!;
                                            selectorList.Add(new ElementSelector(index));
                                            break;
                                        }

                                        case CqlRangeExpressionNode rangeExpression:
                                        {
                                            var startNode = rangeExpression.Start.Value;
                                            var endNode = rangeExpression.End.Value;

                                            if (startNode is null or IQsiLiteralExpressionNode &&
                                                endNode is null or IQsiLiteralExpressionNode)
                                            {
                                                var start = startNode == null ?
                                                    Index.Start :
                                                    (int)(long)((IQsiLiteralExpressionNode)startNode).Value;

                                                var end = endNode == null ?
                                                    Index.End :
                                                    (int)(long)((IQsiLiteralExpressionNode)endNode).Value;

                                                selectorList.Add(new RangeSelector(start..end));
                                            }
                                            else
                                            {
                                                var builder = new StringBuilder();
                                                builder.Append("(MAP<TEXT, INT>){");

                                                if (startNode != null)
                                                {
                                                    builder.Append("'s':");
                                                    builder.Append(context.Engine.TreeDeparser.Deparse(startNode, context.Script));
                                                }

                                                if (endNode != null)
                                                {
                                                    if (startNode != null)
                                                        builder.Append(',');

                                                    builder.Append("'e':");
                                                    builder.Append(context.Engine.TreeDeparser.Deparse(endNode, context.Script));
                                                }

                                                builder.Append('}');

                                                selectorList.Add(new PendingSelector(builder.ToString(), typeof(RangeSelector)));
                                            }

                                            break;
                                        }

                                        default:
                                        {
                                            var sql = context.Engine.TreeDeparser.Deparse(indexerNode, context.Script);
                                            selectorList.Add(new PendingSelector(sql, typeof(ElementSelector)));
                                            break;
                                        }
                                    }

                                    break;
                                }

                                default:
                                    throw new QsiException(QsiError.Syntax);
                            }

                            if (memberAccess.Target is IQsiMemberAccessExpressionNode prevMemberAccess)
                            {
                                memberAccess = prevMemberAccess;
                            }
                            else
                            {
                                break;
                            }
                        } while (true);

                        while (memberAccess.Target is IQsiMemberAccessExpressionNode prevMemberAccess)
                            memberAccess = prevMemberAccess;

                        var columnExpression = (IQsiColumnExpressionNode)memberAccess.Target;
                        var columnReference = (IQsiColumnReferenceNode)columnExpression.Column;

                        selectorList.Reverse();

                        identifier = columnReference.Name[^1];
                        selectors = selectorList.ToArray();

                        break;
                    }

                    default:
                        throw new QsiException(QsiError.Syntax);
                }

                if (!plans.TryGetValue(identifier, out var columnPlan))
                {
                    columnPlan = new ColumnPlan(identifier);
                    plans[identifier] = columnPlan;
                }

                if (selectors != null)
                    columnPlan.Selectors.Add(selectors);
            }

            return plans.Values.ToArray();
        }

        protected override async ValueTask<IQsiAnalysisResult[]> ExecuteDataDeleteAction(IAnalyzerContext context, IQsiDataDeleteActionNode action)
        {
            var tableNode = (CqlDerivedTableNode)action.Target;
            var tableReferenceNode = (QsiTableReferenceNode)tableNode.Source.Value;

            var tableAnalyzer = context.Engine.GetAnalyzer<QsiTableAnalyzer>();
            QsiTableStructure table;

            using (var tableContext = new TableCompileContext(context))
                table = await tableAnalyzer.BuildTableStructure(tableContext, tableReferenceNode);

            ColumnPlan[] columnPlans = CompileColumnPlan(context, table, tableNode.Columns.Value).ToArray();
            await ResolveColumnPlan(context, columnPlans, tableReferenceNode);

            var targetNode = (CqlDerivedTableNode)ReassembleCommonTableNode(tableNode);
            targetNode.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());

            var partialDelete =
                columnPlans.Any(p => p.Selectors.Count > 0) ||
                table.Columns.Count != columnPlans.Length;

            var dataTable = await GetDataTableByCommonTableNode(context, targetNode);

            return ResolveDataManipulationTargets(
                    context,
                    table,
                    columnPlans.Select(c => new QsiQualifiedIdentifier(c.Name)))
                .Select(target =>
                {
                    foreach (var row in dataTable.Rows)
                    {
                        if (!partialDelete)
                        {
                            var targetRow = new QsiDataRow(target.DeleteRows.ColumnCount);

                            foreach (var pivot in target.ColumnPivots)
                                targetRow.Items[pivot.TargetOrder] = row.Items[pivot.TargetOrder];
                            
                            target.DeleteRows.Add(targetRow);

                            continue;
                        }

                        var beforeRow = new QsiDataRow(target.UpdateBeforeRows.ColumnCount);
                        var afterRow = new QsiDataRow(target.UpdateAfterRows.ColumnCount);

                        foreach (var pivot in target.ColumnPivots)
                        {
                            var value = row.Items[pivot.TargetOrder];

                            beforeRow.Items[pivot.TargetOrder] = value;
                            ref var afterValue = ref afterRow.Items[pivot.TargetOrder];

                            if (value.Type == QsiDataType.Null)
                            {
                                afterValue = value;
                                continue;
                            }

                            if (pivot.DeclaredColumn != null)
                            {
                                var plan = columnPlans[pivot.DeclaredOrder];

                                if (plan.Selectors.Count == 0)
                                {
                                    afterValue = QsiDataValue.Default;
                                    continue;
                                }

                                var strValue = (string)value.Value;

                                if (string.IsNullOrWhiteSpace(strValue))
                                {
                                    afterValue = value;
                                    continue;
                                }

                                var jsonValue = JToken.Parse(strValue);

                                foreach (ISelector[] selectors in plan.Selectors)
                                {
                                    var targetValue = jsonValue;

                                    foreach (var selector in selectors)
                                    {
                                        var selectedValue = selector.Run(jsonValue);

                                        if (selectedValue == null)
                                        {
                                            targetValue = null;
                                            break;
                                        }

                                        targetValue = selectedValue;
                                    }

                                    targetValue?.Remove();
                                }

                                afterValue = jsonValue.Any() ?
                                    new QsiDataValue(Serialize(jsonValue), value.Type) :
                                    QsiDataValue.Null;
                            }
                            else
                            {
                                afterValue = value;
                            }
                        }
                        
                        target.UpdateBeforeRows.Add(beforeRow);
                        target.UpdateAfterRows.Add(afterRow);
                    }

                    return new QsiDataManipulationResult
                    {
                        Table = target.Table,
                        AffectedColumns = GetAffectedColumns(target),
                        DeleteRows = target.DeleteRows.ToNullIfEmpty(),
                        UpdateBeforeRows = target.UpdateBeforeRows.ToNullIfEmpty(),
                        UpdateAfterRows = target.UpdateAfterRows.ToNullIfEmpty()
                    };
                })
                .ToArray<IQsiAnalysisResult>();
        }

        private async Task ResolveColumnPlan(IAnalyzerContext context, IEnumerable<ColumnPlan> plans, IQsiTableReferenceNode tableReference)
        {
            PendingSelector[] pendingSelectors = plans
                .SelectMany(p => p.Selectors.SelectMany(s => s).OfType<PendingSelector>())
                .ToArray();

            if (pendingSelectors.Length == 0)
                return;

            var builder = new StringBuilder();

            builder.Append("SELECT ");
            builder.AppendJoin(", ", pendingSelectors.Select((s, i) => $"{s.Sql} AS c{i}"));
            builder.Append($" FROM {tableReference.Identifier} LIMIT 1 ALLOW FILTERING");

            var sql = builder.ToString();
            var script = new QsiScript(sql, QsiScriptType.Select);

            // TODO: Bind parameter in selector
            var table = await context.Engine.RepositoryProvider.GetDataTable(script, null, context.CancellationToken);
            QsiDataValue[] values = table.Rows[0].Items;

            for (int i = 0; i < values.Length; i++)
                pendingSelectors[i].Resolve(values[i].Value);
        }

        protected override IQsiTableNode ReassembleCommonTableNode(IQsiTableNode node)
        {
            if (node is CqlDerivedTableNode cqlNode)
            {
                var ctn = new CqlDerivedTableNode
                {
                    Parent = cqlNode.Parent,
                    IsJson = cqlNode.IsJson,
                    IsDistinct = cqlNode.IsDistinct,
                    AllowFiltering = cqlNode.AllowFiltering
                };

                if (!cqlNode.Columns.IsEmpty)
                    ctn.Columns.SetValue(cqlNode.Columns.Value);

                if (!cqlNode.Source.IsEmpty)
                    ctn.Source.SetValue(cqlNode.Source.Value);

                if (!cqlNode.Where.IsEmpty)
                    ctn.Where.SetValue(cqlNode.Where.Value);

                if (!cqlNode.Grouping.IsEmpty)
                    ctn.Grouping.SetValue(cqlNode.Grouping.Value);

                if (!cqlNode.Order.IsEmpty)
                    ctn.Order.SetValue(cqlNode.Order.Value);

                if (!cqlNode.Limit.IsEmpty)
                    ctn.Limit.SetValue(cqlNode.Limit.Value);

                if (!cqlNode.PerPartitionLimit.IsEmpty)
                    ctn.PerPartitionLimit.SetValue(cqlNode.PerPartitionLimit.Value);

                return ctn;
            }

            return base.ReassembleCommonTableNode(node);
        }

        private static string Serialize(JToken value)
        {
            var buffer = new StringBuilder();
            using var writer = new StringWriter(buffer);
            using var jsonWriter = new JsonTextWriter(writer) { QuoteChar = '\'' };

            var serializer = JsonSerializer.CreateDefault();
            serializer.Serialize(jsonWriter, value);

            return buffer.ToString();
        }

        private class ColumnPlan
        {
            public QsiIdentifier Name { get; }

            public List<ISelector[]> Selectors { get; }

            public ColumnPlan(QsiIdentifier name)
            {
                Name = name;
                Selectors = new List<ISelector[]>();
            }
        }
    }
}

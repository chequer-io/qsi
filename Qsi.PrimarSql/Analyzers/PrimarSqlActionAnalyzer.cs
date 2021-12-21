using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qsi.Analyzers;
using Qsi.Analyzers.Action;
using Qsi.Analyzers.Action.Models;
using Qsi.Analyzers.Context;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Engines;
using Qsi.PrimarSql.Tree;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PrimarSql.Analyzers
{
    public class PrimarSqlActionAnalyzer : QsiActionAnalyzer
    {
        public PrimarSqlActionAnalyzer(QsiEngine engine) : base(engine)
        {
        }

        #region Delete
        protected override async ValueTask<IQsiAnalysisResult[]> ExecuteDataDeleteAction(IAnalyzerContext context, IQsiDataDeleteActionNode action)
        {
            var tableAnalyzer = context.Engine.GetAnalyzer<QsiTableAnalyzer>();
            using var tableContext = new TableCompileContext(context);

            var table = (await tableAnalyzer.BuildTableStructure(tableContext, action.Target)).References[0];
            var tempTable = CreateTemporaryTable(table.Identifier);

            var commonTableNode = ReassembleCommonTableNode(action.Target);
            var dataTable = await GetDataTableByCommonTableNode(context, commonTableNode);

            var deleteRows = new QsiDataRowCollection(1, context.Engine.CacheProviderFactory());

            foreach (var row in dataTable.Rows)
            {
                var targetRow = new QsiDataRow(deleteRows.ColumnCount)
                {
                    Items =
                    {
                        [0] = row.Items[0]
                    }
                };

                deleteRows.Add(targetRow);
            }

            return new QsiDataManipulationResult
            {
                Table = tempTable,
                AffectedColumns = tempTable.Columns.ToArray(),
                DeleteRows = deleteRows.ToNullIfEmpty()
            }.ToSingleArray();
        }
        #endregion

        #region Update
        #endregion

        protected override async ValueTask<IQsiAnalysisResult[]> ExecuteDataUpdateAction(IAnalyzerContext context, IQsiDataUpdateActionNode action)
        {
            var tableAnalyzer = context.Engine.GetAnalyzer<QsiTableAnalyzer>();
            using var tableContext = new TableCompileContext(context);

            var table = (await tableAnalyzer.BuildTableStructure(tableContext, action.Target)).References[0];
            var tempTable = CreateTemporaryTable(table.Identifier);

            var commonTableNode = ReassembleCommonTableNode(action.Target);
            var dataTable = await GetDataTableByCommonTableNode(context, commonTableNode);

            var updateBeforeRows = new QsiDataRowCollection(1, context.Engine.CacheProviderFactory());
            var updateAfterRows = new QsiDataRowCollection(1, context.Engine.CacheProviderFactory());

            (object[], QsiExpressionNode, bool)[] setValues = action.SetValues
                .OfType<PrimarSqlSetColumnExpressionNode>()
                .Select(x => (
                    x.Accessors.Select(AccessorToValue).Prepend(x.Target[^1].Value).ToArray(),
                    x.Value.IsEmpty ? null : x.Value.Value,
                    x.Value.IsEmpty
                ))
                .ToArray();

            if (setValues.Select(x => x.Item1).Distinct(new EnumerableComparer()).Count() != setValues.Length)
            {
                throw new InvalidOperationException("one or more document path overlapped.");
            }

            foreach (var row in dataTable.Rows)
            {
                var oldRow = new QsiDataRow(updateBeforeRows.ColumnCount);
                var newRow = new QsiDataRow(updateAfterRows.ColumnCount);

                var beforeValue = row.Items[0];
                var afterValue = JObject.Parse(beforeValue.Value.ToString() ?? throw new InvalidOperationException());

                foreach ((object[] part, var value, bool deleteProperty) in setValues)
                {
                    if (!SetValueToToken(afterValue, part, ConvertToToken(value, context), deleteProperty))
                    {
                        if (!deleteProperty)
                        {
                            throw new InvalidOperationException("Invalid path for update value.");
                        }
                    }
                }

                oldRow.Items[0] = beforeValue;
                newRow.Items[0] = new QsiDataValue(afterValue.ToString(Formatting.None), QsiDataType.Object);
                
                updateBeforeRows.Add(oldRow);
                updateAfterRows.Add(newRow);
            }

            var tempTable2 = new QsiTableStructure
            {
                Type = QsiTableType.Derived,
                References = { tempTable }
            };

            foreach (var parts in setValues.Select(v => v.Item1))
            {
                var column = tempTable2.NewColumn();
                column.Name = new QsiIdentifier(FormatParts(parts), false);
                column.References.AddRange(tempTable.Columns);
            }

            return new QsiDataManipulationResult
            {
                Table = tempTable2,
                AffectedColumns = tempTable2.Columns.ToArray(),
                UpdateBeforeRows = updateBeforeRows.ToNullIfEmpty(),
                UpdateAfterRows = updateAfterRows.ToNullIfEmpty()
            }.ToSingleArray();
        }

        #region Insert
        protected override async ValueTask<IQsiAnalysisResult[]> ExecuteDataInsertAction(IAnalyzerContext context, IQsiDataInsertActionNode action)
        {
            var tableAnalyzer = context.Engine.GetAnalyzer<QsiTableAnalyzer>();
            using var tableContext = new TableCompileContext(context);

            var table = await tableAnalyzer.BuildTableStructure(tableContext, action.Target);
            var tempTable = CreateTemporaryTable(table.Identifier);

            var insertRows = new QsiDataRowCollection(1, context.Engine.CacheProviderFactory());

            ColumnTarget[] columnTargets = ResolveColumnTargetsFromDataInsertAction(context, table, action);

            foreach (var value in action.Values)
            {
                if (columnTargets.Length != value.ColumnValues.Length)
                    throw new QsiException(QsiError.DifferentColumnsCount);

                var obj = new JObject();

                for (int i = 0; i < columnTargets.Length; i++)
                {
                    var column = columnTargets[i].DeclaredName[^1];
                    var columnValue = value.ColumnValues[i];

                    obj[IdentifierUtility.Unescape(column.Value)] = ConvertToToken(columnValue, context);
                }

                var row = new QsiDataRow(insertRows.ColumnCount)
                {
                    Items =
                    {
                        [0] = new QsiDataValue(obj.ToString(Formatting.None), QsiDataType.Object)
                    }
                };

                insertRows.Add(row);
            }

            var tempTable2 = new QsiTableStructure
            {
                Type = QsiTableType.Derived,
                References = { tempTable }
            };

            foreach (var column in columnTargets)
            {
                var newColumn = tempTable2.NewColumn();
                newColumn.Name = column.DeclaredName[^1];
                newColumn.References.AddRange(tempTable.Columns);
            }

            return new QsiDataManipulationResult
            {
                Table = tempTable2,
                AffectedColumns = tempTable2.Columns.ToArray(),
                InsertRows = insertRows.ToNullIfEmpty(),
            }.ToSingleArray();
        }
        #endregion

        protected override IQsiTableNode ReassembleCommonTableNode(IQsiTableNode node)
        {
            if (node is PrimarSqlDerivedTableNode primarSqlNode)
            {
                var ctn = new PrimarSqlDerivedTableNode
                {
                    Parent = primarSqlNode.Parent,
                    SelectSpec = primarSqlNode.SelectSpec
                };

                if (!primarSqlNode.Columns.IsEmpty)
                    ctn.Columns.SetValue(primarSqlNode.Columns.Value);

                if (!primarSqlNode.Source.IsEmpty)
                    ctn.Source.SetValue(primarSqlNode.Source.Value);

                if (!primarSqlNode.Where.IsEmpty)
                    ctn.Where.SetValue(primarSqlNode.Where.Value);

                if (!primarSqlNode.Order.IsEmpty)
                    ctn.Order.SetValue(primarSqlNode.Order.Value);

                if (!primarSqlNode.Limit.IsEmpty)
                    ctn.Limit.SetValue(primarSqlNode.Limit.Value);

                if (!primarSqlNode.StartKey.IsEmpty)
                    ctn.StartKey.SetValue(primarSqlNode.StartKey.Value);

                return ctn;
            }

            return base.ReassembleCommonTableNode(node);
        }

        private string FormatParts(IEnumerable<object> parts)
        {
            var builder = new StringBuilder();

            foreach (var part in parts)
            {
                switch (part)
                {
                    case string fieldName:
                        if (builder.Length > 0)
                            builder.Append('.');

                        builder.Append(fieldName);
                        break;

                    case int indexer:
                        builder.Append('[').Append(indexer).Append(']');
                        break;

                    default:
                        throw new InvalidOperationException();
                }
            }

            return builder.ToString();
        }

        private bool SetValueToToken(JToken token, object[] parts, JToken value, bool deleteProperty)
        {
            var currentToken = token;

            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                bool isLastPart = i == parts.Length - 1;

                switch (part)
                {
                    case string s when currentToken is JObject jObject:
                        currentToken = jObject[s];

                        if (currentToken == null && isLastPart)
                        {
                            if (deleteProperty)
                                return jObject.Remove(s);

                            jObject[s] = value;
                            return true;
                        }

                        break;

                    case int index when currentToken is JArray jArray:
                        currentToken = jArray[index];

                        if (isLastPart)
                        {
                            if (deleteProperty)
                            {
                                try
                                {
                                    jArray.RemoveAt(index);
                                }
                                catch (Exception)
                                {
                                    // ignored
                                }
                            }
                            else
                            {
                                jArray[index] = value;
                            }

                            return true;
                        }

                        break;

                    default:
                        return false;
                }

                if (currentToken == null)
                    return false;
            }

            currentToken.Replace(value);
            return true;
        }

        private QsiTableStructure CreateTemporaryTable(QsiQualifiedIdentifier identifier)
        {
            var table = new QsiTableStructure
            {
                Identifier = identifier
            };

            var column = table.NewColumn();
            column.Name = new QsiIdentifier("Document", false);

            return table;
        }

        private JToken ConvertToToken(IQsiExpressionNode node, IAnalyzerContext context)
        {
            if (node == null)
                return null;

            if (node is QsiLiteralExpressionNode literalNode)
            {
                return literalNode.Type switch
                {
                    QsiDataType.Binary => new JValue((byte[])literalNode.Value),
                    QsiDataType.Boolean => new JValue((bool)literalNode.Value),
                    QsiDataType.Json => JObject.Parse(literalNode.Value.ToString() ?? throw new InvalidOperationException("literal value is null")),
                    QsiDataType.Numeric => new JValue(double.Parse(literalNode.Value.ToString() ?? "0")),
                    QsiDataType.Decimal => new JValue(double.Parse(literalNode.Value.ToString() ?? "0")),
                    QsiDataType.Null => null,
                    _ => literalNode.Value.ToString()
                };
            }

            return context.Engine.TreeDeparser.Deparse(node, context.Script);
        }

        private class EnumerableComparer : IEqualityComparer<IEnumerable<object>>
        {
            public bool Equals(IEnumerable<object> x, IEnumerable<object> y)
            {
                if (x == null && y == null)
                    return true;

                if (x == null || y == null)
                    return false;

                return x.SequenceEqual(y);
            }

            public int GetHashCode(IEnumerable<object> obj)
            {
                return 0;
            }
        }

        private object AccessorToValue(QsiExpressionNode accessor)
        {
            switch (accessor)
            {
                case PrimarSqlIndexerExpressionNode indexerExpressionNode:
                {
                    if (indexerExpressionNode.Indexer.Value is not QsiLiteralExpressionNode literalExpressionNode)
                        throw new ArgumentException(nameof(accessor));

                    return (int)Convert.ChangeType(literalExpressionNode.Value, TypeCode.Int32)!;
                }

                case QsiFieldExpressionNode fieldExpressionNode:
                {
                    return fieldExpressionNode.Identifier.ToString();
                }
            }

            throw new ArgumentException(nameof(accessor));
        }
    }
}

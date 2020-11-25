using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;
using static Qsi.Cql.Internal.CqlParserInternal;

namespace Qsi.Cql.Tree
{
    internal static class ActionVisitor
    {
        #region InsertStatement
        public static QsiActionNode VisitInsertStatement(InsertStatementContext context)
        {
            var node = new CqlDataInsertActionNode();
            UsingClauseContext usingClause;
            bool ifNotExists;

            node.Target.SetValue(TableVisitor.VisitColumnFamilyName(context.cf));

            if (context.st1 != null)
            {
                node.Columns = context.st1.cidentList().list
                    .Select(i => new QsiQualifiedIdentifier(i))
                    .ToArray();

                var rowNode = new QsiRowValueExpressionNode();
                rowNode.ColumnValues.AddRange(context.st1._values.Select(ExpressionVisitor.VisitTerm));
                // TODO: CqlTree.PutContextSpan(rowNode, context.st1...);

                node.Values.Add(rowNode);

                ifNotExists = context.st1.ifNotExists;
                usingClause = context.st1.usingClause();
            }
            else
            {
                var jsonValue = context.st2.jsonValue().s.raw;
                (QsiQualifiedIdentifier[] columns, var row) = JsonToRowValue(jsonValue);

                node.Columns = columns;
                node.Values.Add(row);
                node.DefaultValue = context.st2.defaultValue;

                ifNotExists = context.st2.ifNotExists;
                usingClause = context.st2.usingClause();
            }

            node.ConflictBehavior = ifNotExists ? QsiDataConflictBehavior.None : QsiDataConflictBehavior.Update;

            if (usingClause != null)
                node.Usings.SetValue(ExpressionVisitor.VisitUsingClause(context.st2.usingClause()));

            CqlTree.PutContextSpan(node, context);

            return node;
        }

        private static (QsiQualifiedIdentifier[] Columns, QsiRowValueExpressionNode Row) JsonToRowValue(string json)
        {
            var jsonObject = JObject.Parse(json);
            var columns = new List<QsiQualifiedIdentifier>();
            var row = new QsiRowValueExpressionNode();

            foreach (var property in jsonObject.Properties())
            {
                var column = new QsiIdentifier(property.Name, property.Name[0] == '"');
                var jValue = property.Value;

                object value;
                QsiDataType type;

                switch (jValue.Type)
                {
                    case JTokenType.Null:
                        value = null;
                        type = QsiDataType.Null;
                        break;

                    case JTokenType.Float:
                        value = jValue.Value<decimal>();
                        type = QsiDataType.Decimal;
                        break;

                    case JTokenType.Integer:
                        value = jValue.Value<int>();
                        type = QsiDataType.Numeric;
                        break;

                    case JTokenType.Boolean:
                        value = jValue.Value<bool>();
                        type = QsiDataType.Boolean;
                        break;

                    case JTokenType.String:
                        value = Escape(jValue.Value<string>());
                        type = QsiDataType.String;
                        break;

                    default:
                        value = Escape(jValue.ToString(Formatting.None));
                        type = QsiDataType.Json;
                        break;
                }

                columns.Add(new QsiQualifiedIdentifier(column));
                row.ColumnValues.Add(TreeHelper.CreateLiteral(value, type));
            }

            return (columns.ToArray(), row);

            static string Escape(string value)
            {
                return $"'{value.Replace("'", "''")}'";
            }
        }
        #endregion

        #region UpdateStatement
        public static QsiActionNode VisitUpdateStatement(UpdateStatementContext context)
        {
            throw new System.NotImplementedException();
        }
        #endregion

        #region DeleteStatement
        public static QsiActionNode VisitDeleteStatement(DeleteStatementContext context)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}

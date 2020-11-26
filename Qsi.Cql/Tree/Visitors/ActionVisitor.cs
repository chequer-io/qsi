using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qsi.Cql.Tree.Common;
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
            QsiTableNode tableNode = TableVisitor.VisitColumnFamilyName(context.cf);

            if (context.wclause != null)
            {
                var derivedTableNode = new QsiDerivedTableNode();

                var whereContext = new ParserRuleContextWrapper<WhereClauseContext>
                (
                    context.wclause,
                    context.w,
                    context.wclause.Stop
                );

                derivedTableNode.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
                derivedTableNode.Source.SetValue(tableNode);
                derivedTableNode.Where.SetValue(ExpressionVisitor.CreateWhere(whereContext));

                tableNode = derivedTableNode;
            }

            var node = new CqlDataUpdateActionNode();

            node.Target.SetValue(tableNode);

            if (context.usingClause() != null)
                node.Usings.SetValue(ExpressionVisitor.VisitUsingClause(context.usingClause()));

            node.SetValues.AddRange(context.columnOperation().Select(ExpressionVisitor.VisitColumnOperation));

            // TODO: need test 'IF EXISTS | IF <conditions>'

            CqlTree.PutContextSpan(node, context);

            return node;
        }
        #endregion

        #region DeleteStatement
        public static QsiActionNode VisitDeleteStatement(DeleteStatementContext context)
        {
            var node = new CqlDataDeleteActionNode();

            var tableNode = new CqlDerivedTableNode();
            tableNode.Columns.SetValue(VisitDeleteSelection(context.dels));
            tableNode.Source.SetValue(TableVisitor.VisitColumnFamilyName(context.cf));

            var whereContext = new ParserRuleContextWrapper<WhereClauseContext>
            (
                context.wclause,
                context.w,
                context.wclause.Stop
            );

            tableNode.Where.SetValue(ExpressionVisitor.CreateWhere(whereContext));

            // TODO: need test 'IF EXISTS | IF <conditions>'

            node.Target.SetValue(tableNode);

            if (context.usingClauseDelete() != null)
                node.Using.SetValue(ExpressionVisitor.VisitUsingClauseDelete(context.usingClauseDelete()));

            CqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiColumnsDeclarationNode VisitDeleteSelection(DeleteSelectionContext context)
        {
            var node = new QsiColumnsDeclarationNode();

            node.Columns.AddRange(context.deleteOp().Select(VisitDeleteOp));
            CqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiColumnNode VisitDeleteOp(DeleteOpContext context)
        {
            switch (context)
            {
                case DeleteSingleContext deleteSingle:
                    return VisitDeleteSingle(deleteSingle);

                case DeleteIndexContext deleteIndex:
                    return VisitDeleteIndex(deleteIndex);

                case DeleteFieldContext deleteField:
                    return VisitDeleteField(deleteField);

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
        }

        private static QsiDeclaredColumnNode VisitDeleteSingle(DeleteSingleContext context)
        {
            return TableVisitor.VisitCident(context.c);
        }

        private static QsiDerivedColumnNode VisitDeleteIndex(DeleteIndexContext context)
        {
            var accessTargetNode = new QsiColumnExpressionNode();
            var accessMemberNode = new CqlIndexerExpressionNode();

            accessTargetNode.Column.SetValue(TableVisitor.VisitCident(context.c));
            accessMemberNode.Indexer.SetValue(ExpressionVisitor.VisitTerm(context.term()));

            var accessNode = new QsiMemberAccessExpressionNode();

            accessNode.Target.SetValue(accessTargetNode);
            accessNode.Member.SetValue(accessMemberNode);

            var node = new QsiDerivedColumnNode();

            node.Expression.SetValue(accessNode);
            CqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiDerivedColumnNode VisitDeleteField(DeleteFieldContext context)
        {
            var accessTargetNode = new QsiColumnExpressionNode();
            var accessMemberNode = new QsiFieldExpressionNode();

            accessTargetNode.Column.SetValue(TableVisitor.VisitCident(context.c));
            accessMemberNode.Identifier = new QsiQualifiedIdentifier(context.field.id);

            var accessNode = new QsiMemberAccessExpressionNode();

            accessNode.Target.SetValue(accessTargetNode);
            accessNode.Member.SetValue(accessMemberNode);

            var node = new QsiDerivedColumnNode();

            node.Expression.SetValue(accessNode);
            CqlTree.PutContextSpan(node, context);

            return node;
        }
        #endregion
    }
}

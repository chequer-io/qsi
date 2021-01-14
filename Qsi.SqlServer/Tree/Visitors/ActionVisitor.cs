using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.SqlServer.Tree
{
    internal sealed class ActionVisitor : VisitorBase
    {
        public ActionVisitor(IVisitorContext visitorContext) : base(visitorContext)
        {
        }

        public QsiChangeSearchPathActionNode VisitUseStatement(UseStatement useStatement)
        {
            var node = new QsiChangeSearchPathActionNode
            {
                Identifiers = new[]
                {
                    IdentifierVisitor.CreateIdentifier(useStatement.DatabaseName),
                }
            };

            SqlServerTree.PutFragmentSpan(node, useStatement);

            return node;
        }

        #region Insert
        public QsiActionNode VisitInsertStatement(InsertStatement insertStatement)
        {
            return VisitInsertSpecificiation(insertStatement.InsertSpecification);
        }

        public QsiActionNode VisitInsertSpecificiation(InsertSpecification insertSpecification)
        {
            var node = new QsiDataInsertActionNode();
            var tableNode = TableVisitor.VisitTableReference(insertSpecification.Target);

            if (!(tableNode is QsiTableAccessNode tableAccessNode))
                throw new QsiException(QsiError.Syntax);

            node.Target.SetValue(tableAccessNode);

            if (!ListUtility.IsNullOrEmpty(insertSpecification.Columns))
            {
                node.Columns = insertSpecification.Columns
                    .Select(ExpressionVisitor.VisitColumnReferenceExpression)
                    .Select(c => c.Column.Value switch
                    {
                        QsiDeclaredColumnNode declaredColumnNode => declaredColumnNode.Name,
                        QsiAllColumnNode allColumnNode => allColumnNode.Path,
                        _ => throw new QsiException(QsiError.Syntax)
                    })
                    .ToArray();
            }

            switch (insertSpecification.InsertSource)
            {
                case ExecuteInsertSource _:
                    throw TreeHelper.NotSupportedFeature("Execute Insert");

                case SelectInsertSource selectInsertSource:
                {
                    node.ValueTable.SetValue(TableVisitor.VisitQueryExpression(selectInsertSource.Select));
                    break;
                }

                case ValuesInsertSource valuesInsertSource:
                {
                    node.Values.AddRange(valuesInsertSource.RowValues.Select(ExpressionVisitor.VisitRowValue));
                    break;
                }

                default:
                    throw TreeHelper.NotSupportedTree(insertSpecification.InsertSource);
            }

            SqlServerTree.PutFragmentSpan(node, insertSpecification);

            return node;
        }
        #endregion

        #region Delete
        public QsiActionNode VisitDeleteStatement(DeleteStatement deleteStatement)
        {
            return VisitDeleteSpecification(deleteStatement.DeleteSpecification);
        }

        public QsiActionNode VisitDeleteSpecification(DeleteSpecification deleteSpecification)
        {
            var table = TableVisitor.VisitTableReference(deleteSpecification.Target);
            var where = deleteSpecification.WhereClause;
            var topRowFilter = deleteSpecification.TopRowFilter;

            if (where != null || topRowFilter != null)
            {
                var derivedTable = new QsiDerivedTableNode();

                derivedTable.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
                derivedTable.Source.SetValue(table);

                if (where != null)
                {
                    derivedTable.Where.SetValue(TableVisitor.VisitWhereClause(where));
                }

                if (topRowFilter != null)
                {
                    derivedTable.Limit.SetValue(TableVisitor.VisitLimitOffset(topRowFilter, null));
                }

                table = derivedTable;
            }

            var node = new QsiDataDeleteActionNode();

            node.Target.SetValue(table);
            SqlServerTree.PutFragmentSpan(node, deleteSpecification);

            return node;
        }
        #endregion

        #region Update
        public QsiDataUpdateActionNode VisitUpdateStatement(UpdateStatement updateStatement)
        {
            return VisitUpdateSpecification(updateStatement.UpdateSpecification);
        }

        public QsiDataUpdateActionNode VisitUpdateSpecification(UpdateSpecification updateSpecification)
        {
            QsiTableNode table = TableVisitor.VisitTableReference(updateSpecification.Target);

            var where = updateSpecification.WhereClause;
            var topRowFilter = updateSpecification.TopRowFilter;

            if (where != null || topRowFilter != null)
            {
                var derivedTable = new QsiDerivedTableNode();

                derivedTable.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
                derivedTable.Source.SetValue(table);

                if (where != null)
                {
                    derivedTable.Where.SetValue(TableVisitor.VisitWhereClause(where));
                }

                if (topRowFilter != null)
                {
                    derivedTable.Limit.SetValue(TableVisitor.VisitLimitOffset(topRowFilter, null));
                }

                table = derivedTable;
            }

            var node = new QsiDataUpdateActionNode();

            node.Target.SetValue(table);
            node.SetValues.AddRange(updateSpecification.SetClauses.Select(ExpressionVisitor.VisitSetClause));

            SqlServerTree.PutFragmentSpan(node, updateSpecification);

            return node;
        }
        #endregion
    }
}

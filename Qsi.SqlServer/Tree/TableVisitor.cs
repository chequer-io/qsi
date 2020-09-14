using System;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Qsi.Tree.Base;
using Qsi.Utilities;

namespace Qsi.SqlServer.Tree
{
    public sealed class TableVisitor : VisitorBase
    {
        public TableVisitor(IContext context) : base(context)
        {
        }

        public QsiTreeNode Visit(TSqlBatch sqlBatch)
        {
            var statement = sqlBatch.Statements.FirstOrDefault();

            if (statement != null)
                return VisitStatements(statement);

            throw new NullReferenceException();
        }

        public QsiTreeNode VisitStatements(TSqlStatement statement)
        {
            switch (statement)
            {
                case StatementWithCtesAndXmlNamespaces statementWithCtesAndXmlNamespaces:
                    return VisitStatementWithCtesAndXmlNamespaces(statementWithCtesAndXmlNamespaces);
            }

            throw TreeHelper.NotSupportedTree(statement);
        }

        #region StatementWithCtesAndXmlNamespaces
        private QsiTreeNode VisitStatementWithCtesAndXmlNamespaces(StatementWithCtesAndXmlNamespaces statementWithCtesAndXmlNamespaces)
        {
            switch (statementWithCtesAndXmlNamespaces)
            {
                case SelectStatement selectStatement:
                    return VisitSelectStatement(selectStatement);

                case DataModificationStatement dataModificationStatement:
                    return VisitDataModificationStatement(dataModificationStatement);
            }

            throw TreeHelper.NotSupportedTree(statementWithCtesAndXmlNamespaces);
        }

        private QsiTableNode VisitSelectStatement(SelectStatement selectStatement)
        {
            return VisitQueryExpression(selectStatement.QueryExpression);
        }

        private QsiTableNode VisitDataModificationStatement(DataModificationStatement dataModificationStatement)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Query Expression
        private QsiTableNode VisitQueryExpression(QueryExpression queryExpression)
        {
            switch (queryExpression)
            {
                case BinaryQueryExpression binaryQueryExpression:
                    return null;

                case QueryParenthesisExpression queryParenthesisExpression:
                    return null;

                case QuerySpecification querySpecification:
                    return VisitQuerySpecification(querySpecification);
            }

            throw TreeHelper.NotSupportedTree(queryExpression);
        }

        private QsiTableNode VisitQuerySpecification(QuerySpecification querySpecification)
        {
            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                var columns = new QsiColumnsDeclarationNode();
                columns.Columns.AddRange(querySpecification.SelectElements.Select(VisitSelectElement));
                n.Columns.SetValue(columns);

                VisitFromClause(querySpecification.FromClause);
            });
        }
        #endregion

        #region SelectElement
        private QsiColumnNode VisitSelectElement(SelectElement selectElement)
        {
            switch (selectElement)
            {
                case SelectScalarExpression selectScalarExpression:
                    return VisitSelectScalarExpression(selectScalarExpression);

                // case SelectSetVariable selectSetVariable:
                //     return VisitSelectSetVariable(selectSetVariable);

                case SelectStarExpression selectStarExpression:
                    return VisitSelectStarExpression(selectStarExpression);
            }

            throw TreeHelper.NotSupportedTree(selectElement);
        }

        private QsiColumnNode VisitSelectStarExpression(SelectStarExpression selectStarExpression)
        {
            return TreeHelper.Create<QsiAllColumnNode>(n =>
            {
                if (selectStarExpression.Qualifier != null)
                {
                    // n.Path = IdentifierVisitor.VisitMultipartIdentifier(selectStarExpression.Qualifier);
                }
            });
        }

        private QsiColumnNode VisitSelectScalarExpression(SelectScalarExpression selectScalarExpression)
        {
            QsiExpressionNode expression = null;
            QsiDeclaredColumnNode column = null;

            switch (selectScalarExpression.Expression)
            {
            }

            throw TreeHelper.NotSupportedTree(selectScalarExpression);
        }
        #endregion

        #region FromClause
        private QsiTableNode VisitFromClause(FromClause fromClause)
        {
            var tableReferences = fromClause.TableReferences;

            // if (tableReferences.Count == 1)
            // return

            var joinedTableNode = TreeHelper.Create<QsiJoinedTableNode>(n =>
            {
            })
        }
        #endregion

        #region TableReference
        private QsiTableNode VisitTableReference(TableReference tableReference)
        {
            switch (tableReference)
            {
                case JoinParenthesisTableReference joinParenthesisTableReference:
                    return VisitJoinParenthesisTableReference(joinParenthesisTableReference);

                case JoinTableReference joinTableReference:
                    return VisitJoinTableReference(joinTableReference);

                case OdbcQualifiedJoinTableReference odbcQualifiedJoinTableReference:
                    return VisitOdbcQualifiedJoinTableReference(odbcQualifiedJoinTableReference);

                case TableReferenceWithAlias tableReferenceWithAlias:
                    return VisitTableReferenceWithAlias(tableReferenceWithAlias);
            }
        }
        #endregion
    }
}

using System;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Qsi.Data;
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
            var statement = sqlBatch?.Statements?.FirstOrDefault();

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
        public QsiTableNode VisitQueryExpression(QueryExpression queryExpression)
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
                var columnsDeclarationNode = new QsiColumnsDeclarationNode();
                columnsDeclarationNode.Columns.AddRange(querySpecification.SelectElements.Select(VisitSelectElement));
                n.Columns.SetValue(columnsDeclarationNode);

                if (querySpecification.FromClause != null)
                {
                    n.Source.SetValue(VisitFromClause(querySpecification.FromClause));
                }
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
                    n.Path = IdentifierVisitor.CreateQualifiedIdentifier(selectStarExpression.Qualifier);
                }
            });
        }

        private QsiColumnNode VisitSelectScalarExpression(SelectScalarExpression selectScalarExpression)
        {
            QsiExpressionNode expression = null;
            QsiDeclaredColumnNode column = null;

            if (selectScalarExpression.Expression is ColumnReferenceExpression columnReferenceExpression)
            {
                column = new QsiDeclaredColumnNode
                {
                    Name = IdentifierVisitor.CreateQualifiedIdentifier(columnReferenceExpression.MultiPartIdentifier)
                };

                if (selectScalarExpression.ColumnName == null)
                    return column;
            }
            else
            {
                expression = ExpressionVisitor.VisitScalarExpression(selectScalarExpression.Expression);
            }

            return TreeHelper.Create<QsiDerivedColumnNode>(n =>
            {
                if (column != null)
                {
                    n.Column.SetValue(column);
                }
                else if (expression != null)
                {
                    n.Expression.SetValue(expression);
                }

                var columnName = selectScalarExpression.ColumnName;

                if (columnName != null)
                {
                    var identifier = columnName.Identifier != null ?
                        IdentifierVisitor.CreateIdentifier(columnName.Identifier) :
                        new QsiIdentifier(columnName.Value, false);

                    n.Alias.SetValue(new QsiAliasNode
                    {
                        Name = identifier
                    });
                }
            });
        }
        #endregion

        #region FromClause
        private QsiTableNode VisitFromClause(FromClause fromClause)
        {
            var tableReferences = fromClause.TableReferences;

            if (tableReferences.Count == 1)
                return VisitTableReference(tableReferences.FirstOrDefault());

            var joinedTableNode = TreeHelper.Create<QsiJoinedTableNode>(n =>
            {
                n.Left.SetValue(VisitTableReference(tableReferences[0]));
                n.Right.SetValue(VisitTableReference(tableReferences[1]));

                n.JoinType = QsiJoinType.Full;
            });

            foreach (var tableExpression in tableReferences.Skip(2))
            {
                var node = joinedTableNode;

                joinedTableNode = TreeHelper.Create<QsiJoinedTableNode>(n =>
                {
                    n.Left.SetValue(node);
                    n.Right.SetValue(VisitTableReference(tableExpression));
                    n.JoinType = QsiJoinType.Full;
                });
            }

            return joinedTableNode;
        }
        #endregion

        #region TableReference
        private QsiTableNode VisitTableReference(TableReference tableReference)
        {
            switch (tableReference)
            {
                // case JoinParenthesisTableReference joinParenthesisTableReference:
                //     return VisitJoinParenthesisTableReference(joinParenthesisTableReference);

                case JoinTableReference joinTableReference:
                    return VisitJoinTableReference(joinTableReference);

                // case OdbcQualifiedJoinTableReference odbcQualifiedJoinTableReference:
                //     return VisitOdbcQualifiedJoinTableReference(odbcQualifiedJoinTableReference);

                case TableReferenceWithAlias tableReferenceWithAlias:
                    return VisitTableReferenceWithAlias(tableReferenceWithAlias);
            }

            throw TreeHelper.NotSupportedTree(tableReference);
        }

        #region JoinTableReference
        private QsiJoinedTableNode VisitJoinTableReference(JoinTableReference joinTableReference)
        {
            switch (joinTableReference)
            {
                case QualifiedJoin qualifiedJoin:
                {
                    var joinType = qualifiedJoin.QualifiedJoinType switch
                    {
                        QualifiedJoinType.Inner => QsiJoinType.Inner,
                        QualifiedJoinType.FullOuter => QsiJoinType.Full,
                        QualifiedJoinType.LeftOuter => QsiJoinType.Left,
                        QualifiedJoinType.RightOuter => QsiJoinType.Right,
                        _ => throw new InvalidOperationException()
                    };

                    return CreateJoinedTableNode(joinType, qualifiedJoin);
                }
                case UnqualifiedJoin unqualifiedJoin:
                {
                    var joinType = unqualifiedJoin.UnqualifiedJoinType switch
                    {
                        UnqualifiedJoinType.CrossApply => QsiJoinType.Inner,
                        UnqualifiedJoinType.CrossJoin => QsiJoinType.Cross,
                        UnqualifiedJoinType.OuterApply => QsiJoinType.Left,
                        _ => throw new InvalidOperationException()
                    };

                    return CreateJoinedTableNode(joinType, unqualifiedJoin);
                }
            }

            throw TreeHelper.NotSupportedTree(joinTableReference);
        }

        private QsiJoinedTableNode CreateJoinedTableNode(QsiJoinType qsiJoinType, JoinTableReference joinTableReference)
        {
            return TreeHelper.Create<QsiJoinedTableNode>(n =>
            {
                n.Left.SetValue(VisitTableReference(joinTableReference.FirstTableReference));
                n.Right.SetValue(VisitTableReference(joinTableReference.SecondTableReference));
                n.JoinType = qsiJoinType;
            });
        }
        #endregion

        #region TableReferenceWithAlias
        private QsiTableNode VisitTableReferenceWithAlias(TableReferenceWithAlias tableReferenceWithAlias)
        {
            switch (tableReferenceWithAlias)
            {
                // case AdHocTableReference adHocTableReference:
                //     return VisitAdHocTableReference(adHocTableReference);
                //
                // case BuiltInFunctionTableReference builtInFunctionTableReference:
                //     return VisitBuiltInFunctionTableReference(builtInFunctionTableReference);
                //
                // case FullTextTableReference fullTextTableReference:
                //     return VisitFullTextTableReference(fullTextTableReference);
                //
                // case GlobalFunctionTableReference globalFunctionTableReference:
                //     return VisitGlobalFunctionTableReference(globalFunctionTableReference);
                //
                // case InternalOpenRowset internalOpenRowset:
                //     return VisitInternalOpenRowset(internalOpenRowset);

                case NamedTableReference namedTableReference:
                    return VisitNamedTableReference(namedTableReference);

                //
                // case OpenJsonTableReference openJsonTableReference:
                //     return VisitOpenJsonTableReference(openJsonTableReference);
                //
                // case OpenQueryTableReference openQueryTableReference:
                //     return VisitOpenQueryTableReference(openQueryTableReference);
                //
                // case OpenRowsetTableReference openRowsetTableReference:
                //     return VisitOpenRowsetTableReference(openRowsetTableReference);
                //
                // case OpenXmlTableReference openXmlTableReference:
                //     return VisitOpenXmlTableReference(openXmlTableReference);
                //
                // case PivotedTableReference pivotedTableReference:
                //     return VisitPivotedTableReference(pivotedTableReference);
                //
                // case SemanticTableReference semanticTableReference:
                //     return VisitSemanticTableReference(semanticTableReference);
                //
                // case TableReferenceWithAliasAndColumns tableReferenceWithAliasAndColumns:
                //     return VisitTableReferenceWithAliasAndColumns(tableReferenceWithAliasAndColumns);
                //
                // case UnpivotedTableReference unpivotedTableReference:
                //     return VisitUnpivotedTableReference(unpivotedTableReference);
                //
                // case VariableTableReference variableTableReference:
                //     return VisitVariableTableReference(variableTableReference);
            }

            throw TreeHelper.NotSupportedTree(tableReferenceWithAlias);
        }

        private QsiTableNode VisitNamedTableReference(NamedTableReference namedTableReference)
        {
            var tableNode = new QsiTableAccessNode
            {
                Identifier = IdentifierVisitor.CreateQualifiedIdentifier(namedTableReference.SchemaObject)
            };

            if (namedTableReference.Alias == null)
                return tableNode;

            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                n.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
                n.Source.SetValue(tableNode);

                n.Alias.SetValue(new QsiAliasNode
                {
                    Name = IdentifierVisitor.CreateIdentifier(namedTableReference.Alias)
                });
            });
        }
        #endregion
        #endregion
    }
}

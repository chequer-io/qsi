using System;
using System.Collections.Generic;
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
            var tableNode = VisitQueryExpression(selectStatement.QueryExpression);

            if (selectStatement.WithCtesAndXmlNamespaces != null)
            {
                var tableDirectivesNode = VisitWithCtesAndXmlNamespaces(selectStatement.WithCtesAndXmlNamespaces);

                switch (tableNode)
                {
                    case QsiDerivedTableNode derivedTableNode:
                        derivedTableNode.Directives.SetValue(tableDirectivesNode);
                        break;

                    default:
                        return TreeHelper.Create<QsiDerivedTableNode>(n =>
                        {
                            n.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
                            n.Source.SetValue(tableNode);
                            n.Directives.SetValue(tableDirectivesNode);
                        });
                }
            }

            return tableNode;
        }

        private QsiTableNode VisitDataModificationStatement(DataModificationStatement dataModificationStatement)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region WithCtesAndXmlNamespaces (Table Directives)
        private QsiTableDirectivesNode VisitWithCtesAndXmlNamespaces(WithCtesAndXmlNamespaces selectStatementWithCtesAndXmlNamespaces)
        {
            return TreeHelper.Create<QsiTableDirectivesNode>(n =>
            {
                n.IsRecursive = true;
                n.Tables.AddRange(selectStatementWithCtesAndXmlNamespaces.CommonTableExpressions.Select(VisitCommonTableExpression));
            });
        }

        private QsiDerivedTableNode VisitCommonTableExpression(CommonTableExpression commonTableExpression)
        {
            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                n.Source.SetValue(VisitQueryExpression(commonTableExpression.QueryExpression));

                var columnsDeclaration = new QsiColumnsDeclarationNode();

                if (commonTableExpression.Columns == null || commonTableExpression.Columns.Count == 0)
                {
                    columnsDeclaration.Columns.Add(new QsiAllColumnNode());
                }
                else
                {
                    columnsDeclaration.Columns.AddRange(CreateSequentialColumnNodes(commonTableExpression.Columns));
                }

                n.Columns.SetValue(columnsDeclaration);

                if (commonTableExpression.ExpressionName != null)
                {
                    n.Alias.SetValue(new QsiAliasNode
                    {
                        Name = IdentifierVisitor.CreateIdentifier(commonTableExpression.ExpressionName)
                    });
                }
            });
        }

        private IEnumerable<QsiColumnNode> CreateSequentialColumnNodes(IEnumerable<Identifier> columns)
        {
            return columns
                .Select((identifier, i) => TreeHelper.Create<QsiSequentialColumnNode>(n =>
                {
                    n.Ordinal = i;

                    n.Alias.SetValue(new QsiAliasNode
                    {
                        Name = IdentifierVisitor.CreateIdentifier(identifier)
                    });
                }));
        }
        #endregion

        #region QueryExpression
        public QsiTableNode VisitQueryExpression(QueryExpression queryExpression)
        {
            switch (queryExpression)
            {
                case BinaryQueryExpression binaryQueryExpression:
                    return VisitBinaryQueryExpression(binaryQueryExpression);

                case QueryParenthesisExpression queryParenthesisExpression:
                    break;

                case QuerySpecification querySpecification:
                    return VisitQuerySpecification(querySpecification);
            }

            throw TreeHelper.NotSupportedTree(queryExpression);
        }

        private QsiCompositeTableNode VisitBinaryQueryExpression(BinaryQueryExpression binaryQueryExpression)
        {
            return TreeHelper.Create<QsiCompositeTableNode>(n =>
            {
                n.Sources.Add(VisitQueryExpression(binaryQueryExpression.FirstQueryExpression));
                n.Sources.Add(VisitQueryExpression(binaryQueryExpression.SecondQueryExpression));
            });
        }

        private QsiDerivedTableNode VisitQuerySpecification(QuerySpecification querySpecification)
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

                case SelectSetVariable selectSetVariable:
                    return VisitSelectSetVariable(selectSetVariable);

                case SelectStarExpression selectStarExpression:
                    return VisitSelectStarExpression(selectStarExpression);
            }

            throw TreeHelper.NotSupportedTree(selectElement);
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

        private QsiDerivedColumnNode VisitSelectSetVariable(SelectSetVariable selectSetVariable)
        {
            return TreeHelper.Create<QsiDerivedColumnNode>(n =>
            {
                var op = selectSetVariable.AssignmentKind switch
                {
                    AssignmentKind.Equals => "=",
                    AssignmentKind.AddEquals => "+=",
                    AssignmentKind.DivideEquals => "/=",
                    AssignmentKind.ModEquals => "%=",
                    AssignmentKind.MultiplyEquals => "*=",
                    AssignmentKind.SubtractEquals => "-=",
                    AssignmentKind.BitwiseAndEquals => "&=",
                    AssignmentKind.BitwiseOrEquals => "|=",
                    AssignmentKind.BitwiseXorEquals => "^=",
                    _ => throw new InvalidOperationException()
                };

                n.Expression.SetValue(TreeHelper.CreateLogicalExpression(
                    op,
                    selectSetVariable.Variable, selectSetVariable.Expression,
                    ExpressionVisitor.VisitScalarExpression
                ));
            });
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

                case FullTextTableReference fullTextTableReference:
                    return VisitFullTextTableReference(fullTextTableReference);

                // case GlobalFunctionTableReference globalFunctionTableReference:
                //     return VisitGlobalFunctionTableReference(globalFunctionTableReference);
                //
                // case InternalOpenRowset internalOpenRowset:
                //     return VisitInternalOpenRowset(internalOpenRowset);

                case NamedTableReference namedTableReference:
                    return VisitNamedTableReference(namedTableReference);

                case OpenJsonTableReference openJsonTableReference:
                    return VisitOpenJsonTableReference(openJsonTableReference);

                case OpenQueryTableReference openQueryTableReference:
                    return VisitOpenQueryTableReference(openQueryTableReference);

                case OpenRowsetTableReference openRowsetTableReference:
                    return VisitOpenRowsetTableReference(openRowsetTableReference);

                case OpenXmlTableReference openXmlTableReference:
                    return VisitOpenXmlTableReference(openXmlTableReference);

                case PivotedTableReference pivotedTableReference:
                    return VisitPivotedTableReference(pivotedTableReference);

                // case SemanticTableReference semanticTableReference:
                //     return VisitSemanticTableReference(semanticTableReference);

                case TableReferenceWithAliasAndColumns tableReferenceWithAliasAndColumns:
                    return VisitTableReferenceWithAliasAndColumns(tableReferenceWithAliasAndColumns);

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

        // FREETEXTTABLE (table , { column_name | (column_list) | * }, 'freetext_string' [ , LANGUAGE language_term ] [ , top_n_by_rank ] )  
        private QsiTableNode VisitFullTextTableReference(FullTextTableReference fullTextTableReference)
        {
            throw TreeHelper.NotSupportedFeature("Table function");

            // var node = TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            // {
            //     n.Member.SetValue(TreeHelper.CreateFunctionAccess(SqlServerKnownFunction.FreeTextTable));
            //
            //     n.Parameters.Add(TreeHelper.Create<QsiTableExpressionNode>(ten =>
            //     {
            //         ten.Table.SetValue(new QsiTableAccessNode
            //         {
            //             Identifier = IdentifierVisitor.CreateQualifiedIdentifier(fullTextTableReference.TableName)
            //         });
            //     }));
            //
            //     n.Parameters.AddRange(fullTextTableReference.Columns.Select(ExpressionVisitor.VisitColumnReferenceExpression));
            //     n.Parameters.Add(ExpressionVisitor.VisitValueExpression(fullTextTableReference.SearchCondition));
            //     
            //     if (fullTextTableReference.Language != null)
            //         n.Parameters.Add(ExpressionVisitor.VisitValueExpression(fullTextTableReference.Language));
            //     
            //     if (fullTextTableReference.TopN != null)
            //         n.Parameters.Add(ExpressionVisitor.VisitValueExpression(fullTextTableReference.TopN));
            // });
        }

        // OPENJSON(VARIABLE[, PATH]) Ex: OPENJSON(@json, '$.path.to."sub-object"')
        // OPENJSON(VARIABLE[, PATH]) [WITH_CLAUSE]
        private QsiDerivedTableNode VisitOpenJsonTableReference(OpenJsonTableReference openJsonTableReference)
        {
            throw TreeHelper.NotSupportedFeature("Table function");

            // // TODO: Impl With Clause
            // var node = TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            // {
            //     n.Member.SetValue(TreeHelper.CreateFunctionAccess(SqlServerKnownFunction.OpenJson));
            //     n.Parameters.Add(ExpressionVisitor.VisitVariableReference(openJsonTableReference.Variable));
            //
            //     if (openJsonTableReference.RowPattern != null)
            //     {
            //         n.Parameters.Add(TreeHelper.CreateLiteral(openJsonTableReference.RowPattern.Value));
            //     }
            // });
        }

        // OPENQUERY(linked_server ,'query') Ex: SELECT * FROM OPENQUERY(OracleSvr, 'SELECT name FROM actor WHERE actor_id = ''abc''');
        private QsiTableNode VisitOpenQueryTableReference(OpenQueryTableReference openQueryTableReference)
        {
            throw TreeHelper.NotSupportedFeature("Table function");

            // var node = TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            // {
            //     n.Member.SetValue(TreeHelper.CreateFunctionAccess(SqlServerKnownFunction.OpenQuery));
            //
            //     // TODO: Server Identfiier is not variable
            //     n.Parameters.Add(new QsiVariableAccessExpressionNode
            //     {
            //         Identifier = new QsiQualifiedIdentifier(IdentifierVisitor.CreateIdentifier(openQueryTableReference.LinkedServer))
            //     });
            //
            //     n.Parameters.Add(ExpressionVisitor.VisitLiteral(openQueryTableReference.Query));
            // });
        }

        private QsiTableNode VisitOpenRowsetTableReference(OpenRowsetTableReference openRowsetTableReference)
        {
            throw TreeHelper.NotSupportedFeature("Remote table");
        }

        private QsiTableNode VisitOpenXmlTableReference(OpenXmlTableReference openXmlTableReference)
        {
            throw TreeHelper.NotSupportedFeature("Table function");

            // // TODO: Impl With Clause
            // var node = TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            // {
            //     n.Member.SetValue(TreeHelper.CreateFunctionAccess(SqlServerKnownFunction.OpenXml));
            //     
            //     n.Parameters.Add(ExpressionVisitor.VisitVariableReference(openXmlTableReference.Variable));
            //     n.Parameters.Add(ExpressionVisitor.VisitValueExpression(openXmlTableReference.RowPattern));
            //
            //     if (openXmlTableReference.Flags != null)
            //     {
            //         n.Parameters.Add(ExpressionVisitor.VisitValueExpression(openXmlTableReference.Flags));    
            //     }
            // });
        }

        private QsiTableNode VisitPivotedTableReference(PivotedTableReference pivotedTableReference)
        {
            // TODO: Implement
            throw TreeHelper.NotSupportedTree(pivotedTableReference);
        }

        #region TableReferenceWithAliasAndColumns
        private QsiTableNode VisitTableReferenceWithAliasAndColumns(TableReferenceWithAliasAndColumns tableReferenceWithAliasAndColumns)
        {
            switch (tableReferenceWithAliasAndColumns)
            {
                // case BulkOpenRowset bulkOpenRowset:
                //     return VisitBulkOpenRowset(bulkOpenRowset);
                //
                // case ChangeTableChangesTableReference changeTableChangesTableReference:
                //     return VisitChangeTableChangesTableReference(changeTableChangesTableReference);
                //
                // case ChangeTableVersionTableReference changeTableVersionTableReference:
                //     return VisitChangeTableVersionTableReference(changeTableVersionTableReference);
                //
                // case DataModificationTableReference dataModificationTableReference:
                //     return VisitDataModificationTableReference(dataModificationTableReference);
                //
                // case InlineDerivedTable inlineDerivedTable:
                //     return VisitInlineDerivedTable(inlineDerivedTable);

                case QueryDerivedTable queryDerivedTable:
                    return VisitQueryDerivedTable(queryDerivedTable);

                // case SchemaObjectFunctionTableReference schemaObjectFunctionTableReference:
                //     return VisitSchemaObjectFunctionTableReference(schemaObjectFunctionTableReference);
                //
                // case VariableMethodCallTableReference variableMethodCallTableReference:
                //     return VisitVariableMethodCallTableReference(variableMethodCallTableReference);
            }

            throw TreeHelper.NotSupportedTree(tableReferenceWithAliasAndColumns);
        }

        private QsiDerivedTableNode VisitQueryDerivedTable(QueryDerivedTable queryDerivedTable)
        {
            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                if (queryDerivedTable.Alias != null)
                {
                    n.Alias.SetValue(new QsiAliasNode
                    {
                        Name = IdentifierVisitor.CreateIdentifier(queryDerivedTable.Alias)
                    });
                }

                n.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());

                n.Source.SetValue(VisitQueryExpression(queryDerivedTable.QueryExpression));
            });
        }
        #endregion
        #endregion
        #endregion
    }
}

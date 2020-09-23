using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Qsi.Data;
using Qsi.Tree.Base;
using Qsi.Utilities;

namespace Qsi.SqlServer.Tree
{
    internal sealed class TableVisitor : VisitorBase
    {
        public TableVisitor(IVisitorContext visitorContext) : base(visitorContext)
        {
        }

        public QsiTreeNode Visit(TSqlBatch sqlBatch)
        {
            var statement = sqlBatch?.Statements?.FirstOrDefault()
                            ?? throw new QsiException(QsiError.SyntaxError);

            return VisitStatements(statement);
        }

        public QsiTreeNode VisitStatements(TSqlStatement statement)
        {
            switch (statement)
            {
                case StatementWithCtesAndXmlNamespaces statementWithCtesAndXmlNamespaces:
                    return VisitStatementWithCtesAndXmlNamespaces(statementWithCtesAndXmlNamespaces);

                case ViewStatementBody viewStatementBody:
                    return VisitStatementBody(viewStatementBody);
            }

            throw TreeHelper.NotSupportedTree(statement);
        }

        #region StatementWithCtesAndXmlNamespaces
        public QsiTreeNode VisitStatementWithCtesAndXmlNamespaces(StatementWithCtesAndXmlNamespaces statementWithCtesAndXmlNamespaces)
        {
            switch (statementWithCtesAndXmlNamespaces)
            {
                case SelectStatement selectStatement:
                    return VisitSelectStatement(selectStatement);
            }

            throw TreeHelper.NotSupportedTree(statementWithCtesAndXmlNamespaces);
        }

        public QsiTableNode VisitSelectStatement(SelectStatement selectStatement)
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
        #endregion

        #region StatementBody
        public QsiTableNode VisitStatementBody(ViewStatementBody viewStatementBody)
        {
            switch (viewStatementBody)
            {
                case CreateViewStatement createViewStatement:
                    return VisitCreateViewStatement(createViewStatement);
            }

            throw TreeHelper.NotSupportedTree(viewStatementBody);
        }

        public QsiTableNode VisitCreateViewStatement(CreateViewStatement createViewStatement)
        {
            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                if (createViewStatement.Columns == null || createViewStatement.Columns.Count == 0)
                {
                    n.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
                }
                else
                {
                    var columnsDeclaration = new QsiColumnsDeclarationNode();
                    columnsDeclaration.Columns.AddRange(CreateSequentialColumnNodes(createViewStatement.Columns));
                    n.Columns.SetValue(columnsDeclaration);
                }

                n.Source.SetValue(VisitSelectStatement(createViewStatement.SelectStatement));

                n.Alias.SetValue(CreateAliasNode(createViewStatement.SchemaObjectName[^1]));
            });
        }
        #endregion

        #region WithCtesAndXmlNamespaces (Table Directives)
        public QsiTableDirectivesNode VisitWithCtesAndXmlNamespaces(WithCtesAndXmlNamespaces selectStatementWithCtesAndXmlNamespaces)
        {
            return TreeHelper.Create<QsiTableDirectivesNode>(n =>
            {
                n.IsRecursive = true;
                n.Tables.AddRange(selectStatementWithCtesAndXmlNamespaces.CommonTableExpressions.Select(VisitCommonTableExpression));
            });
        }

        public QsiDerivedTableNode VisitCommonTableExpression(CommonTableExpression commonTableExpression)
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
                    n.Alias.SetValue(CreateAliasNode(commonTableExpression.ExpressionName));
                }
            });
        }

        public IEnumerable<QsiColumnNode> CreateSequentialColumnNodes(IEnumerable<Identifier> columns)
        {
            return columns
                .Select((identifier, i) => TreeHelper.Create<QsiSequentialColumnNode>(n =>
                {
                    n.Ordinal = i;

                    n.Alias.SetValue(CreateAliasNode(identifier));
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
                    return VisitQueryParenthesisExpression(queryParenthesisExpression);

                case QuerySpecification querySpecification:
                    return VisitQuerySpecification(querySpecification);
            }

            throw TreeHelper.NotSupportedTree(queryExpression);
        }

        public QsiCompositeTableNode VisitBinaryQueryExpression(BinaryQueryExpression binaryQueryExpression)
        {
            return TreeHelper.Create<QsiCompositeTableNode>(n =>
            {
                n.Sources.Add(VisitQueryExpression(binaryQueryExpression.FirstQueryExpression));
                n.Sources.Add(VisitQueryExpression(binaryQueryExpression.SecondQueryExpression));
            });
        }

        public QsiDerivedTableNode VisitQueryParenthesisExpression(QueryParenthesisExpression queryParenthesisExpression)
        {
            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                // Ignored ForClause, OffsetClause, OrderByClause
                n.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
                n.Source.SetValue(VisitQueryExpression(queryParenthesisExpression.QueryExpression));
            });
        }

        public QsiDerivedTableNode VisitQuerySpecification(QuerySpecification querySpecification)
        {
            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                // Ignored HavingClause, WhereClause, GroupByClause, TopRowFilter, UniqueRowFilter, ForClause, OffsetClause
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
        public QsiColumnNode VisitSelectElement(SelectElement selectElement)
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

        public QsiColumnNode VisitSelectScalarExpression(SelectScalarExpression selectScalarExpression)
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
                    if (columnName.Identifier == null)
                    {
                        n.Alias.SetValue(new QsiAliasNode
                        {
                            Name = new QsiIdentifier(columnName.Value, false)
                        });
                    }
                    else
                    {
                        n.Alias.SetValue(CreateAliasNode(columnName.Identifier));
                    }
                }
            });
        }

        public QsiDerivedColumnNode VisitSelectSetVariable(SelectSetVariable selectSetVariable)
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

                n.Expression.SetValue(TreeHelper.Create<QsiAssignExpressionNode>(en =>
                {
                    en.Variable.SetValue(ExpressionVisitor.VisitVariableReference(selectSetVariable.Variable));
                    en.Operator = op;
                    en.Value.SetValue(ExpressionVisitor.VisitScalarExpression(selectSetVariable.Expression));
                }));
            });
        }

        public QsiColumnNode VisitSelectStarExpression(SelectStarExpression selectStarExpression)
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
        public QsiTableNode VisitFromClause(FromClause fromClause)
        {
            IList<TableReference> tableReferences = fromClause.TableReferences;

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
        public QsiTableNode VisitTableReference(TableReference tableReference)
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

            throw TreeHelper.NotSupportedTree(tableReference);
        }

        public QsiTableNode VisitJoinParenthesisTableReference(JoinParenthesisTableReference joinParenthesisTableReference)
        {
            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                n.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
                n.Source.SetValue(VisitTableReference(joinParenthesisTableReference.Join));
            });
        }

        #region JoinTableReference
        public QsiJoinedTableNode VisitJoinTableReference(JoinTableReference joinTableReference)
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

        public QsiJoinedTableNode CreateJoinedTableNode(QsiJoinType qsiJoinType, JoinTableReference joinTableReference)
        {
            return TreeHelper.Create<QsiJoinedTableNode>(n =>
            {
                n.Left.SetValue(VisitTableReference(joinTableReference.FirstTableReference));
                n.Right.SetValue(VisitTableReference(joinTableReference.SecondTableReference));
                n.JoinType = qsiJoinType;
            });
        }
        #endregion

        public QsiDerivedTableNode VisitOdbcQualifiedJoinTableReference(OdbcQualifiedJoinTableReference odbcQualifiedJoinTableReference)
        {
            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                n.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
                n.Source.SetValue(VisitTableReference(odbcQualifiedJoinTableReference.TableReference));
            });
        }

        #region TableReferenceWithAlias
        public QsiTableNode VisitTableReferenceWithAlias(TableReferenceWithAlias tableReferenceWithAlias)
        {
            switch (tableReferenceWithAlias)
            {
                // OPENDATASOURCE
                case AdHocTableReference adHocTableReference:
                    return VisitAdHocTableReference(adHocTableReference);

                case BuiltInFunctionTableReference builtInFunctionTableReference:
                    return VisitBuiltInFunctionTableReference(builtInFunctionTableReference);

                case FullTextTableReference fullTextTableReference:
                    return VisitFullTextTableReference(fullTextTableReference);

                case GlobalFunctionTableReference globalFunctionTableReference:
                    return VisitGlobalFunctionTableReference(globalFunctionTableReference);

                // OPENROWSET
                case InternalOpenRowset internalOpenRowset:
                    return VisitInternalOpenRowSet(internalOpenRowset);

                case NamedTableReference namedTableReference:
                    return VisitNamedTableReference(namedTableReference);

                case OpenJsonTableReference openJsonTableReference:
                    return VisitOpenJsonTableReference(openJsonTableReference);

                case OpenQueryTableReference openQueryTableReference:
                    return VisitOpenQueryTableReference(openQueryTableReference);

                // OPENROWSET
                case OpenRowsetTableReference openRowsetTableReference:
                    return VisitOpenRowsetTableReference(openRowsetTableReference);

                case OpenXmlTableReference openXmlTableReference:
                    return VisitOpenXmlTableReference(openXmlTableReference);

                case PivotedTableReference pivotedTableReference:
                    return VisitPivotedTableReference(pivotedTableReference);

                case SemanticTableReference semanticTableReference:
                    return VisitSemanticTableReference(semanticTableReference);

                case TableReferenceWithAliasAndColumns tableReferenceWithAliasAndColumns:
                    return VisitTableReferenceWithAliasAndColumns(tableReferenceWithAliasAndColumns);

                case UnpivotedTableReference unpivotedTableReference:
                    return VisitUnpivotedTableReference(unpivotedTableReference);

                case VariableTableReference variableTableReference:
                    return VisitVariableTableReference(variableTableReference);
            }

            throw TreeHelper.NotSupportedTree(tableReferenceWithAlias);
        }

        public QsiTableNode VisitAdHocTableReference(AdHocTableReference adHocTableReference)
        {
            throw TreeHelper.NotSupportedFeature("Remote table");
        }

        public QsiTableNode VisitBuiltInFunctionTableReference(BuiltInFunctionTableReference builtInFunctionTableReference)
        {
            throw TreeHelper.NotSupportedFeature("Table function");
        }

        public QsiTableNode VisitVariableTableReference(VariableTableReference variableTableReference)
        {
            throw TreeHelper.NotSupportedFeature("Table variable");
        }

        public QsiTableNode VisitGlobalFunctionTableReference(GlobalFunctionTableReference globalFunctionTableReference)
        {
            throw TreeHelper.NotSupportedFeature("Table function");
        }

        public QsiTableNode VisitInternalOpenRowSet(InternalOpenRowset internalOpenRowset)
        {
            throw TreeHelper.NotSupportedFeature("Remote table");
        }

        public QsiTableNode VisitNamedTableReference(NamedTableReference namedTableReference)
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

                n.Alias.SetValue(CreateAliasNode(namedTableReference.Alias));
            });
        }

        public QsiTableNode VisitOpenRowsetTableReference(OpenRowsetTableReference openRowsetTableReference)
        {
            throw TreeHelper.NotSupportedFeature("Remote table");
        }

        // FREETEXTTABLE (table , { column_name | (column_list) | * }, 'freetext_string' [ , LANGUAGE language_term ] [ , top_n_by_rank ] )  
        public QsiTableNode VisitFullTextTableReference(FullTextTableReference fullTextTableReference)
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
        public QsiDerivedTableNode VisitOpenJsonTableReference(OpenJsonTableReference openJsonTableReference)
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
        public QsiTableNode VisitOpenQueryTableReference(OpenQueryTableReference openQueryTableReference)
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

        public QsiTableNode VisitOpenXmlTableReference(OpenXmlTableReference openXmlTableReference)
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

        public QsiTableNode VisitPivotedTableReference(PivotedTableReference pivotedTableReference)
        {
            // TODO: Implement
            throw TreeHelper.NotSupportedFeature("Pivot table");
        }

        public QsiTableNode VisitUnpivotedTableReference(UnpivotedTableReference unpivotedTableReference)
        {
            // TODO: Implement
            throw TreeHelper.NotSupportedFeature("Unpivot table");
        }

        public QsiTableNode VisitSemanticTableReference(SemanticTableReference semanticTableReference)
        {
            // TODO: Implement
            throw TreeHelper.NotSupportedFeature("Semantic table");
        }

        #region TableReferenceWithAliasAndColumns
        public QsiTableNode VisitTableReferenceWithAliasAndColumns(TableReferenceWithAliasAndColumns tableReferenceWithAliasAndColumns)
        {
            switch (tableReferenceWithAliasAndColumns)
            {
                // OPENROWSET (Bulk..)
                case BulkOpenRowset bulkOpenRowset:
                    return VisitBulkOpenRowset(bulkOpenRowset);

                case ChangeTableChangesTableReference changeTableChangesTableReference:
                    return VisitChangeTableChangesTableReference(changeTableChangesTableReference);

                case ChangeTableVersionTableReference changeTableVersionTableReference:
                    return VisitChangeTableVersionTableReference(changeTableVersionTableReference);

                case DataModificationTableReference dataModificationTableReference:
                    return VisitDataModificationTableReference(dataModificationTableReference);

                case InlineDerivedTable inlineDerivedTable:
                    return VisitInlineDerivedTable(inlineDerivedTable);

                case QueryDerivedTable queryDerivedTable:
                    return VisitQueryDerivedTable(queryDerivedTable);

                case SchemaObjectFunctionTableReference schemaObjectFunctionTableReference:
                    return VisitSchemaObjectFunctionTableReference(schemaObjectFunctionTableReference);

                case VariableMethodCallTableReference variableMethodCallTableReference:
                    return VisitVariableMethodCallTableReference(variableMethodCallTableReference);
            }

            throw TreeHelper.NotSupportedTree(tableReferenceWithAliasAndColumns);
        }

        public QsiTableNode VisitBulkOpenRowset(BulkOpenRowset bulkOpenRowset)
        {
            throw TreeHelper.NotSupportedFeature("Remote table");
        }

        // CHANGETABLE Function
        public QsiTableNode VisitSchemaObjectFunctionTableReference(SchemaObjectFunctionTableReference schemaObjectFunctionTableReference)
        {
            throw TreeHelper.NotSupportedFeature("Table function");
        }

        // CHANGETABLE Function
        public QsiTableNode VisitChangeTableChangesTableReference(ChangeTableChangesTableReference changeTableChangesTableReference)
        {
            throw TreeHelper.NotSupportedFeature("Table function");
        }

        public QsiTableNode VisitChangeTableVersionTableReference(ChangeTableVersionTableReference changeTableVersionTableReference)
        {
            throw TreeHelper.NotSupportedFeature("Table function");
        }

        public QsiTableNode VisitDataModificationTableReference(DataModificationTableReference dataModificationTableReference)
        {
            throw TreeHelper.NotSupportedFeature("Data modification table");
        }

        public QsiTableNode VisitInlineDerivedTable(InlineDerivedTable inlineDerivedTable)
        {
            return TreeHelper.Create<QsiInlineDerivedTableNode>(n =>
            {
                if (inlineDerivedTable.Alias != null)
                {
                    n.Alias.SetValue(CreateAliasNode(inlineDerivedTable.Alias));
                }

                if (!ListUtility.IsNullOrEmpty(inlineDerivedTable.Columns))
                {
                    var columnsDeclaration = new QsiColumnsDeclarationNode();
                    columnsDeclaration.Columns.AddRange(CreateSequentialColumnNodes(inlineDerivedTable.Columns));
                    n.Columns.SetValue(columnsDeclaration);
                }

                n.Rows.AddRange(inlineDerivedTable.RowValues.Select(ExpressionVisitor.VisitRowValue));
            });
        }

        public QsiDerivedTableNode VisitQueryDerivedTable(QueryDerivedTable queryDerivedTable)
        {
            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                if (queryDerivedTable.Alias != null)
                {
                    n.Alias.SetValue(CreateAliasNode(queryDerivedTable.Alias));
                }

                n.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
                n.Source.SetValue(VisitQueryExpression(queryDerivedTable.QueryExpression));
            });
        }

        public QsiTableNode VisitVariableMethodCallTableReference(VariableMethodCallTableReference variableMethodCallTableReference)
        {
            throw TreeHelper.NotSupportedFeature("Table function");
        }
        #endregion
        #endregion
        #endregion

        public QsiAliasNode CreateAliasNode(Identifier identifier)
        {
            return new QsiAliasNode
            {
                Name = IdentifierVisitor.CreateIdentifier(identifier)
            };
        }
    }
}

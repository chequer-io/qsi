using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;
using Qsi.Data;
using Qsi.Tree.Base;
using Qsi.Utilities;

namespace Qsi.SqlServer.Tree
{
    internal static class TableVisitor
    {
        #region Tree
        public static QsiTableNode Visit(SqlCodeObject codeObject)
        {
            if (codeObject == null)
                return null;

            switch (codeObject)
            {
                case SqlSelectStatement selectStatement:
                    return VisitSelectStatement(selectStatement);

                case SqlCreateViewStatement createViewStatement:
                    return VisitCreateViewStatement(createViewStatement);
            }

            return null;
        }
        #endregion

        #region Select Statements
        public static QsiTableNode VisitSelectStatement(SqlSelectStatement selectStatement)
        {
            var tableNode = VisitSelectSpecification(selectStatement.SelectSpecification);

            if (selectStatement.QueryWithClause != null)
            {
                var tableDirectivesNode = VisitQueryWithClause(selectStatement.QueryWithClause);

                switch (tableNode)
                {
                    case QsiDerivedTableNode derivedTableNode:
                        derivedTableNode.Directives.SetValue(tableDirectivesNode);
                        break;

                    default:
                        return TreeHelper.Create<QsiDerivedTableNode>(n =>
                        {
                            var columnsDeclaration = new QsiColumnsDeclarationNode();
                            columnsDeclaration.Columns.Add(new QsiAllColumnNode());

                            n.Columns.SetValue(columnsDeclaration);
                            n.Source.SetValue(tableNode);
                            n.Directives.SetValue(tableDirectivesNode);
                        });
                }
            }

            return tableNode;
        }

        public static QsiTableDirectivesNode VisitQueryWithClause(SqlQueryWithClause queryWithClause)
        {
            return TreeHelper.Create<QsiTableDirectivesNode>(n =>
            {
                n.Tables.AddRange(queryWithClause.CommonTableExpressions.Select(VisitTableExpression));
            });
        }

        public static QsiTableNode VisitSelectSpecification(SqlSelectSpecification selectSpecification)
        {
            return VisitQueryExpression(selectSpecification.QueryExpression);
        }

        public static QsiTableNode VisitQueryExpression(SqlQueryExpression queryExpression)
        {
            switch (queryExpression)
            {
                case SqlQuerySpecification querySpecification:
                    return VisitQuerySpecification(querySpecification);

                case SqlBinaryQueryExpression binaryQueryExpression:
                    return VisitBinaryQueryExpression(binaryQueryExpression);
            }

            return null;
        }

        private static QsiDerivedTableNode VisitQuerySpecification(SqlQuerySpecification querySpecification)
        {
            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                foreach (var codeObject in querySpecification.Children)
                {
                    switch (codeObject)
                    {
                        case SqlSelectClause selectClause:
                            n.Columns.SetValue(VisitSelectClause(selectClause));
                            break;

                        case SqlFromClause fromClause:
                            n.Source.SetValue(VisitFromClause(fromClause));
                            break;
                    }
                }
            });
        }

        private static QsiCompositeTableNode VisitBinaryQueryExpression(SqlBinaryQueryExpression binaryQueryExpression)
        {
            return TreeHelper.Create<QsiCompositeTableNode>(n =>
            {
                n.Sources.Add(VisitQueryExpression(binaryQueryExpression.Left));
                n.Sources.Add(VisitQueryExpression(binaryQueryExpression.Right));
            });
        }

        public static QsiColumnsDeclarationNode VisitSelectClause(SqlSelectClause selectClause)
        {
            return TreeHelper.Create<QsiColumnsDeclarationNode>(dn =>
            {
                foreach (var selectExpression in selectClause.SelectExpressions)
                {
                    switch (selectExpression)
                    {
                        case SqlSelectStarExpression starExpression:
                            dn.Columns.Add(VisitStarExpression(starExpression));
                            break;

                        case SqlSelectScalarExpression scalarExpression:
                            dn.Columns.Add(VisitSelectScalarExpression(scalarExpression));
                            break;
                    }
                }
            });
        }

        public static QsiTableNode VisitFromClause(SqlFromClause fromClause)
        {
            var tableExpressions = fromClause.TableExpressions;

            if (tableExpressions.Count == 1)
                return VisitTableExpression(tableExpressions.FirstOrDefault());

            var joinedTableNode = TreeHelper.Create<QsiJoinedTableNode>(n =>
            {
                n.Left.SetValue(VisitTableExpression(tableExpressions[0]));
                n.Right.SetValue(VisitTableExpression(tableExpressions[1]));

                n.JoinType = QsiJoinType.Full;
            });

            foreach (var tableExpression in tableExpressions.Skip(2))
            {
                var node = joinedTableNode;

                joinedTableNode = TreeHelper.Create<QsiJoinedTableNode>(n =>
                {
                    n.Left.SetValue(node);
                    n.Right.SetValue(VisitTableExpression(tableExpression));
                    n.JoinType = QsiJoinType.Full;
                });
            }

            return joinedTableNode;
        }

        private static QsiTableNode VisitTableExpression(SqlTableExpression tableExpression)
        {
            switch (tableExpression)
            {
                case SqlTableRefExpression tableRefExpression:
                    return VisitTableRefExpression(tableRefExpression);

                case SqlTableVariableRefExpression tableVariableRefExpression:
                    return VisitTableVariableRefExpression(tableVariableRefExpression);

                case SqlQualifiedJoinTableExpression qualifiedJoinTableExpression:
                    return VisitQualifiedJoinTableExpression(qualifiedJoinTableExpression);

                case SqlPivotTableExpression pivotTableExpression:
                    return VisitPivotTableExpression(pivotTableExpression);

                case SqlCommonTableExpression commonTableExpression:
                    return VisitCommonTableExpression(commonTableExpression);

                case SqlDerivedTableExpression derivedTableExpression:
                    return VisitDerivedTableExpression(derivedTableExpression);

                // TODO: Implement table function
                case SqlTableValuedFunctionRefExpression tableValuedFunctionRefExpression:
                    throw TreeHelper.NotSupportedFeature("Table function");
            }

            return null;
        }

        private static QsiTableNode VisitTableRefExpression(SqlTableRefExpression tableRefExpression)
        {
            var tableNode = new QsiTableAccessNode
            {
                Identifier = IdentifierVisitor.VisitMultipartIdentifier(tableRefExpression.ObjectIdentifier)
            };

            if (tableRefExpression.Alias == null)
                return tableNode;

            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                var allDeclaration = new QsiColumnsDeclarationNode();
                allDeclaration.Columns.Add(new QsiAllColumnNode());

                n.Columns.SetValue(allDeclaration);
                n.Source.SetValue(tableNode);

                n.Alias.SetValue(new QsiAliasNode
                {
                    Name = new QsiIdentifier(tableRefExpression.Alias.Value, false)
                });
            });
        }

        private static QsiTableNode VisitTableVariableRefExpression(SqlTableVariableRefExpression tableVariableRefExpression)
        {
            throw TreeHelper.NotSupportedFeature("Table variable");
        }
        
        private static QsiJoinedTableNode VisitQualifiedJoinTableExpression(SqlQualifiedJoinTableExpression qualifiedJoinTableExpression)
        {
            return TreeHelper.Create<QsiJoinedTableNode>(n =>
            {
                n.Left.SetValue(VisitTableExpression(qualifiedJoinTableExpression.Left));
                n.Right.SetValue(VisitTableExpression(qualifiedJoinTableExpression.Right));

                n.JoinType = qualifiedJoinTableExpression.JoinOperator switch
                {
                    SqlJoinOperatorType.CrossApply => QsiJoinType.Inner,
                    SqlJoinOperatorType.CrossJoin => QsiJoinType.Cross,
                    SqlJoinOperatorType.InnerJoin => QsiJoinType.Inner,
                    SqlJoinOperatorType.OuterApply => QsiJoinType.Left,
                    SqlJoinOperatorType.FullOuterJoin => QsiJoinType.Full,
                    SqlJoinOperatorType.LeftOuterJoin => QsiJoinType.Left,
                    SqlJoinOperatorType.RightOuterJoin => QsiJoinType.Right,
                    _ => throw new InvalidOperationException()
                };
            });
        }

        private static QsiTableNode VisitPivotTableExpression(SqlPivotTableExpression pivotTableExpression)
        {
            // TODO: Implement
            return null;
        }

        private static QsiDerivedTableNode VisitCommonTableExpression(SqlCommonTableExpression commonTableExpression)
        {
            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                var allDeclaration = new QsiColumnsDeclarationNode();
                allDeclaration.Columns.Add(new QsiAllColumnNode());

                n.Source.SetValue(VisitQueryExpression(commonTableExpression.QueryExpression));
                n.Columns.SetValue(allDeclaration);

                if (commonTableExpression.Name != null)
                {
                    n.Alias.SetValue(new QsiAliasNode
                    {
                        Name = new QsiIdentifier(commonTableExpression.Name.Value, false)
                    });
                }
            });
        }

        private static QsiDerivedTableNode VisitDerivedTableExpression(SqlDerivedTableExpression derivedTableExpression)
        {
            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                if (derivedTableExpression.Alias != null)
                {
                    n.Alias.SetValue(new QsiAliasNode
                    {
                        Name = IdentifierVisitor.CreateIdentifier(derivedTableExpression.Alias)
                    });
                }

                n.Columns.SetValue(new QsiColumnsDeclarationNode
                {
                    Columns =
                    {
                        new QsiAllColumnNode()
                    }
                });

                n.Source.SetValue(VisitQueryExpression(derivedTableExpression.QueryExpression));
            });
        }

        private static QsiColumnNode VisitSelectScalarExpression(SqlSelectScalarExpression scalarExpression)
        {
            QsiExpressionNode expression = null;
            QsiDeclaredColumnNode column = null;

            switch (scalarExpression.Expression)
            {
                case SqlScalarRefExpression refExpression:
                    column = new QsiDeclaredColumnNode
                    {
                        Name = IdentifierVisitor.VisitMultipartIdentifier(refExpression.MultipartIdentifier)
                    };

                    if (scalarExpression.Alias == null)
                        return column;

                    break;

                default:
                    expression = ExpressionVisitor.VisitScalarExpression(scalarExpression.Expression);
                    break;
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

                if (scalarExpression.Alias != null)
                {
                    n.Alias.SetValue(new QsiAliasNode
                    {
                        Name = new QsiIdentifier(scalarExpression.Alias.Value, false)
                    });
                }
            });
        }

        public static QsiColumnNode VisitStarExpression(SqlSelectStarExpression starExpression)
        {
            return TreeHelper.Create<QsiAllColumnNode>(n =>
            {
                if (starExpression.Qualifier != null)
                {
                    n.Path = IdentifierVisitor.VisitMultipartIdentifier(starExpression.Qualifier);
                }
            });
        }
        #endregion

        #region Columns
        public static IEnumerable<QsiSequentialColumnNode> CreateSequentialColumnNodes(SqlIdentifierCollection identifierCollection)
        {
            return identifierCollection
                .Select((identifier, i) => TreeHelper.Create<QsiSequentialColumnNode>(n =>
                {
                    n.Ordinal = i;
                    n.Alias.SetValue(CreateAliasNode(identifier));
                }));
        }
        #endregion

        #region Alias
        public static QsiAliasNode CreateAliasNode(SqlIdentifier identifier)
        {
            return new QsiAliasNode
            {
                Name = IdentifierVisitor.CreateIdentifier(identifier)
            };
        }
        #endregion

        #region CreateView Statements
        public static QsiTableNode VisitCreateViewStatement(SqlCreateViewStatement createViewStatement)
        {
            return VisitViewDefinition(createViewStatement.Definition);
        }

        public static QsiTableNode VisitViewDefinition(SqlViewDefinition viewDefinition)
        {
            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                var columnsDeclaration = new QsiColumnsDeclarationNode();

                if (viewDefinition.ColumnList == null || viewDefinition.ColumnList.Count == 0)
                {
                    columnsDeclaration.Columns.Add(new QsiAllColumnNode());
                }
                else
                {
                    columnsDeclaration.Columns.AddRange(CreateSequentialColumnNodes(viewDefinition.ColumnList));
                }

                n.Columns.SetValue(columnsDeclaration);
                n.Source.SetValue(VisitQueryExpression(viewDefinition.QueryExpression));

                n.Alias.SetValue(new QsiAliasNode
                {
                    Name = IdentifierVisitor.VisitMultipartIdentifier(viewDefinition.Name)[^1]
                });
            });
        }
        #endregion
    }
}

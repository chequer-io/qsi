using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;
using Qsi.Data;
using Qsi.MySql.Internal;
using Qsi.MySql.Tree.Common;
using Qsi.Tree;
using Qsi.Utilities;
using static Qsi.MySql.Internal.MySqlParser;

namespace Qsi.MySql.Tree
{
    internal static class TableVisitor
    {
        #region Tree
        public static IEnumerable<QsiTableNode> Visit(IParseTree tree)
        {
            switch (tree)
            {
                case RootContext rootContext:
                    return VisitRoot(rootContext);

                case SqlStatementContext statementContext:
                    return new[] { VisitSqlStatement(statementContext) };

                case SelectStatementContext selectStatementContext:
                    return new[] { VisitSelectStatement(selectStatementContext) };
            }

            return Enumerable.Empty<QsiTableNode>();
        }

        public static IEnumerable<QsiTableNode> VisitRoot(RootContext context)
        {
            if (context.sqlStatements() == null)
                yield break;

            foreach (var statementContext in context.sqlStatements().sqlStatement())
            {
                var r = VisitSqlStatement(statementContext);

                if (r != null)
                    yield return r;
            }
        }

        public static QsiTableNode VisitSqlStatement(SqlStatementContext context)
        {
            if (context.children.Count == 0)
                return null;

            switch (context.children[0])
            {
                case DmlStatementContext dmlStatementContext:
                    return VisitDmlStatement(dmlStatementContext);

                case DdlStatementContext ddlStatementContext:
                    return VisitDdlStatement(ddlStatementContext);
            }

            return null;
        }

        internal static QsiTableNode VisitDmlStatement(DmlStatementContext context)
        {
            if (context.selectStatement() != null)
            {
                return VisitSelectStatement(context.selectStatement());
            }

            return null;
        }

        public static QsiTableNode VisitDdlStatement(DdlStatementContext context)
        {
            if (context.createView() != null)
            {
                return VisitCreateView(context.createView());
            }

            return null;
        }
        #endregion

        #region Columns
        public static IEnumerable<QsiSequentialColumnNode> CreateSequentialColumnNodes(IEnumerable<UidContext> uids)
        {
            return uids
                .Select((uid, i) => TreeHelper.Create<QsiSequentialColumnNode>(n =>
                {
                    n.Ordinal = i;
                    n.Alias.SetValue(CreateAliasNode(uid));
                    MySqlTree.PutContextSpan(n, uid);
                }));
        }
        #endregion

        #region Alias
        public static QsiAliasNode CreateAliasNode(UidContext context)
        {
            var node = new QsiAliasNode
            {
                Name = IdentifierVisitor.VisitUid(context)
            };

            MySqlTree.PutContextSpan(node, context);

            return node;
        }
        #endregion

        #region Create View Statement
        public static QsiTableNode VisitCreateView(CreateViewContext context)
        {
            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                var viewIdentifier = IdentifierVisitor.VisitFullId(context.fullId());
                var columnsDeclaration = new QsiColumnsDeclarationNode();

                if (context.uidList() != null)
                {
                    columnsDeclaration.Columns.AddRange(CreateSequentialColumnNodes(context.uidList().uid()));
                    MySqlTree.PutContextSpan(columnsDeclaration, context.uidList());
                }
                else
                {
                    columnsDeclaration.Columns.Add(new QsiAllColumnNode());
                }

                n.Columns.SetValue(columnsDeclaration);
                n.Source.SetValue(VisitSelectStatement(context.selectStatement()));

                n.Alias.SetValue(new QsiAliasNode
                {
                    Name = viewIdentifier[^1]
                });

                MySqlTree.PutContextSpan(n, context);
            });
        }
        #endregion

        #region Select Statements
        internal static QsiTableNode VisitSelectStatement(SelectStatementContext context)
        {
            WithClauseContext withClauseContext = null;
            QsiTableNode tableNode = null;

            switch (context)
            {
                case SimpleSelectContext simpleSelectContext:
                    withClauseContext = simpleSelectContext.withClause();
                    tableNode = VisitSimpleSelect(simpleSelectContext);
                    break;

                case ParenthesisSelectContext parenthesisSelect:
                    withClauseContext = parenthesisSelect.withClause();
                    tableNode = VisitParenthesisSelect(parenthesisSelect);
                    break;

                case UnionSelectContext unionSelect:
                    withClauseContext = unionSelect.withClause();
                    tableNode = VisitUnionSelect(unionSelect);
                    break;

                case UnionParenthesisSelectContext unionParenthesisSelect:
                    withClauseContext = unionParenthesisSelect.withClause();
                    tableNode = VisitUnionParenthesisSelect(unionParenthesisSelect);
                    break;
            }

            if (withClauseContext == null)
            {
                return tableNode;
            }

            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                n.Source.SetValue(tableNode);
                n.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
                n.Directives.SetValue(VisitWithClause(withClauseContext));

                // TODO: alias test

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiTableDirectivesNode VisitWithClause(WithClauseContext context)
        {
            return TreeHelper.Create<QsiTableDirectivesNode>(n =>
            {
                n.IsRecursive = context.RECURSIVE() != null;
                n.Tables.AddRange(context.withExpression().Select(VisitWithExpression));

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiDerivedTableNode VisitWithExpression(WithExpressionContext context)
        {
            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                var columnsDeclaration = new QsiColumnsDeclarationNode();

                if (context.columns != null)
                {
                    columnsDeclaration.Columns.AddRange(CreateSequentialColumnNodes(context.columns.uid()));
                    MySqlTree.PutContextSpan(columnsDeclaration, context.columns);
                }
                else
                {
                    columnsDeclaration.Columns.Add(new QsiAllColumnNode());
                }

                n.Columns.SetValue(columnsDeclaration);
                n.Source.SetValue(VisitSelectStatement(context.selectStatement()));
                n.Alias.SetValue(CreateAliasNode(context.alias));

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiTableNode VisitSimpleSelect(SimpleSelectContext context)
        {
            return VisitQuerySpecification(context.querySpecification());
        }

        public static QsiTableNode VisitParenthesisSelect(ParenthesisSelectContext context)
        {
            return VisitQueryExpression(context.queryExpression());
        }

        public static QsiTableNode VisitUnionSelect(UnionSelectContext context)
        {
            return TreeHelper.Create<QsiCompositeTableNode>(n =>
            {
                n.Sources.Add(VisitQuerySpecificationNointo(context.querySpecificationNointo()));
                n.Sources.AddRange(context.unionStatement().Select(VisitUnionStatement));

                if (context.querySpecification() != null)
                    n.Sources.Add(VisitQuerySpecification(context.querySpecification()));

                if (context.queryExpression() != null)
                    n.Sources.Add(VisitQueryExpression(context.queryExpression()));

                if (context.orderByClause() != null)
                    n.OrderExpression.SetValue(ExpressionVisitor.VisitOrderByClause(context.orderByClause()));

                if (context.limitClause() != null)
                    n.LimitExpression.SetValue(ExpressionVisitor.VisitLimitClause(context.limitClause()));

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiTableNode VisitUnionParenthesisSelect(UnionParenthesisSelectContext context)
        {
            return TreeHelper.Create<QsiCompositeTableNode>(n =>
            {
                n.Sources.Add(VisitQueryExpressionNointo(context.queryExpressionNointo()));
                n.Sources.AddRange(context.unionParenthesis().Select(VisitUnionParenthesis));

                if (context.queryExpression() != null)
                    n.Sources.Add(VisitQueryExpression(context.queryExpression()));

                if (context.orderByClause() != null)
                    n.OrderExpression.SetValue(ExpressionVisitor.VisitOrderByClause(context.orderByClause()));

                if (context.limitClause() != null)
                    n.LimitExpression.SetValue(ExpressionVisitor.VisitLimitClause(context.limitClause()));

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiTableNode VisitUnionStatement(UnionStatementContext context)
        {
            if (context.querySpecificationNointo() != null)
                return VisitQuerySpecificationNointo(context.querySpecificationNointo());

            if (context.queryExpressionNointo() != null)
                return VisitQueryExpressionNointo(context.queryExpressionNointo());

            return null;
        }

        public static QsiTableNode VisitUnionParenthesis(UnionParenthesisContext context)
        {
            return VisitQueryExpressionNointo(context.queryExpressionNointo());
        }
        #endregion

        #region Select Query Specification
        public static QsiTableNode VisitQueryExpression(QueryExpressionContext context)
        {
            while (true)
            {
                if (context.querySpecification() != null)
                    return VisitQuerySpecification(context.querySpecification());

                if (context.queryExpression() != null)
                {
                    context = context.queryExpression();
                    continue;
                }

                return null;
            }
        }

        public static QsiTableNode VisitQuerySpecification(QuerySpecificationContext context)
        {
            return VisitCommonSelect(new CommonSelectContext(context));
        }

        public static QsiTableNode VisitQueryExpressionNointo(QueryExpressionNointoContext context)
        {
            while (true)
            {
                if (context.querySpecificationNointo() != null)
                    return VisitQuerySpecificationNointo(context.querySpecificationNointo());

                if (context.queryExpressionNointo() != null)
                {
                    context = context.queryExpressionNointo();
                    continue;
                }

                throw TreeHelper.NotSupportedTree(context);
            }
        }

        public static QsiTableNode VisitQuerySpecificationNointo(QuerySpecificationNointoContext context)
        {
            return VisitCommonSelect(new CommonSelectContext(context));
        }
        #endregion

        #region Select Elements
        public static QsiTableNode VisitCommonSelect(in CommonSelectContext context)
        {
            var node = new QsiDerivedTableNode();

            node.Columns.SetValue(VisitSelectElements(context.SelectElements));

            if (context.FromClause != null)
            {
                node.Source.SetValue(VisitTableSources(context.FromClause.tableSources()));

                if (context.FromClause.whereExpr != null)
                {
                    var whereContext = new CommonWhereContext(context.FromClause);
                    node.WhereExpression.SetValue(ExpressionVisitor.VisitCommonWhere(whereContext));
                }
            }

            if (context.OrderByClause != null)
            {
                node.OrderExpression.SetValue(ExpressionVisitor.VisitOrderByClause(context.OrderByClause));
            }

            if (context.LimitClause != null)
            {
                node.LimitExpression.SetValue(ExpressionVisitor.VisitLimitClause(context.LimitClause));
            }

            MySqlTree.PutContextSpan(node, context.Context);

            return node;
        }

        public static QsiColumnsDeclarationNode VisitSelectElements(SelectElementsContext context)
        {
            return TreeHelper.Create<QsiColumnsDeclarationNode>(n =>
            {
                if (context.star != null)
                    n.Columns.Add(new QsiAllColumnNode());

                n.Columns.AddRange(context.selectElement().Select(VisitSelectElement));

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiColumnNode VisitSelectElement(SelectElementContext context)
        {
            QsiColumnNode node;

            switch (context)
            {
                case SelectStarElementContext starElementContext:
                {
                    node = new QsiAllColumnNode
                    {
                        Path = IdentifierVisitor.VisitFullId(starElementContext.fullId())
                    };

                    break;
                }

                case SelectColumnElementContext columnElementContext:
                {
                    node = VisitSelectColumnElement(columnElementContext);
                    break;
                }

                case SelectFunctionElementContext functionElementContext:
                {
                    node = TreeHelper.Create<QsiDerivedColumnNode>(n =>
                    {
                        n.Expression.SetValue(ExpressionVisitor.VisitFunctionCall(functionElementContext.functionCall()));

                        if (functionElementContext.alias != null)
                            n.Alias.SetValue(CreateAliasNode(functionElementContext.alias));
                    });

                    break;
                }

                case SelectExpressionElementContext expressionElementContext:
                {
                    var expression = ExpressionVisitor.VisitExpression(expressionElementContext.expression());

                    if (expressionElementContext.localIdAssign() != null)
                    {
                        expression = ExpressionVisitor.VisitLocalIdAssign(
                            expressionElementContext.localIdAssign(),
                            expression);
                    }

                    node = TreeHelper.Create<QsiDerivedColumnNode>(n =>
                    {
                        n.Expression.SetValue(expression);

                        if (expressionElementContext.alias != null)
                            n.Alias.SetValue(CreateAliasNode(expressionElementContext.alias));
                    });

                    break;
                }

                default:
                    return null;
            }

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiColumnNode VisitSelectColumnElement(SelectColumnElementContext columnElementContext)
        {
            var nameContext = columnElementContext.fullColumnName();
            var columnName = IdentifierVisitor.Visit(nameContext);

            if (columnName.Level == 1 &&
                columnName[0].IsEscaped &&
                columnName[0].Value[0] != '`')
            {
                return TreeHelper.Create<QsiDerivedColumnNode>(n =>
                {
                    n.Expression.SetValue(new QsiLiteralExpressionNode
                    {
                        Value = IdentifierUtility.Unescape(columnName[0].Value),
                        Type = QsiDataType.String
                    });

                    if (columnElementContext.alias != null)
                        n.Alias.SetValue(CreateAliasNode(columnElementContext.alias));

                    MySqlTree.PutContextSpan(n, columnElementContext);
                });
            }

            var columnNode = new QsiDeclaredColumnNode
            {
                Name = columnName
            };

            if (columnElementContext.alias == null)
            {
                MySqlTree.PutContextSpan(columnNode, columnElementContext);
                return columnNode;
            }

            MySqlTree.PutContextSpan(columnNode, nameContext);

            return TreeHelper.Create<QsiDerivedColumnNode>(n =>
            {
                n.Column.SetValue(columnNode);
                n.Alias.SetValue(CreateAliasNode(columnElementContext.alias));

                MySqlTree.PutContextSpan(n, columnElementContext);
            });
        }
        #endregion

        #region From Clause
        public static QsiTableNode VisitTableSources(TableSourcesContext context)
        {
            QsiTableNode[] sources = context.tableSource()
                .Select(VisitTableSource)
                .ToArray();

            if (sources.Length == 1)
            {
                return sources[0];
            }

            if (sources.Length > 1)
            {
                // comma join

                var anchor = sources[0];

                foreach (var source in sources.Skip(1))
                {
                    var nextJoin = new QsiJoinedTableNode
                    {
                        JoinType = QsiJoinType.Comma
                    };

                    nextJoin.Left.SetValue(anchor);
                    nextJoin.Right.SetValue(source);

                    var leftSpan = MySqlTree.GetSpan(anchor);
                    var rightSpan = MySqlTree.GetSpan(source);

                    anchor = nextJoin;

                    MySqlTree.PutSpan(nextJoin, new Range(leftSpan.Start, rightSpan.End));
                }

                return anchor;
            }

            return null;
        }

        public static QsiTableNode VisitTableSource(TableSourceContext context)
        {
            switch (context)
            {
                case TableSourceBaseContext baseContext:
                {
                    return VisitTableSource(new CommonTableSourceContext(baseContext));
                }

                case TableSourceNestedContext nestedContext:
                {
                    return TreeHelper.Create<QsiDerivedTableNode>(n =>
                    {
                        n.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
                        n.Source.SetValue(VisitTableSource(new CommonTableSourceContext(nestedContext)));

                        MySqlTree.PutContextSpan(n, nestedContext);
                    });
                }

                default:
                {
                    throw new NotSupportedException();
                }
            }
        }

        public static QsiTableNode VisitTableSourceItem(TableSourceItemContext context)
        {
            switch (context)
            {
                case AtomTableItemContext atomTableItemContext:
                    return VisitAtomTableItem(atomTableItemContext);

                case SubqueryTableItemContext subqueryTableItemContext:
                    return VisitSubqueryTableItem(subqueryTableItemContext);

                case TableSourcesItemContext tableSourcesItemContext:
                    return VisitTableSourcesItem(tableSourcesItemContext);

                default:
                    throw new NotSupportedException();
            }
        }

        public static QsiTableNode VisitTableSource(CommonTableSourceContext context)
        {
            var anchor = VisitTableSourceItem(context.TableSourceItem);

            if (context.JoinPart?.Length > 0)
            {
                foreach (var join in context.JoinPart)
                {
                    QsiJoinType joinType;
                    TableSourceItemContext sourceItemContext;
                    UidListContext uidListContext = null;

                    switch (join)
                    {
                        case InnerJoinContext innerJoinContext:
                            joinType = innerJoinContext.INNER() != null ? QsiJoinType.Inner : QsiJoinType.Cross;
                            sourceItemContext = innerJoinContext.tableSourceItem();
                            uidListContext = innerJoinContext.uidList();
                            break;

                        case StraightJoinContext straightJoinContext:
                            joinType = QsiJoinType.Straight;
                            sourceItemContext = straightJoinContext.tableSourceItem();
                            break;

                        case OuterJoinContext outerJoinContext:
                        {
                            var joinOuter = outerJoinContext.joinOuter();
                            var isLeft = joinOuter.joinType.Type == MySqlLexer.LEFT;

                            if (joinOuter.OUTER() == null)
                                joinType = isLeft ? QsiJoinType.Left : QsiJoinType.Right;
                            else
                                joinType = isLeft ? QsiJoinType.LeftOuter : QsiJoinType.RightOuter;

                            sourceItemContext = outerJoinContext.tableSourceItem();
                            uidListContext = outerJoinContext.uidList();
                            break;
                        }

                        case NaturalJoinContext naturalJoinContext:
                        {
                            var joinOuter = naturalJoinContext.joinOuter();
                            var isLeft = joinOuter.joinType.Type == MySqlLexer.LEFT;

                            if (joinOuter.OUTER() == null)
                                joinType = isLeft ? QsiJoinType.NaturalLeft : QsiJoinType.NaturalRight;
                            else
                                joinType = isLeft ? QsiJoinType.NaturalLeftOuter : QsiJoinType.NaturalRightOuter;

                            sourceItemContext = naturalJoinContext.tableSourceItem();
                            break;
                        }

                        default:
                            throw new NotSupportedException();
                    }

                    var nextJoin = new QsiJoinedTableNode
                    {
                        JoinType = joinType
                    };

                    nextJoin.Left.SetValue(anchor);
                    nextJoin.Right.SetValue(VisitTableSourceItem(sourceItemContext));

                    var leftSpan = MySqlTree.GetSpan(anchor);
                    MySqlTree.PutSpan(nextJoin, new Range(leftSpan.Start, join.Stop.StopIndex + 1));

                    anchor = nextJoin;

                    // PivotColumns: USING (..)
                    if (uidListContext != null)
                    {
                        IEnumerable<QsiDeclaredColumnNode> columns = uidListContext.uid()
                            .Select(uid => new QsiDeclaredColumnNode
                            {
                                Name = new QsiQualifiedIdentifier(IdentifierVisitor.VisitUid(uid))
                            });

                        var columnsDeclaration = TreeHelper.Create<QsiColumnsDeclarationNode>(dn =>
                        {
                            dn.Columns.AddRange(columns);
                        });

                        nextJoin.PivotColumns.SetValue(columnsDeclaration);

                        MySqlTree.PutContextSpan(columnsDeclaration, uidListContext);
                    }
                }
            }

            return anchor;
        }

        public static QsiTableAccessNode VisitTableName(TableNameContext context)
        {
            return new QsiTableAccessNode
            {
                Identifier = IdentifierVisitor.VisitFullId(context.fullId())
            };
        }

        // .. FROM db.table [AS alias]
        public static QsiTableNode VisitAtomTableItem(AtomTableItemContext context)
        {
            var tableNode = VisitTableName(context.tableName());

            if (context.alias == null)
            {
                MySqlTree.PutContextSpan(tableNode, context);
                return tableNode;
            }

            MySqlTree.PutContextSpan(tableNode, context.tableName());

            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                n.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
                n.Source.SetValue(tableNode);
                n.Alias.SetValue(CreateAliasNode(context.alias));

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiTableAccessNode VisitFullId(FullIdContext context)
        {
            var node = new QsiTableAccessNode
            {
                Identifier = IdentifierVisitor.VisitFullId(context)
            };

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        // .. FROM (subquery) alias
        public static QsiTableNode VisitSubqueryTableItem(SubqueryTableItemContext context)
        {
            if (context.alias == null)
                throw new QsiException(QsiError.NoAlias);

            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                n.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
                n.Source.SetValue(VisitSelectStatement(context.selectStatement()));
                n.Alias.SetValue(CreateAliasNode(context.alias));

                MySqlTree.PutContextSpan(n, context);
            });
        }

        // .. FROM (sources, ..)
        public static QsiTableNode VisitTableSourcesItem(TableSourcesItemContext context)
        {
            return VisitTableSources(context.tableSources());
        }
        #endregion
    }
}

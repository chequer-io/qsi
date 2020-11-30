using System.Linq;
using Qsi.Tree;
using Qsi.Utilities;
using static PrimarSql.Internal.PrimarSqlParser;

namespace Qsi.PrimarSql.Tree
{
    internal static class TableVisitor
    {
        public static QsiTableNode VisitRoot(RootContext context)
        {
            return VisitSqlStatement(context.sqlStatement());
        }

        public static QsiTableNode VisitSqlStatement(SqlStatementContext context)
        {
            if (context.dmlStatement() != null)
                return VisitDmlStatement(context.dmlStatement());

            return null;
        }

        public static QsiTableNode VisitDmlStatement(DmlStatementContext context)
        {
            if (context.selectStatement() != null)
                return VisitSelectStatement(context.selectStatement());

            return null;
        }

        public static QsiTableNode VisitSelectStatement(SelectStatementContext context)
        {
            switch (context)
            {
                case SimpleSelectContext simpleSelectContext:
                    return VisitQuerySpecification(simpleSelectContext.querySpecification());

                case ParenthesisSelectContext parenthesisSelectContext:
                    return VisitQueryExpression(parenthesisSelectContext.queryExpression());
            }

            return null;
        }

        public static QsiTableNode VisitQueryExpression(QueryExpressionContext context)
        {
            if (context.querySpecification() != null)
                return VisitQuerySpecification(context.querySpecification());

            return VisitQueryExpression(context.queryExpression());
        }

        public static QsiTableNode VisitQuerySpecification(QuerySpecificationContext context)
        {
            var node = new PrimarSqlDerivedTableNode
            {
                SelectSpec = VisitSelectSpec(context.selectSpec())
            };

            node.Columns.SetValue(VisitSelectElements(context.selectElements()));

            if (context.fromClause() != null)
            {
                node.Source.SetValue(VisitFromCluase(context.fromClause()));

                if (context.fromClause().whereExpr != null)
                {
                    node.Where.SetValue(VisitExpression(context.fromClause().whereExpr));
                }
            }
            
            PrimarSqlTree.PutContextSpan(node, context);

            return node;
        }

        public static SelectSpec VisitSelectSpec(SelectSpecContext context)
        {
            if (context == null)
                return SelectSpec.Empty;

            if (context.STRONGLY() != null)
                return SelectSpec.Strongly;

            if (context.EVENTUALLY() != null)
                return SelectSpec.Eventually;

            return SelectSpec.Empty;
        }

        public static QsiColumnsDeclarationNode VisitSelectElements(SelectElementsContext context)
        {
            return TreeHelper.Create<QsiColumnsDeclarationNode>(n =>
            {
                if (context.star != null)
                    n.Columns.Add(new QsiAllColumnNode());

                n.Columns.AddRange(context.selectElement().Select(VisitSelectElement));
            });
        }

        public static QsiColumnNode VisitSelectElement(SelectElementContext context)
        {
            QsiColumnNode node;

            switch (context)
            {
                case SelectColumnElementContext columnElementContext:
                {
                    node = VisitSelectColumnElement(columnElementContext);
                    break;
                }
                
                case SelectFunctionElementContext _:
                    throw TreeHelper.NotSupportedFeature("Select Element Function");

                case SelectExpressionElementContext _:
                    throw TreeHelper.NotSupportedFeature("Select Element Expression");
                
                default:
                {
                    node = null;
                    break;
                }
            }

            return node;
        }

        public static QsiColumnNode VisitSelectColumnElement(SelectColumnElementContext context)
        {
            var nameContext = context.fullColumnName();
            var column = IdentifierVisitor.VisitFullColumnName(nameContext);

            if (context.alias == null)
                return column;

            return TreeHelper.Create<QsiDerivedColumnNode>(n =>
            {
                n.Column.SetValue(column);
                n.Alias.SetValue(CreateAliasNode(context.alias));
                PrimarSqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiTableNode VisitFromCluase(FromClauseContext context)
        {
            return VisitTableSource(context.tableSource());
        }

        public static QsiTableNode VisitTableSource(TableSourceContext context)
        {
            switch (context)
            {
                case TableSourceBaseContext tableSourceBaseContext:
                    return VisitTableSourceItem(tableSourceBaseContext.tableSourceItem());

                case TableSourceNestedContext tableSourceNestedContext:
                    return VisitTableSourceItem(tableSourceNestedContext.tableSourceItem());
            }

            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiTableNode VisitTableSourceItem(TableSourceItemContext context)
        {
            switch (context)
            {
                case AtomTableItemContext atomTableItemContext:
                    return VisitAtomTableItem(atomTableItemContext);

                case SubqueryTableItemContext subqueryTableItemContext:
                    return VisitSubqueryTableItem(subqueryTableItemContext);
            }

            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiTableNode VisitAtomTableItem(AtomTableItemContext context)
        {
            QsiTableNode node = TreeHelper.Create<QsiTableAccessNode>(n =>
            {
                n.Identifier = IdentifierVisitor.VisitFullId(context.tableName().fullId());
            });

            if (context.alias == null)
                return node;

            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                n.Alias.SetValue(CreateAliasNode(context.alias));

                n.Source.SetValue(node);
            });
        }

        public static QsiTableNode VisitSubqueryTableItem(SubqueryTableItemContext context)
        {
            if (context.alias == null)
                throw new QsiException(QsiError.NoAlias);

            return TreeHelper.Create<QsiDerivedTableNode>(n =>
            {
                n.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
                n.Source.SetValue(VisitSelectStatement(context.selectStatement()));
                n.Alias.SetValue(CreateAliasNode(context.alias));
                
                PrimarSqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiWhereExpressionNode VisitExpression(ExpressionContext context)
        {
            return TreeHelper.Create<QsiWhereExpressionNode>(n =>
            {
                n.Expression.SetValue(ExpressionVisitor.VisitExpression(context));
            });
        }

        #region Alias
        public static QsiAliasNode CreateAliasNode(UidContext context)
        {
            var node = new QsiAliasNode
            {
                Name = IdentifierVisitor.VisitUid(context)
            };

            PrimarSqlTree.PutContextSpan(node, context);

            return node;
        }
        #endregion
    }
}

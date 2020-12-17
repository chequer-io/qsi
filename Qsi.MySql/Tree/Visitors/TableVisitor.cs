using System.Linq;
using Antlr4.Runtime.Tree;
using Qsi.MySql.Data;
using Qsi.MySql.Tree.Common;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Utilities;
using static Qsi.MySql.Internal.MySqlParserInternal;

namespace Qsi.MySql.Tree
{
    internal static class TableVisitor
    {
        public static QsiTableNode VisitSelectStatement(SelectStatementContext context)
        {
            switch (context.children[0])
            {
                case QueryExpressionContext queryExpression:
                    var commonContext = new CommonQueryContext(queryExpression, context.lockingClauseList());
                    return VisitQueryExpression(commonContext);

                case QueryExpressionParensContext queryExpressionParens:
                    return VisitQueryExpressionParens(queryExpressionParens);

                case SelectStatementWithIntoContext selectStatementWithInto:
                    return VisitSelectStatementWithInto(selectStatementWithInto);

                default:
                    throw TreeHelper.NotSupportedTree(context.children[0]);
            }
        }

        public static QsiTableNode VisitQueryExpression(in CommonQueryContext context)
        {
            var source = context.Source switch
            {
                QueryExpressionBodyContext c => VisitQueryExpressionBody(c),
                QueryExpressionParensContext c => VisitQueryExpressionParens(c),
                FromClauseContext c => VisitFromClause(c),
                _ => throw TreeHelper.NotSupportedTree(context.Source)
            };

            var contexts = new object[]
            {
                context.WithClause,
                context.SelectOptions,
                context.SelectItemList,
                context.IntoClause,
                context.WhereClause,
                context.GroupByClause,
                context.HavingClause,
                context.WindowClause,
                context.OrderClause,
                context.LimitClause,
                context.ProcedureAnalyseClause,
                context.LockingClauseList
            };

            if (contexts.All(c => c == null))
                return source;

            var node = new MySqlDerivedTableNode();

            if (context.WithClause != null)
                node.Directives.SetValue(VisitWithClause(context.WithClause));

            if (!ListUtility.IsNullOrEmpty(context.SelectOptions))
                node.SelectOptions.AddRange(context.SelectOptions.Select(VisitSelectOption));

            node.Columns.SetValue(context.SelectItemList != null ?
                VisitSelectItemList(context.SelectItemList) :
                TreeHelper.CreateAllColumnsDeclaration());

            // TODO: context.IntoClause

            node.Source.SetValue(source);

            if (context.WhereClause != null)
                node.Where.SetValue(ExpressionVisitor.VisitWhereClause(context.WhereClause));

            if (context.GroupByClause != null)
            {
                node.Grouping.SetValue(ExpressionVisitor.VisitGroupByClause(context.GroupByClause));

                if (context.HavingClause != null)
                    node.Grouping.Value.Having.SetValue(ExpressionVisitor.VisitHavingClause(context.HavingClause));
            }

            // TODO: context.WindowClause

            if (context.OrderClause != null)
                node.Order.SetValue(ExpressionVisitor.VisitOrderClause(context.OrderClause));

            if (context.LimitClause != null)
                node.Limit.SetValue(ExpressionVisitor.VisitLimitClause(context.LimitClause));

            if (context.ProcedureAnalyseClause != null)
                node.ProcedureAnalyse.SetValue(VisitProcedureAnalyseClause(context.ProcedureAnalyseClause));

            if (context.LockingClauseList != null)
            {
                LockingClauseContext[] lockingClauses = context.LockingClauseList.lockingClause();
                node.Lockings.AddRange(lockingClauses.Select(VisitLockingClause));
            }

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static MySqlSelectOptionNode VisitSelectOption(SelectOptionContext context)
        {
            throw new System.NotImplementedException();
        }

        public static QsiColumnsDeclarationNode VisitSelectItemList(SelectItemListContext context)
        {
            throw new System.NotImplementedException();
        }

        public static QsiTableNode VisitFromClause(FromClauseContext context)
        {
            throw new System.NotImplementedException();
        }

        public static QsiTableNode VisitQueryExpressionBody(QueryExpressionBodyContext context)
        {
            QsiTableNode[] sources = context.children
                .Where(c => c is QueryPrimaryContext || c is QueryExpressionParensContext)
                .Select(c =>
                {
                    if (c is QueryExpressionParensContext queryExpressionParens)
                        return VisitQueryExpressionParens(queryExpressionParens);

                    return VisitQueryPrimary((QueryPrimaryContext)c);
                })
                .ToArray();

            if (sources.Length == 1)
                return sources[0];

            var node = new QsiCompositeTableNode();

            node.Sources.AddRange(sources);
            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiTableNode VisitQueryPrimary(QueryPrimaryContext context)
        {
            switch (context.children[0])
            {
                case QuerySpecificationContext querySpecification:
                    return VisitQuerySpecification(querySpecification);

                case TableValueConstructorContext tableValueConstructor:
                    return VisitTableValueConstructor(tableValueConstructor);

                case ExplicitTableContext explicitTable:
                    return VisitExplicitTable(explicitTable);

                default:
                    throw TreeHelper.NotSupportedTree(context.children[0]);
            }
        }

        public static QsiTableNode VisitQuerySpecification(QuerySpecificationContext context)
        {
            throw new System.NotImplementedException();
        }

        public static QsiTableNode VisitTableValueConstructor(TableValueConstructorContext context)
        {
            var node = new QsiInlineDerivedTableNode();

            node.Rows.AddRange(context.rowValueExplicit().Select(VisitRowValueExplicit));
            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiRowValueExpressionNode VisitRowValueExplicit(RowValueExplicitContext context)
        {
            var node = new QsiRowValueExpressionNode();

            foreach (var value in context.values().children)
            {
                switch (value)
                {
                    case ITerminalNode terminalNode when terminalNode.Symbol.Type == COMMA_SYMBOL:
                        continue;

                    case ExprContext expr:
                        node.ColumnValues.Add(ExpressionVisitor.VisitExpr(expr));
                        break;

                    default:
                        node.ColumnValues.Add(TreeHelper.CreateDefaultLiteral());
                        break;
                }
            }

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiTableNode VisitExplicitTable(ExplicitTableContext context)
        {
            var node = new MySqlExplicitTableAccessNode
            {
                Identifier = IdentifierVisitor.VisitTableRef(context.tableRef())
            };

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QueryExpressionParensContext UnwrapQueryExpressionParens(QueryExpressionParensContext context)
        {
            do
            {
                var nestedContext = context.queryExpressionParens();

                if (nestedContext == null)
                    break;

                context = nestedContext;
            } while (true);

            return context;
        }

        public static QsiTableNode VisitQueryExpressionParens(QueryExpressionParensContext context)
        {
            var parens = UnwrapQueryExpressionParens(context);
            var commonContext = new CommonQueryContext(parens.queryExpression(), parens.lockingClauseList());
            var node = VisitQueryExpression(commonContext);

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiTableNode VisitSelectStatementWithInto(SelectStatementWithIntoContext context)
        {
            throw new System.NotImplementedException();
        }

        public static QsiTableDirectivesNode VisitWithClause(WithClauseContext context)
        {
            var node = new QsiTableDirectivesNode
            {
                IsRecursive = context.HasToken(RECURSIVE_SYMBOL)
            };

            node.Tables.AddRange(context.commonTableExpression().Select(VisitCommonTableExpression));

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiDerivedTableNode VisitCommonTableExpression(CommonTableExpressionContext context)
        {
            var node = new QsiDerivedTableNode();

            node.Alias.SetValue(new QsiAliasNode
            {
                Name = IdentifierVisitor.VisitIdentifier(context.identifier())
            });

            node.Columns.SetValue(context.columnInternalRefList() == null ?
                TreeHelper.CreateAllColumnsDeclaration() :
                CreateSequentialColumns(context.columnInternalRefList()));

            node.Source.SetValue(VisitSubquery(context.subquery()));

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiColumnsDeclarationNode CreateSequentialColumns(ColumnInternalRefListContext context)
        {
            var node = new QsiColumnsDeclarationNode();

            node.Columns.AddRange(context.columnInternalRef().Select(VisitSequentialColumn));
            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiSequentialColumnNode VisitSequentialColumn(ColumnInternalRefContext context)
        {
            var node = new QsiSequentialColumnNode();

            node.Alias.SetValue(new QsiAliasNode
            {
                Name = IdentifierVisitor.VisitIdentifier(context.identifier())
            });

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiTableNode VisitSubquery(SubqueryContext context)
        {
            return VisitQueryExpressionParens(context.queryExpressionParens());
        }

        public static MySqlProcedureAnalyseNode VisitProcedureAnalyseClause(ProcedureAnalyseClauseContext context)
        {
            var node = new MySqlProcedureAnalyseNode();
            ITerminalNode[] numbers = context.INT_NUMBER();

            node.MaxElements = int.Parse(numbers[0].GetText());

            if (numbers.Length == 2)
                node.MaxMemory = int.Parse(numbers[1].GetText());

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static MySqlLockingNode VisitLockingClause(LockingClauseContext context)
        {
            var node = new MySqlLockingNode();
            var lockStrengh = context.lockStrengh();

            if (lockStrengh != null)
            {
                node.TableLockType = lockStrengh.HasToken(UPDATE_SYMBOL) ?
                    MySqlTableLockType.Update : MySqlTableLockType.Share;

                node.Tables = context.tableAliasRefList()?
                    .tableRefWithWildcard()
                    .Select(IdentifierVisitor.VisitTableRefWithWildcard)
                    .ToArray();

                var lockedRowAction = context.lockedRowAction();

                if (lockedRowAction != null)
                {
                    node.RowLockType = lockedRowAction.HasToken(SKIP_SYMBOL) ?
                        MySqlRowLockType.SkipLocked : MySqlRowLockType.NoWait;
                }
            }
            else
            {
                node.TableLockType = MySqlTableLockType.ShareMode;
            }

            MySqlTree.PutContextSpan(node, context);

            return node;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Antlr4.Runtime.Tree;
using Qsi.Data;
using Qsi.Impala.Internal;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Impala.Tree.Visitors
{
    using static ImpalaParserInternal;

    internal static class TableVisitor
    {
        public static QsiTableNode VisitQueryStmt(Query_stmtContext context)
        {
            var withClause = context.opt_with_clause();
            var setOperandList = context.set_operand_list();

            var node = VisitSetOperandList(setOperandList);

            if (withClause is not null)
            {
                if (node is not QsiDerivedTableNode { Directives: { IsEmpty: true } } derivedTableNode)
                {
                    derivedTableNode = ImpalaTree.CreateWithSpan<QsiDerivedTableNode>(context);
                    derivedTableNode.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();
                    derivedTableNode.Source.Value = node;

                    node = derivedTableNode;
                }

                derivedTableNode.Directives.Value = VisitWithClause(withClause);
            }

            return node;
        }

        public static QsiTableNode VisitSetOperandList(Set_operand_listContext context)
        {
            QsiTableNode[] sources = context._sets
                .Select(VisitSetOperand)
                .ToArray();

            var orderByClause = context.opt_order_by_clause();
            var limitOffsetClauseExpr = ExpressionVisitor.VisitLimitOffsetClause(context.opt_limit_offset_clause());

            if (sources.Length > 1)
            {
                var node = ImpalaTree.CreateWithSpan<QsiCompositeTableNode>(context);

                node.Sources.AddRange(sources);

                if (orderByClause is not null)
                    node.Order.Value = ExpressionVisitor.VisitOrderByClause(orderByClause);

                if (limitOffsetClauseExpr is not null)
                    node.Limit.Value = limitOffsetClauseExpr;

                return node;
            }

            if (orderByClause is not null || limitOffsetClauseExpr is not null)
            {
                if (sources[0] is not QsiDerivedTableNode
                {
                    Order: { IsEmpty: true },
                    Limit: { IsEmpty: true }
                } derivedTableNode)
                {
                    derivedTableNode = new QsiDerivedTableNode
                    {
                        Source = { Value = sources[0] }
                    };

                    sources[0] = derivedTableNode;
                }

                if (orderByClause is not null)
                    derivedTableNode.Order.Value = ExpressionVisitor.VisitOrderByClause(orderByClause);

                if (limitOffsetClauseExpr is not null)
                    derivedTableNode.Limit.Value = limitOffsetClauseExpr;

                ImpalaTree.PutContextSpan(sources[0], context);
            }

            return sources[0];
        }

        public static QsiTableNode VisitSetOperand(Set_operandContext context)
        {
            switch (context.children[0])
            {
                case Values_stmtContext valuesStmt:
                    return VisitValuesStmt(valuesStmt);

                case Select_stmtContext selectStmt:
                    return VisitSelectStmt(selectStmt);

                default:
                    var node = VisitQueryStmt(context.query_stmt());
                    ImpalaTree.PutContextSpan(node, context);
                    return node;
            }
        }

        public static QsiTableNode VisitValuesStmt(Values_stmtContext context)
        {
            var node = ImpalaTree.CreateWithSpan<ImpalaValuesTableNode>(context);

            node.Rows.AddRange(ExpressionVisitor.VisitValuesOperandList(context.values_operand_list()));

            var orderByClause = context.opt_order_by_clause();
            var limitOffsetClauseExpr = ExpressionVisitor.VisitLimitOffsetClause(context.opt_limit_offset_clause());

            if (orderByClause is not null)
                node.Order.Value = ExpressionVisitor.VisitOrderByClause(orderByClause);

            if (limitOffsetClauseExpr is not null)
                node.Limit.Value = limitOffsetClauseExpr;

            return node;
        }

        public static QsiTableNode VisitSelectStmt(Select_stmtContext context)
        {
            var node = ImpalaTree.CreateWithSpan<ImpalaDerivedTableNode>(context);

            if (context.select.children[1] is ITerminalNode { Symbol: { Type: KW_DISTINCT or KW_ALL } })
                node.IsDistinct = context.select.children[1] is ITerminalNode { Symbol: { Type: KW_DISTINCT } };

            // -- select_clause --

            var planHints = context.select.plan_hints();

            if (planHints is not null)
                node.PlanHints = VisitPlanHints(planHints);

            node.Columns.Value = ImpalaTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.select.select_list());

            foreach (var selectItem in ExpressionVisitor.VisitSelectList(context.select.select_list()))
            {
                if (selectItem is QsiColumnExpressionNode columnExpression)
                {
                    node.Columns.Value.Columns.Add(columnExpression.Column.Value);
                }
                else
                {
                    var exprColummNode = new QsiDerivedColumnNode
                    {
                        Expression =
                        {
                            Value = selectItem
                        }
                    };

                    ImpalaTree.Span[exprColummNode] = ImpalaTree.Span[selectItem];

                    node.Columns.Value.Columns.Add(exprColummNode);
                }
            }

            // -- from_clause --

            if (context.from is null)
                return node;

            node.Source.Value = VisitFromClause(context.from);

            if (context.where is not null)
                node.Where.Value = ExpressionVisitor.VisitWhereClause(context.where);

            if (context.groupBy is not null)
                node.Grouping.Value = ExpressionVisitor.VisitGroupByClause(context.groupBy);

            // TODO: Impl after adding QsiHavingExpressionNode
            // if (context.having is not null)
            //     node.Having.Value = ExpressionVisitor.VisitHavingClause(context.having);

            if (context.orderBy is not null)
                node.Order.Value = ExpressionVisitor.VisitOrderByClause(context.orderBy);

            var limitOffsetClauseExpr = ExpressionVisitor.VisitLimitOffsetClause(context.limitOffset);

            if (limitOffsetClauseExpr is not null)
                node.Limit.Value = limitOffsetClauseExpr;

            return node;
        }

        public static QsiTableNode VisitFromClause(From_clauseContext context)
        {
            return VisitTableRefList(context.table_ref_list());
        }

        public static QsiTableNode VisitTableRefList(Table_ref_listContext context)
        {
            switch (context)
            {
                case SingleTableRefContext singleTableRef:
                    return VisitTableRef(singleTableRef.table_ref());

                case CrossJoinContext crossJoin:
                    return VisitCrossJoin(crossJoin);

                case JoinContext join:
                    return VisitJoin(join);

                case CommaJoinContext commaJoin:
                    return VisitCommaJoin(commaJoin);

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QsiTableNode VisitTableRef(Table_refContext context)
        {
            // ImpalaTableReferenceNode | ImpalaDerivedTableNode
            QsiTableNode node;

            if (context.reference is not null)
            {
                node = VisitDottedPath(context.reference);
            }
            else
            {
                var derivedTableNode = ImpalaTree.CreateWithSpan<ImpalaDerivedTableNode>(context);
                derivedTableNode.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();
                derivedTableNode.Source.Value = VisitQueryStmt(context.query);

                node = derivedTableNode;
            }

            if (context.alias is not null)
            {
                if (node is not ImpalaDerivedTableNode derivedTableNode)
                {
                    derivedTableNode = ImpalaTree.CreateWithSpan<ImpalaDerivedTableNode>(context);
                    derivedTableNode.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();
                    derivedTableNode.Source.Value = node;

                    node = derivedTableNode;
                }

                derivedTableNode.Alias.Value = VisitAliasClause(context.alias);
            }

            switch (node)
            {
                case ImpalaTableReferenceNode tableRef:

                    if (context.sample is not null)
                        tableRef.TableSample = VisitTableSample(context.sample);

                    if (context.hint is not null)
                        tableRef.PlanHints = VisitPlanHints(context.hint);

                    break;

                case ImpalaDerivedTableNode derivedTable:

                    if (context.sample is not null)
                        derivedTable.TableSample = VisitTableSample(context.sample);

                    if (context.hint is not null)
                        derivedTable.PlanHints = VisitPlanHints(context.hint);

                    break;
            }

            return node;
        }

        private static ImpalaTableReferenceNode VisitDottedPath(Dotted_pathContext context)
        {
            var node = ImpalaTree.CreateWithSpan<ImpalaTableReferenceNode>(context);
            node.Identifier = IdentifierVisitor.VisitDottedPath(context);
            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QsiTableNode VisitCrossJoin(CrossJoinContext context)
        {
            var node = ImpalaTree.CreateWithSpan<ImpalaJoinedTableNode>(context);

            node.Left.Value = VisitTableRefList(context.left);
            node.JoinType = "CROSS JOIN";
            node.Right.Value = VisitTableRef(context.right);

            if (context.hint is not null)
                node.PlanHints = VisitPlanHints(context.hint);

            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QsiTableNode VisitJoin(JoinContext context)
        {
            var node = ImpalaTree.CreateWithSpan<ImpalaJoinedTableNode>(context);

            node.Left.Value = VisitTableRefList(context.left);
            node.JoinType = string.Join(" ", context.join_operator().children.Select(c => c.GetText()));
            node.Right.Value = VisitTableRef(context.right);

            if (context.hint is not null)
                node.PlanHints = VisitPlanHints(context.hint);

            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QsiTableNode VisitCommaJoin(CommaJoinContext context)
        {
            var node = ImpalaTree.CreateWithSpan<QsiJoinedTableNode>(context);

            node.Left.Value = VisitTableRefList(context.left);
            node.IsComma = true;
            node.Right.Value = VisitTableRef(context.right);

            return node;
        }

        public static QsiTableDirectivesNode VisitWithClause(Opt_with_clauseContext context)
        {
            var node = ImpalaTree.CreateWithSpan<QsiTableDirectivesNode>(context);
            node.Tables.AddRange(VisitWithViewDefList(context.with_view_def_list()));
            return node;
        }

        public static IEnumerable<QsiDerivedTableNode> VisitWithViewDefList(With_view_def_listContext context)
        {
            return context.with_view_def().Select(VisitWithViewDef);
        }

        public static QsiDerivedTableNode VisitWithViewDef(With_view_defContext context)
        {
            var node = ImpalaTree.CreateWithSpan<QsiDerivedTableNode>(context);

            node.Alias.Value = new QsiAliasNode
            {
                Name = context.children[0] switch
                {
                    Ident_or_defaultContext identOrDefault => IdentifierVisitor.VisitIdentOrDefault(identOrDefault),
                    _ => IdentifierVisitor.VisitStringLiteral((ITerminalNode)context.children[0])
                }
            };

            var identList = context.ident_list();

            if (identList is not null)
            {
                node.Columns.Value = new QsiColumnsDeclarationNode();

                node.Columns.Value.Columns.AddRange(
                    IdentifierVisitor.VisitIdentList(identList).Select(i =>
                        new QsiSequentialColumnNode
                        {
                            Alias =
                            {
                                Value = new QsiAliasNode
                                {
                                    Name = i
                                }
                            }
                        }
                    )
                );
            }
            else
            {
                node.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();
            }

            node.Source.SetValue(VisitQueryStmt(context.query_stmt()));

            return node;
        }

        public static QsiColumnNode VisitStarExpr(Star_exprContext context)
        {
            var node = ImpalaTree.CreateWithSpan<QsiAllColumnNode>(context);

            if (context.dotted_path() is not null)
                node.Path = IdentifierVisitor.VisitDottedPath(context.dotted_path());

            return node;
        }

        public static QsiAliasNode VisitAliasClause(Alias_clauseContext context)
        {
            var node = ImpalaTree.CreateWithSpan<QsiAliasNode>(context);
            var identOrDefault = context.ident_or_default();

            node.Name = identOrDefault is not null ?
                IdentifierVisitor.VisitIdentOrDefault(identOrDefault) :
                IdentifierVisitor.VisitStringLiteral(context.STRING_LITERAL());

            return node;
        }

        public static string VisitTableSample(Opt_tablesampleContext context)
        {
            return context.GetInputText();
        }

        public static string VisitPlanHints(Plan_hintsContext context)
        {
            return context.GetInputText();
        }

        public static QsiTableNode VisitSubquery(SubqueryContext context)
        {
            while (context.nest is not null)
                context = context.nest;

            return VisitQueryStmt(context.query_stmt());
        }

        public static ImpalaTableReferenceNode VisitTableName(Table_nameContext context)
        {
            var node = ImpalaTree.CreateWithSpan<ImpalaTableReferenceNode>(context);
            node.Identifier = IdentifierVisitor.VisitTableName(context);
            return node;
        }
    }
}

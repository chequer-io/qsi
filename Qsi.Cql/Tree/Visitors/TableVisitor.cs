using System.Linq;
using Qsi.Cql.Tree.Common;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;
using static Qsi.Cql.Internal.CqlParserInternal;

namespace Qsi.Cql.Tree
{
    internal static class TableVisitor
    {
        #region SelectStatement
        public static QsiTableNode VisitSelectStatement(SelectStatementContext context)
        {
            var node = new CqlDerivedTableNode
            {
                IsJson = context.json,
                IsDistinct = context.distinct,
                AllowFiltering = context.allowFiltering
            };

            node.Columns.SetValue(VisitSelectors(context.selectors()));
            node.Source.SetValue(VisitColumnFamilyName(context.columnFamilyName()));

            if (context.whereClause() != null)
            {
                var clause = context.whereClause();

                var whereContext = new ParserRuleContextWrapper<WhereClauseContext>
                (
                    clause,
                    context.w,
                    clause.Stop
                );

                node.Where.SetValue(ExpressionVisitor.CreateWhere(whereContext));
            }

            if (!ListUtility.IsNullOrEmpty(context.groupByClause()))
            {
                GroupByClauseContext[] clauses = context.groupByClause();

                var groupingContext = new ParserRuleContextWrapper<GroupByClauseContext[]>
                (
                    clauses,
                    context.g,
                    clauses[^1].Stop
                );

                node.Grouping.SetValue(ExpressionVisitor.CreateGrouping(groupingContext));
            }

            if (!ListUtility.IsNullOrEmpty(context.orderByClause()))
            {
                OrderByClauseContext[] clauses = context.orderByClause();

                var orderContext = new ParserRuleContextWrapper<OrderByClauseContext[]>
                (
                    clauses,
                    context.o,
                    clauses[^1].Stop
                );

                node.Order.SetValue(ExpressionVisitor.CreateOrder(orderContext));
            }

            if (context.perLimit != null)
                node.PerPartitionLimit.SetValue(ExpressionVisitor.VisitIntValue(context.perLimit));

            if (context.limit != null)
            {
                var limitContext = new ParserRuleContextWrapper<IntValueContext>
                (
                    context.limit,
                    context.l,
                    context.limit.Stop
                );

                node.Limit.SetValue(ExpressionVisitor.CreateLimit(limitContext));
            }

            CqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiTableNode VisitColumnFamilyName(ColumnFamilyNameContext context)
        {
            var node = new QsiTableAccessNode
            {
                Identifier = context.id
            };

            CqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiColumnsDeclarationNode VisitSelectors(SelectorsContext context)
        {
            if (ListUtility.IsNullOrEmpty(context.selector()))
                return TreeHelper.CreateAllColumnsDeclaration();

            return TreeHelper.Create<QsiColumnsDeclarationNode>(n =>
            {
                n.Columns.AddRange(context.selector().Select(VisitSelector));
                CqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiColumnNode VisitSelector(SelectorContext context)
        {
            var record = VisitUnaliasedSelector(context.unaliasedSelector());
            var aliasNode = context.alias()?.node;
            QsiDerivedColumnNode node;

            if (aliasNode == null)
            {
                if (record.Column != null)
                    return record.Column;

                node = new QsiDerivedColumnNode();
                node.Expression.SetValue(record.Expression);
            }
            else
            {
                node = new QsiDerivedColumnNode();

                if (record.Column != null)
                    node.Column.SetValue(record.Column);
                else
                    node.Expression.SetValue(record.Expression);

                node.Alias.SetValue(aliasNode);
            }

            CqlTree.PutContextSpan(node, context);

            return node;
        }

        public static SelectorPair VisitUnaliasedSelector(UnaliasedSelectorContext context)
        {
            return VisitSelectionAddition(context.selectionAddition());
        }

        public static SelectorPair VisitSelectionAddition(SelectionAdditionContext context)
        {
            if (ListUtility.IsNullOrEmpty(context.additionOperator()))
                return VisitSelectionMultiplication(context.left);

            return new SelectorPair(ExpressionVisitor.VisitSelectionAddition(context));
        }

        public static SelectorPair VisitSelectionMultiplication(SelectionMultiplicationContext context)
        {
            if (ListUtility.IsNullOrEmpty(context.multiplicationOperator()))
                return VisitSelectionGroup(context.left);

            return new SelectorPair(ExpressionVisitor.VisitSelectionMultiplication(context));
        }

        public static SelectorPair VisitSelectionGroup(SelectionGroupContext context)
        {
            if (context.ChildCount != 1)
                throw new QsiException(QsiError.Syntax);

            switch (context.children[0])
            {
                case SelectionGroupWithFieldContext selectionGroupWithField:
                    return new SelectorPair(ExpressionVisitor.VisitSelectionGroupWithField(selectionGroupWithField));

                case SelectionGroupWithoutFieldContext selectionGroupWithoutField:
                    return VisitSelectionGroupWithoutField(selectionGroupWithoutField);

                // '-' selectionGroup
                case SelectionGroupContext selectionGroup:
                    return new SelectorPair(ExpressionVisitor.VisitSelectionGroup(context.unary, selectionGroup));

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
        }

        public static SelectorPair VisitSelectionGroupWithoutField(SelectionGroupWithoutFieldContext context)
        {
            if (context.ChildCount != 1)
                throw new QsiException(QsiError.Syntax);

            switch (context.children[0])
            {
                case SimpleUnaliasedSelectorContext simpleUnaliasedSelector:
                    return VisitSimpleUnaliasedSelector(simpleUnaliasedSelector);

                case SelectionTypeHintContext selectionTypeHint:
                    return new SelectorPair(ExpressionVisitor.VisitSelectionTypeHint(selectionTypeHint));

                case SelectionTupleOrNestedSelectorContext selectionTupleOrNestedSelector:
                    return VisitSelectionTupleOrNestedSelector(selectionTupleOrNestedSelector);

                case SelectionListContext selectionList:
                    return new SelectorPair(ExpressionVisitor.VisitSelectionList(selectionList));

                case SelectionMapOrSetContext selectionMapOrSet:
                    return new SelectorPair(ExpressionVisitor.VisitSelectionMapOrSet(selectionMapOrSet));

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
        }

        private static SelectorPair VisitSelectionTupleOrNestedSelector(SelectionTupleOrNestedSelectorContext context)
        {
            if (context._list.Count > 1)
                return new SelectorPair(ExpressionVisitor.VisitSelectionTupleOrNestedSelector(context));

            return VisitUnaliasedSelector(context._list[0]);
        }

        public static SelectorPair VisitSimpleUnaliasedSelector(SimpleUnaliasedSelectorContext context)
        {
            if (context.ChildCount != 1)
                throw new QsiException(QsiError.Syntax);

            switch (context.children[0])
            {
                case SidentContext sident:
                    return new SelectorPair(VisitSident(sident));

                case SelectionLiteralContext selectionLiteral:
                    return new SelectorPair(ExpressionVisitor.VisitSelectionLiteral(selectionLiteral));

                case SelectionFunctionContext selectionFunction:
                    return new SelectorPair(ExpressionVisitor.VisitSelectionFunction(selectionFunction));

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
        }

        public static QsiDeclaredColumnNode VisitSident(SidentContext context)
        {
            return new QsiDeclaredColumnNode
            {
                Name = new QsiQualifiedIdentifier(context.id)
            };
        }
        #endregion

        #region CreateMaterializedViewStatement
        public static QsiTableNode VisitCreateMaterializedViewStatement(CreateMaterializedViewStatementContext context)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}

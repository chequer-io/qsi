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
                IsJson = context.K_JSON() != null,
                AllowFiltering = context.K_FILTERING() != null
            };

            node.Columns.SetValue(VisitSelectors(context.selectors()));
            node.Source.SetValue(VisitColumnFamilyName(context.columnFamilyName()));

            if (context.whereClause() != null)
                node.Where.SetValue(ExpressionVisitor.VisitWhereClause(context.whereClause()));

            if (!ListUtility.IsNullOrEmpty(context.groupByClause()))
                node.Grouping.SetValue(ExpressionVisitor.VisitGroupByClauses(context.groupByClause()));

            if (!ListUtility.IsNullOrEmpty(context.orderByClause()))
                node.Order.SetValue(ExpressionVisitor.VisitOrderByClauses(context.orderByClause()));

            if (context.perLimit != null)
                node.PerPartitionLimit = ExpressionVisitor.VisitIntValue(context.perLimit);

            if (context.limit != null)
                node.Limit.SetValue(ExpressionVisitor.CreateLimit(context.limit));

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
                    return new SelectorPair(ExpressionVisitor.VisitSelectionTupleOrNestedSelector(selectionTupleOrNestedSelector));

                case SelectionListContext selectionList:
                    return new SelectorPair(ExpressionVisitor.VisitSelectionList(selectionList));

                case SelectionMapOrSetContext selectionMapOrSet:
                    return new SelectorPair(ExpressionVisitor.VisitSelectionMapOrSet(selectionMapOrSet));

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
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

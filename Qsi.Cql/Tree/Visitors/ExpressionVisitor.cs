using System.Linq;
using System.Linq.Expressions;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Qsi.Cql.Tree.Common;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;
using static Qsi.Cql.Internal.CqlParserInternal;

namespace Qsi.Cql.Tree
{
    internal static class ExpressionVisitor
    {
        public static QsiExpressionNode ConvertToExpression(SelectorPair pair)
        {
            if (pair.Column != null)
            {
                var node = new QsiColumnExpressionNode();
                node.Column.SetValue(pair.Column);
                return node;
            }

            return pair.Expression;
        }

        public static QsiExpressionNode VisitSelectionGroup(SelectionGroupContext context)
        {
            return ConvertToExpression(TableVisitor.VisitSelectionGroup(context));
        }

        public static QsiExpressionNode VisitSelectionGroupWithoutField(SelectionGroupWithoutFieldContext context)
        {
            return ConvertToExpression(TableVisitor.VisitSelectionGroupWithoutField(context));
        }

        public static QsiExpressionNode VisitUnaliasedSelector(UnaliasedSelectorContext context)
        {
            return ConvertToExpression(TableVisitor.VisitUnaliasedSelector(context));
        }

        public static QsiExpressionNode VisitSelectionGroup(IToken unary, SelectionGroupContext context)
        {
            var node = new QsiUnaryExpressionNode();

            node.Expression.SetValue(VisitSelectionGroup(context));
            CqlTree.PutContextSpan(node, unary, context.Stop);

            return node;
        }

        public static QsiExpressionNode VisitSelectionAddition(SelectionAdditionContext context)
        {
            AdditionOperatorContext[] operators = context.additionOperator();
            var left = VisitSelectionMultiplication(context.left);

            if (ListUtility.IsNullOrEmpty(operators))
                return left;

            SelectionMultiplicationContext[] rights = context.selectionMultiplication();

            for (int i = 0; i < operators.Length; i++)
            {
                var node = new QsiLogicalExpressionNode
                {
                    Operator = operators[i].GetText()
                };

                node.Left.SetValue(left);
                node.Right.SetValue(VisitSelectionMultiplication(rights[i + 1]));

                left = node;
            }

            return left;
        }

        public static QsiExpressionNode VisitSelectionMultiplication(SelectionMultiplicationContext context)
        {
            MultiplicationOperatorContext[] operators = context.multiplicationOperator();
            var left = VisitSelectionGroup(context.left);

            if (ListUtility.IsNullOrEmpty(operators))
                return left;

            SelectionGroupContext[] rights = context.selectionGroup();

            for (int i = 0; i < operators.Length; i++)
            {
                var node = new QsiLogicalExpressionNode
                {
                    Operator = operators[i].GetText()
                };

                node.Left.SetValue(left);
                node.Right.SetValue(VisitSelectionGroup(rights[i + 1]));

                left = node;
            }

            return left;
        }

        public static QsiWhereExpressionNode VisitWhereClause(WhereClauseContext context)
        {
            throw new System.NotImplementedException();
        }

        public static QsiGroupingExpressionNode VisitGroupByClauses(GroupByClauseContext[] contexts)
        {
            throw new System.NotImplementedException();
        }

        public static QsiMultipleOrderExpressionNode VisitOrderByClauses(OrderByClauseContext[] contexts)
        {
            throw new System.NotImplementedException();
        }

        public static QsiLimitExpressionNode CreateLimit(IntValueContext context)
        {
            throw new System.NotImplementedException();
        }

        public static QsiExpressionNode VisitIntValue(IntValueContext context)
        {
            if (context.t != null)
            {
                var node = TreeHelper.CreateLiteral(long.Parse(context.t.Text));
                CqlTree.PutContextSpan(node, context);
                return node;
            }

            return VisitBindParameter(context.bindParameter());
        }

        private static QsiExpressionNode VisitBindParameter(BindParameterContext context)
        {
            return TreeHelper.Create<QsiColumnExpressionNode>(n =>
            {
                n.Column.SetValue(new QsiBindingColumnNode
                {
                    Id = context.id?.id?.Value ?? "?"
                });

                CqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiExpressionNode VisitSelectionGroupWithField(SelectionGroupWithFieldContext context)
        {
            var target = VisitSelectionGroupWithoutField(context.selectionGroupWithoutField());
            var node = VisitSelectorModifier(target, context.selectorModifier());

            CqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiExpressionNode VisitSelectorModifier(QsiExpressionNode target, SelectorModifierContext context)
        {
            QsiExpressionNode access1;
            SelectorModifierContext access2;

            switch (context)
            {
                case FieldAccessContext fieldAccess:
                    access1 = VisitFieldSelectorModifier(fieldAccess.fieldSelectorModifier());
                    access2 = fieldAccess.selectorModifier();
                    break;

                case RangeAccessContext rangeAccess:
                    access1 = VisitCollectionSubSelection(rangeAccess.collectionSubSelection());
                    access2 = rangeAccess.selectorModifier();
                    break;

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }

            var node = new QsiMemberAccessExpressionNode();
            node.Member.SetValue(target);
            node.Target.SetValue(access1);

            if (access2 == null)
                return node;

            var memberAccessNode = VisitSelectorModifier(node, access2);
            CqlTree.PutContextSpan(memberAccessNode, context);

            return memberAccessNode;
        }

        private static QsiExpressionNode VisitCollectionSubSelection(CollectionSubSelectionContext context)
        {
            QsiExpressionNode term1 = null;
            QsiExpressionNode term2 = null;

            if (context.t1 != null)
                term1 = VisitTerm(context.t1);

            if (context.t2 != null)
                term2 = VisitTerm(context.t2);
            else if (context.RANGE() == null)
                return term1;

            return TreeHelper.Create<CqlRangeExpressionNode>(n =>
            {
                if (term1 != null)
                    n.Start.SetValue(term1);

                if (term2 != null)
                    n.Start.SetValue(term2);

                CqlTree.PutContextSpan(n, context);
            });
        }

        private static QsiExpressionNode VisitFieldSelectorModifier(FieldSelectorModifierContext context)
        {
            return TreeHelper.Create<QsiFieldExpressionNode>(n =>
            {
                n.Identifier = new QsiQualifiedIdentifier(context.fident().id);
                CqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiExpressionNode VisitSelectionTypeHint(SelectionTypeHintContext context)
        {
            var type = context.comparatorType().GetText();
            var node = TreeHelper.CreateUnary($"({type})", VisitSelectionGroupWithoutField(context.selectionGroupWithoutField()));

            CqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiExpressionNode VisitSelectionTupleOrNestedSelector(SelectionTupleOrNestedSelectorContext context)
        {
            return TreeHelper.Create<QsiMultipleExpressionNode>(n =>
            {
                n.Elements.AddRange(context.unaliasedSelector().Select(VisitUnaliasedSelector));
                CqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiExpressionNode VisitSelectionList(SelectionListContext context)
        {
        }

        public static QsiExpressionNode VisitSelectionMapOrSet(SelectionMapOrSetContext context)
        {
            throw new System.NotImplementedException();
        }

        public static QsiExpressionNode VisitSelectionLiteral(SelectionLiteralContext context)
        {
            if (context.c != null)
                return VisitConstant(context.c);

            if (context.n != null)
                return TreeHelper.CreateNullLiteral();

            return VisitBindParameter(context.b);
        }

        public static QsiExpressionNode VisitSelectionFunction(SelectionFunctionContext context)
        {
            throw new System.NotImplementedException();
        }

        private static QsiExpressionNode VisitTerm(TermContext context)
        {
            return VisitTermAddition(context.termAddition());
        }

        private static QsiExpressionNode VisitTermAddition(TermAdditionContext context)
        {
            AdditionOperatorContext[] operators = context.additionOperator();
            var left = VisitTermMultiplication(context.left);

            if (ListUtility.IsNullOrEmpty(operators))
                return left;

            TermMultiplicationContext[] rights = context.termMultiplication();

            for (int i = 0; i < operators.Length; i++)
            {
                var node = new QsiLogicalExpressionNode
                {
                    Operator = operators[i].GetText()
                };

                node.Left.SetValue(left);
                node.Right.SetValue(VisitTermMultiplication(rights[i + 1]));

                left = node;
            }

            return left;
        }

        private static QsiExpressionNode VisitTermMultiplication(TermMultiplicationContext context)
        {
            MultiplicationOperatorContext[] operators = context.multiplicationOperator();
            var left = VisitTermGroup(context.left);

            if (ListUtility.IsNullOrEmpty(operators))
                return left;

            TermGroupContext[] rights = context.termGroup();

            for (int i = 0; i < operators.Length; i++)
            {
                var node = new QsiLogicalExpressionNode
                {
                    Operator = operators[i].GetText()
                };

                node.Left.SetValue(left);
                node.Right.SetValue(VisitTermGroup(rights[i + 1]));

                left = node;
            }

            return left;
        }

        private static QsiExpressionNode VisitTermGroup(TermGroupContext context)
        {
            var node = VisitSimpleTerm(context.simpleTerm());

            if (context.u == null)
                return node;

            node = TreeHelper.CreateUnary(context.u.Text, node);
            CqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiExpressionNode VisitSimpleTerm(SimpleTermContext context)
        {
            if (context.v != null)
                return VisitValue(context.v);

            if (context.f != null)
                return VisitFunction(context.f);

            var type = context.comparatorType().GetText();
            var node = TreeHelper.CreateUnary($"({type})", VisitSimpleTerm(context.t));

            CqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiExpressionNode VisitValue(ValueContext context)
        {
            if (context.c != null)
                return VisitConstant(context.c);

            if (context.l != null)
                return VisitCollectionLiteral(context.l);

            if (context.u != null)
                return VisitUsertypeLiteral(context.u);

            if (context.t != null)
                return VisitTupleLiteral(context.t);

            if (context.n != null)
                return TreeHelper.CreateNullLiteral();

            return VisitBindParameter(context.bindParameter());
        }

        private static QsiExpressionNode VisitConstant(ConstantContext context)
        {
            string text = context.GetText();
            QsiExpressionNode node;

            switch (context)
            {
                case LiteralStringContext _:
                {
                    // '..'
                    // $$..$$ 
                    string value = text[0] == '\'' ?
                        IdentifierUtility.Unescape(text) :
                        text[2..^2];

                    node = TreeHelper.CreateLiteral(value);
                    break;
                }

                case LiteralIntegerContext _:
                {
                    node = TreeHelper.CreateLiteral(int.Parse(text));
                    break;
                }

                case LiteralFloatContext _:
                {
                    node = TreeHelper.CreateLiteral(double.Parse(text));
                    break;
                }

                case LiteralBooleanContext _:
                {
                    node = TreeHelper.CreateLiteral(bool.Parse(text));
                    break;
                }

                case LiteralHexnumberContext _:
                {
                    node = TreeHelper.CreateLiteral(text, QsiDataType.Hexadecimal);
                    break;
                }

                case LiteralPositiveNanContext _:
                {
                    node = TreeHelper.CreateLiteral(double.NaN);
                    break;
                }

                case LiteralNegativeNanContext _:
                {
                    node = TreeHelper.CreateLiteral(-double.NaN);
                    break;
                }

                case LiteralPositiveInfinityContext _:
                {
                    node = TreeHelper.CreateLiteral(double.PositiveInfinity);
                    break;
                }

                case LiteralNegativeInfinityContext _:
                {
                    node = TreeHelper.CreateLiteral(double.NegativeInfinity);
                    break;
                }

                default:
                {
                    node = TreeHelper.CreateLiteral(text, QsiDataType.Raw);
                    break;
                }
            }

            CqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiExpressionNode VisitCollectionLiteral(CollectionLiteralContext context)
        {
            throw new System.NotImplementedException();
        }

        private static QsiExpressionNode VisitUsertypeLiteral(UsertypeLiteralContext context)
        {
            throw new System.NotImplementedException();
        }

        private static QsiExpressionNode VisitTupleLiteral(TupleLiteralContext context)
        {
            throw new System.NotImplementedException();
        }

        private static QsiExpressionNode VisitFunction(FunctionContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
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

        public static QsiWhereExpressionNode CreateWhere(ParserRuleContextWrapper<WhereClauseContext> context)
        {
            QsiExpressionNode[] nodes = context.Value.relationOrExpression()
                .Select(VisitRelationOrExpression)
                .ToArray();

            var node = nodes[0];

            for (int i = 1; i < nodes.Length; i++)
            {
                var binaryNode = new QsiLogicalExpressionNode
                {
                    Operator = "AND"
                };

                binaryNode.Left.SetValue(node);
                binaryNode.Right.SetValue(nodes[i]);

                var leftSpan = CqlTree.Span[node];
                var rightSpan = CqlTree.Span[nodes[i]];

                CqlTree.Span[binaryNode] = new Range(leftSpan.Start, rightSpan.End);

                node = binaryNode;
            }

            var whereNode = new QsiWhereExpressionNode();
            whereNode.Expression.SetValue(node);

            CqlTree.PutContextSpan(whereNode, context);

            return whereNode;
        }

        private static QsiExpressionNode VisitRelationOrExpression(RelationOrExpressionContext context)
        {
            if (context.ChildCount != 1)
                throw new QsiException(QsiError.Syntax);

            switch (context.children[0])
            {
                case RelationContext relation:
                    return VisitRelation(relation);

                case CustomIndexExpressionContext customIndexExpression:
                    return VisitCustomIndexExpression(customIndexExpression);

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
        }

        private static QsiExpressionNode VisitRelation(RelationContext context)
        {
            switch (context)
            {
                case LogicalExpr1Context logicalExpr1:
                    return VisitLogicalExpr1(logicalExpr1);

                case LikeExprContext likeExpr:
                    return VisitLikeExpr(likeExpr);

                case IsNotNulExprContext isNotNulExpr:
                    return VisitIsNotNulExpr(isNotNulExpr);

                case TokenExprContext tokenExpr:
                    return VisitTokenExpr(tokenExpr);

                case InExpr1Context inExpr1:
                    return VisitInExpr1(inExpr1);

                case InExpr2Context inExpr2:
                    return VisitInExpr2(inExpr2);

                case ContainsExprContext constainsExpr:
                    return VisitContainsExpr(constainsExpr);

                case LogicalExpr2Context logicalExpr2:
                    return VisitLogicalExpr2(logicalExpr2);

                case TupleExprContext tupleExpr:
                    return VisitTupleExpr(tupleExpr);

                case GroupExprContext groupExpr:
                    return VisitGroupExpr(groupExpr);

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
        }

        private static QsiExpressionNode VisitLogicalExpr1(LogicalExpr1Context context)
        {
            var node = new QsiLogicalExpressionNode
            {
                Operator = context.op.GetText()
            };

            node.Left.SetValue(VisitCident(context.l));
            node.Right.SetValue(VisitTerm(context.r));

            CqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiExpressionNode VisitLikeExpr(LikeExprContext context)
        {
            var node = new QsiInvokeExpressionNode();

            node.Member.SetValue(TreeHelper.CreateFunction(CqlKnownFunction.Like));
            node.Parameters.Add(VisitCident(context.l));
            node.Parameters.Add(VisitTerm(context.r));

            CqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiExpressionNode VisitIsNotNulExpr(IsNotNulExprContext context)
        {
            var node = new QsiInvokeExpressionNode();

            node.Member.SetValue(TreeHelper.CreateFunction(CqlKnownFunction.IsNotNull));
            node.Parameters.Add(VisitCident(context.l));

            CqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiExpressionNode VisitTokenExpr(TokenExprContext context)
        {
            var node = new QsiInvokeExpressionNode();

            node.Member.SetValue(TreeHelper.CreateFunction(CqlKnownFunction.TokenCompare));
            node.Parameters.Add(VisitTupleOfIdentifiers(context.l));
            node.Parameters.Add(TreeHelper.CreateLiteral(context.op.GetText()));
            node.Parameters.Add(VisitTerm(context.r));

            CqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiExpressionNode VisitInExpr1(InExpr1Context context)
        {
            var node = new QsiInvokeExpressionNode();

            node.Member.SetValue(TreeHelper.CreateFunction(CqlKnownFunction.In));
            node.Parameters.Add(VisitCident(context.l));
            node.Parameters.Add(VisitInMarker(context.r));

            CqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiExpressionNode VisitInExpr2(InExpr2Context context)
        {
            var node = new QsiInvokeExpressionNode();

            node.Member.SetValue(TreeHelper.CreateFunction(CqlKnownFunction.In));
            node.Parameters.Add(VisitCident(context.l));
            node.Parameters.Add(VisitSingleColumnInValues(context.r));

            CqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiExpressionNode VisitContainsExpr(ContainsExprContext context)
        {
            var functionName = context.op.key ?
                CqlKnownFunction.ContainsKey :
                CqlKnownFunction.Contains;

            var node = new QsiInvokeExpressionNode();

            node.Member.SetValue(TreeHelper.CreateFunction(functionName));
            node.Parameters.Add(VisitCident(context.l));
            node.Parameters.Add(VisitTerm(context.r));

            CqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiExpressionNode VisitLogicalExpr2(LogicalExpr2Context context)
        {
            var node = new QsiLogicalExpressionNode
            {
                Operator = context.op.GetText()
            };

            var leftNode = new QsiMemberAccessExpressionNode();
            var indexerNode = new CqlIndexerExpressionNode();

            indexerNode.Indexer.SetValue(VisitTerm(context.key));

            leftNode.Member.SetValue(VisitCident(context.l));
            leftNode.Target.SetValue(indexerNode);

            node.Left.SetValue(leftNode);
            node.Right.SetValue(VisitTerm(context.r));

            CqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiExpressionNode VisitTupleExpr(TupleExprContext context)
        {
            QsiExpressionNode node;

            var leftNode = VisitTupleOfIdentifiers(context.l);

            if (context.tupleInMarker != null || context.literals != null || context.markers != null)
            {
                var invokeNode = new QsiInvokeExpressionNode();

                invokeNode.Member.SetValue(TreeHelper.CreateFunction(CqlKnownFunction.In));
                invokeNode.Parameters.Add(leftNode);

                if (context.tupleInMarker != null)
                    invokeNode.Parameters.Add(VisitInMarkerForTuple(context.tupleInMarker));

                if (context.literals != null)
                    invokeNode.Parameters.Add(VisitTupleOfTupleLiterals(context.literals));

                if (context.markers != null)
                    invokeNode.Parameters.Add(VisitTupleOfMarkersForTuples(context.markers));

                node = invokeNode;
            }
            else if (context.literal != null || context.tupleMarker != null)
            {
                var logicalNode = new QsiLogicalExpressionNode
                {
                    Operator = context.op.GetText()
                };

                logicalNode.Left.SetValue(leftNode);

                if (context.literal != null)
                    logicalNode.Right.SetValue(VisitTupleLiteral(context.literal));

                if (context.tupleMarker != null)
                    logicalNode.Right.SetValue(VisitMarkerForTuple(context.tupleMarker));

                node = logicalNode;
            }
            else
            {
                node = new CqlTupleExpressionNode();
            }

            CqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiExpressionNode VisitTupleOfIdentifiers(TupleOfIdentifiersContext context)
        {
            var node = new CqlTupleExpressionNode();

            node.Elements.AddRange(context.cidentList()._list.Select(VisitCident));
            CqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiExpressionNode VisitSingleColumnInValues(SingleColumnInValuesContext context)
        {
            var node = new QsiMultipleExpressionNode();

            node.Elements.AddRange(context._list.Select(VisitTerm));
            CqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiExpressionNode VisitGroupExpr(GroupExprContext context)
        {
            var node = VisitRelation(context.relation());
            CqlTree.PutContextSpan(node, context);
            return node;
        }

        private static QsiExpressionNode VisitCustomIndexExpression(CustomIndexExpressionContext context)
        {
            var node = new QsiInvokeExpressionNode();

            node.Member.SetValue(TreeHelper.CreateFunction(CqlKnownFunction.Expr));
            node.Parameters.Add(VisitIdxName(context.idxName()));
            node.Parameters.Add(VisitTerm(context.term()));

            CqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiExpressionNode VisitIdxName(IdxNameContext context)
        {
            var node = new CqlIndexExpressionNode
            {
                Identifier = new QsiQualifiedIdentifier(context.id)
            };

            CqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiGroupingExpressionNode CreateGrouping(ParserRuleContextWrapper<GroupByClauseContext[]> context)
        {
            var node = new QsiGroupingExpressionNode();

            node.Items.AddRange(context.Value.Select(VisitGroupByClause));
            CqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiExpressionNode VisitGroupByClause(GroupByClauseContext context)
        {
            return VisitCident(context.cident());
        }

        public static QsiMultipleOrderExpressionNode CreateOrder(ParserRuleContextWrapper<OrderByClauseContext[]> context)
        {
            var node = new QsiMultipleOrderExpressionNode();

            node.Orders.AddRange(context.Value.Select(VisitOrderByClause));
            CqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiOrderExpressionNode VisitOrderByClause(OrderByClauseContext context)
        {
            var node = new QsiOrderExpressionNode();

            node.Expression.SetValue(VisitCident(context.cident()));
            node.Order = context.isDescending ? QsiSortOrder.Descending : QsiSortOrder.Ascending;
            CqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiLimitExpressionNode CreateLimit(ParserRuleContextWrapper<IntValueContext> context)
        {
            var node = new QsiLimitExpressionNode();

            node.Limit.SetValue(VisitIntValue(context.Value));
            CqlTree.PutContextSpan(node, context);

            return node;
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
            QsiExpressionNode node;

            var term1 = context.t1 != null ? VisitTerm(context.t1) : null;
            var term2 = context.t2 != null ? VisitTerm(context.t2) : null;

            if (context.range)
            {
                var rangeNode = new CqlRangeExpressionNode();

                if (term1 != null)
                    rangeNode.Start.SetValue(term1);

                if (term2 != null)
                    rangeNode.End.SetValue(term2);

                node = rangeNode;
            }
            else
            {
                var indexerNode = new CqlIndexerExpressionNode();

                indexerNode.Indexer.SetValue(term1);
                node = indexerNode;
            }

            CqlTree.PutContextSpan(node, context);

            return node;
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
            var type = new QsiQualifiedIdentifier(new QsiIdentifier(context.comparatorType().GetText(), false));
            var node = new QsiInvokeExpressionNode();

            node.Member.SetValue(TreeHelper.CreateFunction(CqlKnownFunction.InlineCast));
            node.Parameters.Add(VisitSelectionGroupWithoutField(context.selectionGroupWithoutField()));
            node.Parameters.Add(new QsiTypeExpressionNode { Identifier = type });

            CqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiExpressionNode VisitSelectionTupleOrNestedSelector(SelectionTupleOrNestedSelectorContext context)
        {
            QsiExpressionNode[] children = context._list
                .Select(VisitUnaliasedSelector)
                .ToArray();

            if (children.Length > 1)
            {
                var node = new CqlTupleExpressionNode();

                node.Elements.AddRange(children);
                CqlTree.PutContextSpan(node, context);

                return node;
            }

            return children[0];
        }

        public static QsiExpressionNode VisitSelectionList(SelectionListContext context)
        {
            return TreeHelper.Create<CqlListExpressionNode>(n =>
            {
                n.Elements.AddRange(context.unaliasedSelector().Select(VisitUnaliasedSelector));
                CqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiExpressionNode VisitSelectionMapOrSet(SelectionMapOrSetContext context)
        {
            var first = context.unaliasedSelector();
            QsiExpressionNode node;

            if (first == null)
            {
                node = new CqlMapExpressionNode();
            }
            else if (context.m != null)
            {
                node = VisitSelectionMap(first, context.m);
            }
            else
            {
                node = VisitSelectionSet(first, context.s);
            }

            CqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiExpressionNode VisitSelectionMap(UnaliasedSelectorContext first, SelectionMapContext context)
        {
            return TreeHelper.Create<CqlMapExpressionNode>(n =>
            {
                n.Elements.AddRange(
                    context.unaliasedSelector()
                        .Prepend(first)
                        .Select(VisitUnaliasedSelector));
            });
        }

        private static QsiExpressionNode VisitSelectionSet(UnaliasedSelectorContext first, SelectionSetContext context)
        {
            return TreeHelper.Create<CqlSetExpressionNode>(n =>
            {
                n.Elements.AddRange(
                    context.unaliasedSelector()
                        .Prepend(first)
                        .Select(VisitUnaliasedSelector));
            });
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
            switch (context)
            {
                case CountFunctionContext countFunction:
                    return VisitCountFunction(countFunction);

                case WritetimeFunctionContext writetimeFunction:
                    return VisitWritetimeFunction(writetimeFunction);

                case TtlFunctionContext ttlFunction:
                    return VisitTtlFunction(ttlFunction);

                case CastFunctionContext castFunction:
                    return VisitCastFunction(castFunction);

                case UserFunctionContext userFunction:
                    return VisitUserFunction(userFunction);

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
        }

        private static QsiExpressionNode VisitCountFunction(CountFunctionContext context)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunction(CqlKnownFunction.Count));

                n.Parameters.Add(TreeHelper.Create<QsiColumnExpressionNode>(cn =>
                {
                    cn.Column.SetValue(new QsiAllColumnNode());
                }));

                CqlTree.PutContextSpan(n, context);
            });
        }

        private static QsiExpressionNode VisitWritetimeFunction(WritetimeFunctionContext context)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunction(CqlKnownFunction.Writetime));
                n.Parameters.Add(VisitSident(context.c));

                CqlTree.PutContextSpan(n, context);
            });
        }

        private static QsiExpressionNode VisitTtlFunction(TtlFunctionContext context)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunction(CqlKnownFunction.TTL));
                n.Parameters.Add(VisitSident(context.c));

                CqlTree.PutContextSpan(n, context);
            });
        }

        private static QsiExpressionNode VisitCastFunction(CastFunctionContext context)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunction(CqlKnownFunction.Cast));
                n.Parameters.Add(VisitUnaliasedSelector(context.sn));
                n.Parameters.Add(VisitNativeType(context.t));

                CqlTree.PutContextSpan(n, context);
            });
        }

        private static QsiExpressionNode VisitUserFunction(UserFunctionContext context)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(new QsiFunctionExpressionNode
                {
                    Identifier = context.n.id
                });

                n.Parameters.AddRange(context.args.unaliasedSelector().Select(VisitUnaliasedSelector));

                CqlTree.PutContextSpan(n.Member.Value, context.n);
                CqlTree.PutContextSpan(n, context);
            });
        }

        private static QsiExpressionNode VisitFunction(FunctionContext context)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(new QsiFunctionExpressionNode
                {
                    Identifier = context.n.id
                });

                n.Parameters.AddRange(context.args.term().Select(VisitTerm));

                CqlTree.PutContextSpan(n.Member.Value, context.n);
                CqlTree.PutContextSpan(n, context);
            });
        }

        private static QsiExpressionNode VisitNativeType(Native_typeContext context)
        {
            return new QsiTypeExpressionNode
            {
                Identifier = new QsiQualifiedIdentifier(new QsiIdentifier(context.GetText(), false))
            };
        }

        private static QsiExpressionNode VisitSident(SidentContext context)
        {
            var node = new QsiColumnExpressionNode();

            node.Column.SetValue(new QsiDeclaredColumnNode
            {
                Name = new QsiQualifiedIdentifier(context.id)
            });

            CqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiExpressionNode VisitCident(CidentContext context)
        {
            var node = new QsiColumnExpressionNode();

            node.Column.SetValue(new QsiDeclaredColumnNode
            {
                Name = new QsiQualifiedIdentifier(context.id)
            });

            CqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiExpressionNode VisitIdent(IdentContext context)
        {
            var node = new QsiColumnExpressionNode();

            node.Column.SetValue(new QsiDeclaredColumnNode
            {
                Name = new QsiQualifiedIdentifier(context.id)
            });

            CqlTree.PutContextSpan(node, context);

            return node;
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
                case LiteralStringContext literalString:
                {
                    node = TreeHelper.CreateLiteral(literalString.s.raw);
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
            QsiExpressionNode node;

            if (context.l != null)
            {
                node = TreeHelper.Create<CqlListExpressionNode>(n =>
                {
                    n.Elements.AddRange(context.l.term().Select(VisitTerm));
                });
            }
            else if (context.t != null)
            {
                if (context.v.m != null)
                {
                    node = TreeHelper.Create<CqlMapExpressionNode>(n =>
                    {
                        IEnumerable<TermContext> terms = context.v.m.term().Prepend(context.t);
                        n.Elements.AddRange(terms.Select(VisitTerm));
                    });
                }
                else
                {
                    node = TreeHelper.Create<CqlSetExpressionNode>(n =>
                    {
                        IEnumerable<TermContext> terms = context.v.s.term().Prepend(context.t);
                        n.Elements.AddRange(terms.Select(VisitTerm));
                    });
                }
            }
            else
            {
                node = new CqlSetExpressionNode();
            }

            CqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiExpressionNode VisitUsertypeLiteral(UsertypeLiteralContext context)
        {
            var node = TreeHelper.CreateLiteral(context.GetText());

            CqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiExpressionNode VisitTupleLiteral(TupleLiteralContext context)
        {
            var node = new CqlTupleExpressionNode();

            node.Elements.AddRange(context._list.Select(VisitTerm));
            CqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiExpressionNode VisitTupleOfTupleLiterals(TupleOfTupleLiteralsContext context)
        {
            var node = new CqlTupleExpressionNode();

            node.Elements.AddRange(context._list.Select(VisitTupleLiteral));
            CqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiExpressionNode VisitTupleOfMarkersForTuples(TupleOfMarkersForTuplesContext context)
        {
            var node = new CqlTupleExpressionNode();

            node.Elements.AddRange(context._list.Select(VisitMarkerForTuple));
            CqlTree.PutContextSpan(node, context);

            return node;
        }

        #region BindParameter
        private static QsiExpressionNode VisitBindParameter(BindParameterContext context)
        {
            return CreateBindingColumn(new ParserRuleContextWrapper<QsiIdentifier>(context.id?.id, context));
        }

        private static QsiExpressionNode VisitInMarker(InMarkerContext context)
        {
            return CreateBindingColumn(new ParserRuleContextWrapper<QsiIdentifier>(context.id?.id, context));
        }

        private static QsiExpressionNode VisitInMarkerForTuple(InMarkerForTupleContext context)
        {
            return CreateBindingColumn(new ParserRuleContextWrapper<QsiIdentifier>(context.id?.id, context));
        }

        private static QsiExpressionNode VisitMarkerForTuple(MarkerForTupleContext context)
        {
            return CreateBindingColumn(new ParserRuleContextWrapper<QsiIdentifier>(context.id?.id, context));
        }

        private static QsiExpressionNode CreateBindingColumn(ParserRuleContextWrapper<QsiIdentifier> context)
        {
            return TreeHelper.Create<QsiColumnExpressionNode>(n =>
            {
                n.Column.SetValue(new QsiBindingColumnNode
                {
                    Id = context.Value?.Value ?? "?"
                });

                CqlTree.PutContextSpan(n, context);
            });
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qsi.Data;
using Qsi.PostgreSql.Data;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Utilities;
using static Qsi.PostgreSql.Internal.PostgreSqlParserInternal;

namespace Qsi.PostgreSql.Tree.Visitors;

internal static class ExpressionVisitor
{
    public static QsiMultipleExpressionNode VisitExpressionList(ExpressionListContext context)
    {
        var node = new QsiMultipleExpressionNode();
        node.Elements.AddRange(context.expression().Select(VisitExpression));

        PostgreSqlTree.PutContextSpan(node, context);
        
        return node;
    }

    public static QsiExpressionNode VisitExpression(ExpressionContext context)
    {
        if (context.andExpression() != null)
        {
            return VisitAndExpression(context.andExpression());
        }

        var node = new QsiBinaryExpressionNode
        {
            Left = { Value = VisitExpression(context.expression(0)) },
            Operator = context.OR().GetText(),
            Right = { Value = VisitExpression(context.expression(1)) }
        };
        
        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    // public static QsiExpressionNode VisitExpressionNoParens(ExpressionNoParensContext context)
    // {
    //     if (context.OR() == null)
    //     {
    //         return VisitAndExpression(context.andExpression());
    //     }
    //
    //     return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
    //     {
    //         n.Left.Value =(VisitExpressionNoParens(context.expressionNoParens(0)));
    //         n.Operator = context.OR().GetText();
    //         n.Right.Value =(VisitExpressionNoParens(context.expressionNoParens(1)));
    //
    //         PostgreSqlTree.PutContextSpan(n, context);
    //     });
    // }

    public static QsiExpressionNode VisitExpressionParens(ExpressionParensContext context)
    {
        var node = VisitExpression(context.expression());
        
        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }
    
    // public static ExpressionContext UnwrapExpressionParens(ExpressionParensContext context)
    // {
    //     var nested = context;
    //
    //     while (nested.expressionParens() != null)
    //     {
    //         nested = context.expressionParens();
    //     }
    //
    //     return nested.expression();
    // }

    public static QsiExpressionNode VisitAndExpression(AndExpressionContext context)
    {
        if (context.booleanExpression() != null)
        {
            return VisitBooleanExpression(context.booleanExpression());
        }
        
        if (context.NOT() != null)
        {
            return TreeHelper.Create<QsiUnaryExpressionNode>(n =>
            {
                n.Expression.Value = VisitAndExpression(context.andExpression(0));
                n.Operator = context.NOT().GetText();
                
                PostgreSqlTree.PutContextSpan(n, context);
            });
        }

        if (context.AND() != null)
        {
            return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
            {
                n.Left.Value = VisitAndExpression(context.andExpression(0));
                n.Operator = context.AND().GetText();
                n.Right.Value = VisitAndExpression(context.andExpression(1));

                PostgreSqlTree.PutContextSpan(n, context);
            });
        }

        throw TreeHelper.NotSupportedTree(context);
    }

    public static QsiExpressionNode VisitBooleanExpression(BooleanExpressionContext context)
    {
        if (context.comparisonExpression() != null)
        {
            return VisitComparisonExpression(context.comparisonExpression());
        }

        if (context.DISTINCT() != null)
        {
            return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
            {
                n.Left.Value = VisitBooleanExpression(context.booleanExpression(0));
                n.Right.Value = VisitBooleanExpression(context.booleanExpression(1));

                var operatorName = context.children
                    .Select(c => c.GetText())
                    .ToArray()[1..^2];
                
                var operatorBuilder = new StringBuilder().AppendJoin(' ', operatorName);

                n.Operator = operatorBuilder.ToString();
                
                PostgreSqlTree.PutContextSpan(n, context);
            });
        }

        var opeartorCandidates = context.children
            .Where(c => c is not BooleanExpressionContext && c is not ComparisonExpressionContext);

        if (opeartorCandidates.All(c => c == null))
        {
            // TODO: Specify error reason.
            throw new Exception();
        }

        return TreeHelper.Create<QsiUnaryExpressionNode>(n =>
        {
            n.Expression.Value = VisitBooleanExpression(context.booleanExpression(0));

            var operatorName = context.children
                .Select(c => c.GetText())
                .ToArray()[1..];

            var operatorBuilder = new StringBuilder().AppendJoin(' ', operatorName);

            n.Operator = operatorBuilder.ToString();

            PostgreSqlTree.PutContextSpan(n, context);
        });
    }

    public static QsiExpressionNode VisitComparisonExpression(ComparisonExpressionContext context)
    {
        return context switch
        {
            ComparisonExpressionLikeContext qualified => VisitComparisonExpressionLike(qualified),
            ComparisonExpressionBaseContext comparison => VisitComparisonExpressionBase(comparison),
            ComparisonExpressionSubqueryContext subquery => VisitComparisonExpressionSubquery(subquery),
            _ => throw TreeHelper.NotSupportedTree(context)
        };
    }

    public static QsiExpressionNode VisitComparisonExpressionLike(ComparisonExpressionLikeContext context)
    {
        if (context.likeExpressionOptions() == null)
        {
            return VisitQualifiedOperatorExpression(context.qualifiedOperatorExpression(0));
        }

        var rightExpressions = TreeHelper.Create<QsiMultipleExpressionNode>(mn =>
        {
            mn.Elements.Add(VisitQualifiedOperatorExpression(context.qualifiedOperatorExpression(1)));

            if (!context.HasToken(ESCAPE))
            {
                PostgreSqlTree.PutContextSpan(mn, context.qualifiedOperatorExpression(1));
                return;
            }
            
            mn.Elements.Add(VisitExpression(context.expression()));
            PostgreSqlTree.PutContextSpan(mn, context.qualifiedOperatorExpression(1).Start, context.expression().Stop);
        });

        return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
        {
            n.Left.Value = VisitQualifiedOperatorExpression(context.qualifiedOperatorExpression(0));
            n.Operator = context.likeExpressionOptions().GetText();
            n.Right.Value =rightExpressions;
        });
    }

    public static QsiExpressionNode VisitComparisonExpressionBase(ComparisonExpressionBaseContext context)
    {
        return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
        {
            n.Left.Value = VisitComparisonExpression(context.comparisonExpression(0));
            n.Operator = context.comparisonOperator().GetText();
            n.Right.Value = VisitComparisonExpression(context.comparisonExpression(1));
        });
    }

    public static QsiExpressionNode VisitComparisonExpressionSubquery(ComparisonExpressionSubqueryContext context)
    {
        var node = new QsiBinaryExpressionNode
        {
            Left = { Value = VisitComparisonExpression(context.comparisonExpression()) },
            Operator = context.subqueryOperator().GetText() + " " + context.subqueryType().GetText(),
        };
        
        if (context.expression() != null)
        {
            node.Right.Value = VisitExpression(context.expression());
        }
        else
        {
            var tableExpression = new QsiTableExpressionNode
            {
                Table = { Value = TableVisitor.VisitQueryExpressionParens(context.queryExpressionParens()) }
            };
                
            PostgreSqlTree.PutContextSpan(tableExpression, context.queryExpressionParens());

            node.Right.Value = tableExpression;
        }
            
        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static QsiExpressionNode VisitQualifiedOperatorExpression(QualifiedOperatorExpressionContext context)
    {
        var expressions = context.unaryQualifiedOperatorExpression();

        var anchorNode = VisitUnaryQualifiedOperatorExpression(expressions[0]);
        
        if (expressions.Length == 1)
        {
            return anchorNode;
        }

        var operators = context.qualifiedWithoutMathOperator();
        
        for (var i = 1; i < expressions.Length; ++i)
        {
            var binaryNode = new QsiBinaryExpressionNode();
            binaryNode.Left.Value =anchorNode;
            binaryNode.Operator = operators[i - 1].GetText();
            binaryNode.Right.Value = VisitUnaryQualifiedOperatorExpression(expressions[i]);

            PostgreSqlTree.PutContextSpan(binaryNode, context.Start, expressions[i].Stop);
            
            anchorNode = binaryNode;
        }

        return anchorNode;
    }

    public static QsiExpressionNode VisitUnaryQualifiedOperatorExpression(UnaryQualifiedOperatorExpressionContext context)
    {
        var expression = VisitArithmeticExpression(context.arithmeticExpression());
        
        if (context.qualifiedWithoutMathOperator() == null)
        {
            return expression;
        }

        var node = new QsiUnaryExpressionNode();
        node.Expression.Value =expression;
        node.Operator = context.qualifiedWithoutMathOperator().GetText();

        PostgreSqlTree.PutContextSpan(node, context);
        
        return node;
    }

    public static QsiExpressionNode VisitArithmeticExpression(ArithmeticExpressionContext context)
    {
        if (context.collateExpression() != null)
        {
            return VisitCollateExpression(context.collateExpression());
        }

        if (context.arithmeticExpression().Length == 1)
        {
            var unary = new QsiUnaryExpressionNode();
            unary.Expression.Value = VisitArithmeticExpression(context.arithmeticExpression(0));
            unary.Operator = context.children[0].GetText();
            
            PostgreSqlTree.PutContextSpan(unary, context);
            
            return unary;
        }

        var binary = new QsiBinaryExpressionNode();
        binary.Left.Value = VisitArithmeticExpression(context.arithmeticExpression(0));
        binary.Operator = context.children[1].GetText();
        binary.Right.Value = VisitArithmeticExpression(context.arithmeticExpression(1));

        PostgreSqlTree.PutContextSpan(binary, context);
        
        return binary;
    }

    public static QsiExpressionNode VisitCollateExpression(CollateExpressionContext context)
    {
        var typecastNode = VisitTypecastExpression(context.typecastExpression());
        
        if (context.COLLATE() == null)
        {
            return typecastNode;
        }

        var collate = new QsiVariableExpressionNode
        {
            Identifier = IdentifierVisitor.VisitQualifiedIdentifier(context.qualifiedIdentifier())
        };

        PostgreSqlTree.PutContextSpan(collate, context.qualifiedIdentifier());
        
        var node = new QsiBinaryExpressionNode();
        node.Left.Value =typecastNode;
        node.Operator = context.COLLATE().GetText();
        node.Right.Value =collate;
        
        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static QsiExpressionNode VisitTypecastExpression(TypecastExpressionContext context)
    {
        var anchor = VisitValueExpression(context.valueExpression());
        
        if (context.TYPECAST() == null || context.TYPECAST().Length == 0)
        {
            return anchor;
        }

        var types = context.type();

        foreach (var t in types)
        {
            var binary = new QsiBinaryExpressionNode();
            binary.Left.Value =anchor;
            binary.Operator = context.TYPECAST(0).GetText();
            binary.Right.Value = VisitType(t);

            PostgreSqlTree.PutContextSpan(binary, context.Start, t.Stop);
            
            anchor = binary;
        }
        
        return anchor;
    }

    public static QsiExpressionNode VisitValueExpression(ValueExpressionContext context)
    {
        return context switch
        {
            ValueExpressionArrayContext arrayContext => VisitValueExpressionArray(arrayContext),
            ValueExpressionCaseContext caseContext => VisitValueExpressionCase(caseContext),
            // ValueExpressionColumnContext columnContext => VisitValueExpressionColumn(columnContext),
            ValueExpressionConstantContext constantContext => VisitValueExpressionConstant(constantContext),
            ValueExpressionFunctionContext functionContext => VisitValueExpressionFunction(functionContext),
            ValueExpressionGroupingContext groupingContext => VisitValueExpressionGrouping(groupingContext),
            // ValueExpressionParamContext paramContext => VisitValueExpressionParam(paramContext),
            ValueExpressionRowContext rowContext => VisitRow(rowContext.row()),
            ValueExpressionRowOverlapsContext rowOverlapsContext => VisitValueExpressionRowOverlaps(rowOverlapsContext),
            ValueExpressionSubqueryContext subqueryContext => VisitValueExpressionSubquery(subqueryContext),
            ValueExpressionStarContext starContext => VisitValueExpressionStar(starContext),
            ValueExpressionSubscriptContext subscriptContext => VisitSubscriptExpression(subscriptContext.subscriptExpression()),
            _ => throw TreeHelper.NotSupportedTree(context)
        };
    }

    public static QsiExpressionNode VisitValueExpressionArray(ValueExpressionArrayContext context)
    {
        return VisitExpressionList(context.expressionList());
    }

    public static QsiExpressionNode VisitValueExpressionCase(ValueExpressionCaseContext context)
    {
        var caseContext = context.caseExpression();
        
        var node = new QsiSwitchExpressionNode();

        if (caseContext.expression() != null)
        {
            node.Value.Value = VisitExpression(caseContext.expression());
        }

        if (caseContext.defaultCase() != null)
        {
            var defaultNode = new QsiSwitchCaseExpressionNode();
            defaultNode.Consequent.Value = VisitExpression(caseContext.defaultCase().expression());
            
            PostgreSqlTree.PutContextSpan(defaultNode, caseContext.defaultCase());
            
            node.Cases.Add(defaultNode);
        }

        var caseList = caseContext.whenClauseList().whenClause();
        var caseNodes = caseList.Select(c =>
        {
            var when = c.expression(0);
            var then = c.expression(1);

            var caseNode = new QsiSwitchCaseExpressionNode();
            caseNode.Condition.Value = VisitExpression(when);
            caseNode.Consequent.Value = VisitExpression(then);
            
            PostgreSqlTree.PutContextSpan(caseNode, c);

            return caseNode;
        });

        node.Cases.AddRange(caseNodes);
        
        PostgreSqlTree.PutContextSpan(node, caseContext);
        
        return node;
    }


    // public static QsiExpressionNode VisitValueExpressionColumn(ValueExpressionColumnContext context)
    // {
    //     var columnNode = new QsiColumnReferenceNode
    //     {
    //         Name = IdentifierVisitor.VisitQualifiedIdentifier(context.qualifiedIdentifier())
    //     };
    //     
    //     PostgreSqlTree.PutContextSpan(columnNode, context.qualifiedIdentifier());
    //
    //     var node = new QsiColumnExpressionNode();
    //     node.Column.Value =columnNode;
    //
    //     PostgreSqlTree.PutContextSpan(node, context);
    //
    //     return node;
    // }

    public static QsiExpressionNode VisitValueExpressionConstant(ValueExpressionConstantContext context)
    {
        return ConstantVisitor.VisitConstant(context.constant());
    }

    public static QsiExpressionNode VisitValueExpressionFunction(ValueExpressionFunctionContext context)
    {
        return FunctionVisitor.VisitFunctionExpression(context.functionExpression());
    }

    public static QsiExpressionNode VisitValueExpressionGrouping(ValueExpressionGroupingContext context)
    {
        return VisitExpressionList(context.expressionList());
    }

    public static QsiExpressionNode VisitParam(ParameterMarkerContext context)
    {
        var text = context.GetText()[1..];
        var index = int.Parse(text);
    
        var node = new QsiBindParameterExpressionNode
        {
            Prefix = "$",
            NoSuffix = true,
            Index = index,
            Type = QsiParameterType.Index
        };
        
        PostgreSqlTree.PutContextSpan(node, context);
    
        return node;
    }
    
    public static QsiExpressionNode VisitRow(RowContext context)
    {
        var node = new QsiMultipleExpressionNode();
        
        PostgreSqlTree.PutContextSpan(node, context);

        var explicitRow = context.explicitRow();
        var implicitRow = context.implicitRow();

        if (explicitRow?.expressionList() != null)
        {
            node.Elements.AddRange(explicitRow.expressionList().expression().Select(VisitExpression));
            return node;
        }
        
        node.Elements.Add( VisitExpression(implicitRow.expression()));
        node.Elements.AddRange(implicitRow.expressionList().expression().Select(VisitExpression));
        return node;
    }

    public static QsiExpressionNode VisitValueExpressionRowOverlaps(ValueExpressionRowOverlapsContext context)
    {
        var node = new QsiBinaryExpressionNode();
        node.Left.Value = VisitRow(context.row(0));
        node.Operator = context.OVERLAPS().GetText();
        node.Right.Value = VisitRow(context.row(1));
        
        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }
    
    public static QsiExpressionNode VisitValueExpressionSubquery(ValueExpressionSubqueryContext context)
    {
        var table = TableVisitor.VisitQueryExpressionParens(context.queryExpressionParens());
        
        var node = new QsiTableExpressionNode
        {
            Table =
            {
                Value = table
            }
        };

        PostgreSqlTree.PutContextSpan(node, context);
        return node;
    }

    public static QsiExpressionNode VisitValueExpressionStar(ValueExpressionStarContext context)
    {
        var tableIdentifier = IdentifierVisitor.VisitIdentifier(context.starIdentifier().columnIdentifier());
        var qualified = new QsiQualifiedIdentifier(tableIdentifier);
        
        var columnNode = new QsiAllColumnNode { Path = qualified };

        var node = new QsiColumnExpressionNode
        {
            Column = { Value = columnNode }
        };
        
        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static QsiExpressionNode VisitSubscriptExpression(SubscriptExpressionContext context)
    {
        if (context.qualifiedIdentifier() != null)
        {
            var qualified = IdentifierVisitor.VisitQualifiedIdentifier(context.qualifiedIdentifier());
            
            var column = new QsiColumnReferenceNode { Name = qualified };
            PostgreSqlTree.PutContextSpan(column, context);

            var expressionNode = new QsiColumnExpressionNode { Column = { Value = column } };
            PostgreSqlTree.PutContextSpan(expressionNode, context);

            return expressionNode;
        }

        if (context.subscriptExpression() != null)
        {
            var expression = VisitSubscriptExpression(context.subscriptExpression());
            var subscriptIndirection = VisitSubscriptIndirection(context.subscriptIndirection());

            var expressionNode = new QsiMemberAccessExpressionNode
            {
                Target = { Value = expression },
                Member = { Value = subscriptIndirection }
            };
            
            PostgreSqlTree.PutContextSpan(expressionNode, context);

            return expressionNode;
        }

        var target = context.expressionParens() != null ?
            VisitExpressionParens(context.expressionParens()) :
            VisitParam(context.parameterMarker());

        if (context.indirection() == null)
        {
            return target;
        }
        
        var indirection = VisitIndirection(context.indirection());

        var node = new QsiMemberAccessExpressionNode
        {
            Target = { Value = target },
            Member = { Value = indirection }
        };
            
        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static QsiExpressionNode VisitSubscriptIndirection(SubscriptIndirectionContext context)
    {
        var subscript = VisitSubscript(context.subscript());

        if (context.indirection() == null)
        {
            PostgreSqlTree.PutContextSpan(subscript, context);
            return subscript;
        }

        var indirection = VisitIndirection(context.indirection());

        var node = new QsiMemberAccessExpressionNode
        {
            Target = { Value = subscript },
            Member = { Value = indirection }
        };

        return node;
    }

    public static PostgreSqlSubscriptExpressionNode VisitSubscript(SubscriptContext context)
    {
        var node = new PostgreSqlSubscriptExpressionNode();

        if (context.expression().Length == 1)
        {
            node.Index.Value = VisitExpression(context.expression(0));
        }
        else
        {
            node.Start.Value = VisitExpression(context.expression(0));
            node.End.Value = VisitExpression(context.expression(1));
        }
        
        PostgreSqlTree.PutContextSpan(node, context);
        
        return node;
    }
    
    public static QsiMultipleOrderExpressionNode VisitOrderByClause(OrderByClauseContext context)
    {
        var orders = context.orderList().orderExpression();
        
        var node = new QsiMultipleOrderExpressionNode();
        node.Orders.AddRange(orders.Select(VisitOrderExpression));
        
        PostgreSqlTree.PutContextSpan(node, context);
        return node;
    }

    public static QsiOrderExpressionNode VisitOrderExpression(OrderExpressionContext context)
    {
        var node = new QsiOrderExpressionNode
        {
            Order = context.DESC() != null ? QsiSortOrder.Descending : QsiSortOrder.Ascending
        };

        node.Expression.Value = VisitExpression(context.expression());

        PostgreSqlTree.PutContextSpan(node, context);
        
        return node;
    }

    public static QsiLimitExpressionNode VisitLimitClause(LimitClauseContext context)
    {
        var node = new QsiLimitExpressionNode();

        if (context.limit() != null)
        {
            node.Limit.Value = VisitExpression(context.limit().expression());
        }

        if (context.offset() != null)
        {
            node.Offset.Value = VisitExpression(context.offset().expression());
        }
        
        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static QsiWhereExpressionNode VisitWhereClause(WhereClauseContext context)
    {
        var node = new QsiWhereExpressionNode();
        node.Expression.Value = VisitExpression(context.expression());
        
        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static QsiGroupingExpressionNode VisitGroupByClause(GroupByClauseContext context)
    {
        var groups = context.groupByItemList().groupByItem();
        
        var node = new QsiGroupingExpressionNode();
        node.Items.AddRange(groups.Select(VisitGroupByItem).Where(i => i != null));
        
        PostgreSqlTree.PutContextSpan(node, context);
        return node;
    }
    
    public static QsiExpressionNode VisitGroupByItem(GroupByItemContext context)
    {
        if (context.expression() != null)
        {
            return VisitExpression(context.expression());
        }

        if (context.expressionList() != null)
        {
            return VisitExpressionList(context.expressionList());
        }

        if (context.groupByItemList() != null)
        {
            var groups = context.groupByItemList().groupByItem();
            var node = new QsiGroupingExpressionNode();
            node.Items.AddRange(groups.Select(VisitGroupByItem));
        
            PostgreSqlTree.PutContextSpan(node, context);
            return node;
        }

        return null;
    }

    public static QsiTypeExpressionNode VisitType(TypeContext context)
    {
        var identifier = new QsiIdentifier(context.GetText(), false);
        var qualified = new QsiQualifiedIdentifier(identifier);
        var node = new QsiTypeExpressionNode { Identifier = qualified };

        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static IEnumerable<QsiSetColumnExpressionNode> VisitUpdateSetList(UpdateSetListContext context)
    {
        return context.updateSet().Select(VisitUpdateSet);
    }

    public static QsiSetColumnExpressionNode VisitUpdateSet(UpdateSetContext context)
    {
        return context switch
        {
            ColumnUpdateSetContext columnUpdate => VisitColumnUpdateSet(columnUpdate),
            _ => throw TreeHelper.NotSupportedTree(context)
        };
    }

    public static QsiSetColumnExpressionNode VisitColumnUpdateSet(ColumnUpdateSetContext context)
    {
        var identifier = IdentifierVisitor.VisitIdentifier(context.columnIdentifier());
        var qualified = new QsiQualifiedIdentifier(identifier);

        var node = new QsiSetColumnExpressionNode
        {
            Target = qualified
        };

        var value = context.expression() != null ?
            VisitExpression(context.expression()) :
            TreeHelper.CreateDefaultLiteral();

        node.Value.Value = value;
        
        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static QsiExpressionNode VisitIndirection(IndirectionContext context)
    {
        var firstIdentifier = IdentifierVisitor.VisitIdentifier(context.columnLabelIdentifier(0));
        var firstQualified = new QsiQualifiedIdentifier(firstIdentifier);
        
        QsiExpressionNode anchor = new QsiFieldExpressionNode { Identifier = firstQualified };

        ColumnLabelIdentifierContext[] labels = context.columnLabelIdentifier();

        for (var i = 1; i < labels.Length; i++)
        {
            var label = labels[i];
            var identifier = IdentifierVisitor.VisitIdentifier(label);
            var qualified = new QsiQualifiedIdentifier(identifier);
            
            var name = new QsiFieldExpressionNode { Identifier = qualified };
            PostgreSqlTree.PutContextSpan(name, label);

            var node = new QsiMemberAccessExpressionNode
            {
                Target = { Value = anchor },
                Member = { Value = name }
            };

            PostgreSqlTree.PutContextSpan(node, context.Start, label.Stop);

            anchor = node;
        }

        return anchor;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.Oracle.Internal;
using Qsi.Shared.Extensions;
using Qsi.Tree;

namespace Qsi.Oracle.Tree.Visitors
{
    using static OracleParserInternal;

    internal static class ExpressionVisitor
    {
        public static IEnumerable<QsiColumnNode> VisitSelectList(SelectListContext context)
        {
            if (context.HasToken(MULT_SYMBOL))
                yield return OracleTree.CreateWithSpan<QsiAllColumnNode>(context);

            foreach (var selectListItem in context.selectListItem())
                yield return VisitSelectListItem(selectListItem);
        }

        public static QsiColumnNode VisitSelectListItem(SelectListItemContext context)
        {
            switch (context)
            {
                case ObjectSelectListItemContext objectSelectListItem:
                    var allNode = OracleTree.CreateWithSpan<QsiAllColumnNode>(objectSelectListItem);
                    allNode.Path = IdentifierVisitor.CreateQualifiedIdentifier(objectSelectListItem.identifier());

                    return allNode;

                case ExprSelectListItemContext exprSelectListItem:
                    QsiColumnNode columnNode;

                    var exprNode = VisitExpr(exprSelectListItem.expr());

                    if (exprNode is QsiColumnExpressionNode columnExpressionNode)
                    {
                        columnNode = columnExpressionNode.Column.Value;
                    }
                    else
                    {
                        var node = OracleTree.CreateWithSpan<QsiDerivedColumnNode>(exprSelectListItem);
                        node.Expression.Value = exprNode;

                        columnNode = node;
                    }

                    if (exprSelectListItem.alias() != null)
                    {
                        if (columnNode is not QsiDerivedColumnNode qsiDerivedColumn)
                        {
                            qsiDerivedColumn = OracleTree.CreateWithSpan<QsiDerivedColumnNode>(exprSelectListItem);
                            qsiDerivedColumn.Column.Value = columnNode;

                            columnNode = qsiDerivedColumn;
                        }

                        qsiDerivedColumn.Alias.Value = new QsiAliasNode
                        {
                            Name = IdentifierVisitor.VisitAlias(exprSelectListItem.alias())
                        };
                    }

                    return columnNode;
            }

            throw new NotSupportedException();
        }

        public static QsiExpressionNode VisitExpr(ExprContext context)
        {
            while (context is ParenthesisExprContext parens)
                context = parens.expr();

            return context switch
            {
                SignExprContext signExpr => VisitSignExpr(signExpr),
                TimestampExprContext timestampExpr => VisitTimestampExpr(timestampExpr),
                BinaryExprContext binaryExpr => VisitBinaryExpr(binaryExpr),
                CollateExprContext collateExpr => VisitCollateExpr(collateExpr),
                FunctionExprContext functionExpr => VisitFunctionExpr(functionExpr.functionExpression()),
                CalcMeasExprContext calcMeasExpr => VisitCalcMeasExpr(calcMeasExpr.avMeasExpression()),
                CaseExprContext caseExpr => VisitCaseExpr(caseExpr.caseExpression()),
                CursorExprContext cursorExpr => VisitCursorExpr(cursorExpr),
                IntervalExprContext intervalExpr => VisitIntervalExpr(intervalExpr.intervalExpression()),
                ModelExprContext modelExpr => VisitModelExpr(modelExpr.modelExpression()),
                ObjectAccessExprContext objectAccessExpr => VisitObjectAccessExpr(objectAccessExpr.objectAccessExpression()),
                PlaceholderExprContext placeholderExpr => VisitPlaceholderExpr(placeholderExpr.placeholderExpression()),
                ScalarSubqueryExprContext scalarSubqueryExpr => VisitScalarSubqueryExpr(scalarSubqueryExpr),
                TypeConstructorExprContext typeConstructorExpr => VisitTypeConstructorExpr(typeConstructorExpr.typeConstructorExpression()),
                DatetimeExprContext datetimeExpr => VisitDatetimeExpr(datetimeExpr),
                SimpleExprContext simpleExpr => VisitSimpleExpr(simpleExpr.simpleExpression()),
                BindVariableExprContext bindVariableExpr => VisitBindVariable(bindVariableExpr.bindVariable()),
                MultisetExceptExprContext multisetExceptExpr => VisitMultisetExceptExpr(multisetExceptExpr),
                ColumnOuterJoinExprContext columnOuterJoinExpr => VisitColumnOuterJoinExpr(columnOuterJoinExpr),
                _ => throw new NotSupportedException()
            };
        }

        public static QsiExpressionNode VisitParenthesisExpr(ParenthesisExprContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitSignExpr(SignExprContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitTimestampExpr(TimestampExprContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitBinaryExpr(BinaryExprContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitCollateExpr(CollateExprContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitFunctionExpr(FunctionExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitCalcMeasExpr(AvMeasExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitCaseExpr(CaseExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitCursorExpr(CursorExprContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitIntervalExpr(IntervalExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitModelExpr(ModelExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitObjectAccessExpr(ObjectAccessExpressionContext context)
        {
            switch (context)
            {
                case ColumnWithExprAccessExpressionContext columnWithExprAccessExpression:
                    throw new NotSupportedException();

                case ColumnAccessExpressionContext columnAccessExpression:
                {
                    var columnExpressionNode = OracleTree.CreateWithSpan<QsiColumnExpressionNode>(columnAccessExpression);
                    var columnNode = OracleTree.CreateWithSpan<QsiColumnReferenceNode>(columnAccessExpression);
                    columnNode.Name = new QsiQualifiedIdentifier(columnAccessExpression.identifier().Select(IdentifierVisitor.VisitIdentifier));
                    columnExpressionNode.Column.Value = columnNode;

                    return columnExpressionNode;
                }

                case PseudoColumnAccessExpressionContext pseudoColumnAccessExpression:
                {
                    var pseudoColumn = pseudoColumnAccessExpression.pseudoColumn();

                    var columnExpressionNode = OracleTree.CreateWithSpan<QsiColumnExpressionNode>(pseudoColumnAccessExpression);
                    var columnNode = OracleTree.CreateWithSpan<QsiColumnReferenceNode>(pseudoColumn);
                    var text = pseudoColumn.GetText();
                    var identifier = new QsiIdentifier(text, false);
                    columnNode.Name = new QsiQualifiedIdentifier(identifier);

                    columnExpressionNode.Column.Value = columnNode;

                    return columnExpressionNode;
                }
            }

            throw new NotSupportedException();
        }

        public static QsiExpressionNode VisitPlaceholderExpr(PlaceholderExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitScalarSubqueryExpr(ScalarSubqueryExprContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitTypeConstructorExpr(TypeConstructorExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitDatetimeExpr(DatetimeExprContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitSimpleExpr(SimpleExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitBindVariable(BindVariableContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitMultisetExceptExpr(MultisetExceptExprContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitColumnOuterJoinExpr(ColumnOuterJoinExprContext context)
        {
            throw new NotImplementedException();
        }
    }
}

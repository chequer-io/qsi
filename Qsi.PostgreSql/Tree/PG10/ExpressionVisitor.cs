using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qsi.Data;
using Qsi.PostgreSql.Internal;
using Qsi.PostgreSql.Internal.PG10.Types;
using Qsi.Tree.Base;
using Qsi.Utilities;

namespace Qsi.PostgreSql.Tree.PG10
{
    internal static class ExpressionVisitor
    {
        public static QsiExpressionNode Visit(IPg10Node node)
        {
            switch (node)
            {
                case A_Expr aExpr:
                    return VisitAtomicExpression(aExpr);

                case A_Const aConst:
                    return VisitAtomicConstant(aConst);

                case A_ArrayExpr arrayExpr:
                    return VisitAtomicArrayExpression(arrayExpr);

                case FuncCall funcCall:
                    return VisitFunctionCall(funcCall);

                case SubLink subLink:
                    return VisitSubLink(subLink);

                case TypeCast typeCast:
                    return VisitTypeCast(typeCast);

                case TypeName typeName:
                    return VisitTypeName(typeName);

                case Value value:
                    return VisitValue(value);

                case ColumnRef columnRef:
                    return VisitColumnRef(columnRef);

                case CaseExpr caseExpr:
                    return VisitCaseExpression(caseExpr);

                case BoolExpr boolExpr:
                    return VisitBoolExpression(boolExpr);

                case RowExpr rowExpr:
                    return VisitRowExpression(rowExpr);
            }

            throw TreeHelper.NotSupportedTree(node);
        }

        private static QsiExpressionNode VisitExpressions(IPg10Node[] expressions)
        {
            if (expressions.Length == 1)
                return Visit(expressions[0]);

            return TreeHelper.Create<QsiArrayExpressionNode>(n =>
            {
                n.Elements.AddRange(expressions.Select(Visit));
            });
        }

        private static QsiExpressionNode VisitAtomicExpression(A_Expr expression)
        {
            var builder = new StringBuilder();

            switch (expression.kind)
            {
                case A_Expr_Kind.AEXPR_OP_ANY:
                    builder.Append("ANY ");
                    break;

                case A_Expr_Kind.AEXPR_OP_ALL:
                    builder.Append("ALL ");
                    break;

                default:
                    if (expression.name?.Length == 1 && expression.name[0] is PgString pgString)
                        builder.Append(pgString.str).Append(' ');

                    break;
            }

            if (expression.kind != A_Expr_Kind.AEXPR_OP)
                builder.Append(expression.kind.ToString()[6..]);

            return TreeHelper.CreateLogicalExpression(
                builder.ToString(),
                expression.lexpr,
                expression.rexpr,
                VisitExpressions);
        }

        public static QsiExpressionNode VisitAtomicConstant(A_Const constant)
        {
            return VisitValue(constant.val);
        }

        private static QsiExpressionNode VisitAtomicArrayExpression(A_ArrayExpr arrayExpression)
        {
            return TreeHelper.Create<QsiArrayExpressionNode>(n =>
            {
                n.Elements.AddRange(arrayExpression.elements.Select(Visit));
            });
        }

        private static QsiExpressionNode VisitValue(Value value)
        {
            return TreeHelper.Create<QsiLiteralExpressionNode>(n =>
            {
                n.Type = value.Type switch
                {
                    NodeTag.T_Integer => QsiLiteralType.Numeric,
                    NodeTag.T_Float => QsiLiteralType.Decimal,
                    NodeTag.T_String => QsiLiteralType.String,
                    NodeTag.T_BitString => QsiLiteralType.String,
                    _ => throw TreeHelper.NotSupportedTree(value)
                };

                if (value.Type == NodeTag.T_Integer)
                {
                    n.Value = value.ival;
                }
                else
                {
                    n.Value = value.str;
                }
            });
        }

        public static QsiExpressionNode VisitFunctionCall(FuncCall funcCall)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(new QsiFunctionAccessExpressionNode
                {
                    Identifier = IdentifierVisitor.VisitStrings(funcCall.funcname.Cast<PgString>())
                });

                foreach (var arg in funcCall.args ?? Array.Empty<IPg10Node>())
                {
                    n.Parameters.Add(Visit(arg));
                }

                if (funcCall.agg_star ?? false)
                {
                    n.Parameters.Add(TreeHelper.Create<QsiColumnAccessExpressionNode>(node =>
                    {
                        node.IsAll = true;
                    }));
                }
            });
        }

        private static QsiExpressionNode VisitSubLink(SubLink subLink)
        {
            switch (subLink.subLinkType)
            {
                case SubLinkType.EXPR_SUBLINK:
                {
                    return TreeHelper.Create<QsiTableExpressionNode>(n =>
                    {
                        n.Table.SetValue(TableVisitor.VisitSelectStmt((SelectStmt)subLink.subselect[0]));
                    });
                }
            }

            throw TreeHelper.NotSupportedTree(subLink.subLinkType);
        }

        private static QsiExpressionNode VisitTypeCast(TypeCast typeCast)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(new QsiFunctionAccessExpressionNode
                {
                    Identifier = new QsiQualifiedIdentifier(new QsiIdentifier("CAST", false))
                });

                n.Parameters.Add(Visit(typeCast.arg[0]));
                n.Parameters.AddRange(typeCast.typeName.Select(VisitTypeName));
            });
        }

        private static QsiParametersExpressionNode VisitTypeName(TypeName typeName)
        {
            return TreeHelper.Create<QsiParametersExpressionNode>(n =>
            {
                var identifier = IdentifierVisitor.VisitStrings(typeName.names.Cast<PgString>());

                if (typeName.arrayBounds != null)
                {
                    QsiIdentifier[] identifiers = identifier.ToArray();

                    var bounds = new StringBuilder();
                    bounds.Append(identifier[^1].Value);

                    foreach (var bound in typeName.arrayBounds.OfType<PgInteger>())
                        bounds.Append(bound.ival >= 0 ? $"[{bound.ival}]" : "[]");

                    identifiers[^1] = new QsiIdentifier(bounds.ToString(), false);
                    identifier = new QsiQualifiedIdentifier(identifiers);
                }

                n.Expressions.Add(new QsiUnknownMemberAcessExpressionNode
                {
                    Identifier = identifier
                });
            });
        }

        private static QsiExpressionNode VisitColumnRef(ColumnRef columnRef)
        {
            return TreeHelper.Create<QsiColumnAccessExpressionNode>(n =>
            {
                var identifier = IdentifierVisitor.VisitColumnRef(columnRef);

                if (identifier[^1].Value == "*")
                {
                    n.IsAll = true;
                    n.Identifier = identifier.Level == 1 ? null : new QsiQualifiedIdentifier(identifier[..^1]);
                }
                else
                {
                    n.Identifier = identifier;
                }
            });
        }

        private static QsiExpressionNode VisitCaseExpression(CaseExpr caseExpr)
        {
            var switchExp = new QsiSwitchExpressionNode();

            // CASE <arg>
            if (caseExpr.arg?.Length > 0)
            {
                switchExp.Value.SetValue(Visit(caseExpr.arg[0]));
            }

            // WHEN <expr> THEN <result> 
            foreach (var caseWhen in caseExpr.args.Cast<CaseWhen>())
            {
                switchExp.Cases.Add(
                    TreeHelper.Create<QsiSwitchCaseExpressionNode>(n =>
                    {
                        if (caseWhen.expr?.Length == 1)
                            n.Condition.SetValue(Visit(caseWhen.expr[0]));

                        if (caseWhen.result?.Length == 1)
                            n.Consequent.SetValue(Visit(caseWhen.result[0]));
                    }));
            }

            // ELSE <else>
            foreach (var defResult in caseExpr.defresult ?? Array.Empty<IPg10Node>())
            {
                switchExp.Cases.Add(
                    TreeHelper.Create<QsiSwitchCaseExpressionNode>(n =>
                    {
                        n.Consequent.SetValue(Visit(defResult));
                    }));
            }

            return switchExp;
        }

        // <expr> OP <expr> OP <expr> ..
        private static QsiExpressionNode VisitBoolExpression(BoolExpr boolExpr)
        {
            string op = boolExpr.boolop.ToString().Split("_", 2)[0];

            var anchorExpr = TreeHelper.CreateLogicalExpression(op, boolExpr.args[0], boolExpr.args[1], Visit);

            foreach (var arg in boolExpr.args.Skip(2))
            {
                var expr = new QsiLogicalExpressionNode
                {
                    Operator = op
                };

                expr.Left.SetValue(anchorExpr);
                expr.Right.SetValue(Visit(arg));
                anchorExpr = expr;
            }

            return anchorExpr;
        }

        // ROW(..)
        private static QsiExpressionNode VisitRowExpression(RowExpr rowExpr)
        {
            if (rowExpr.colnames?.Length > 0)
                throw TreeHelper.NotSupportedTree(rowExpr);

            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(new QsiFunctionAccessExpressionNode
                {
                    Identifier = new QsiQualifiedIdentifier(new QsiIdentifier("ROW", false))
                });

                n.Parameters.AddRange(rowExpr.args.Select(Visit));
            });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using PgQuery;
using Qsi.Data;
using Qsi.PostgreSql.NewTree.Nodes;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PostgreSql.NewTree;

internal static partial class PgNodeVisitor
{
    public static IQsiTreeNode VisitExpression(IPgExpressionNode node)
    {
        return node switch
        {
            FuncCall funcCall => Visit(funcCall),
            XmlExpr xmlExpr => Visit(xmlExpr),
            TypeCast typeCast => Visit(typeCast),
            A_Const aConst => Visit(aConst),
            ColumnRef columnRef => Visit(columnRef),
            A_Expr aExpr => Visit(aExpr),
            CaseExpr caseExpr => Visit(caseExpr),
            CaseWhen caseWhen => Visit(caseWhen),
            A_ArrayExpr aArrayExpr => Visit(aArrayExpr),
            NullTest nullTest => Visit(nullTest),
            XmlSerialize xmlSerialize => Visit(xmlSerialize),
            ParamRef paramRef => Visit(paramRef),
            BoolExpr boolExpr => Visit(boolExpr),
            SubLink subLink => Visit(subLink),
            RowExpr rowExpr => Visit(rowExpr),
            CoalesceExpr coalesceExpr => Visit(coalesceExpr),
            SetToDefault setToDefault => Visit(setToDefault),
            A_Indirection aIndirection => Visit(aIndirection),
            CollateClause collateClause => Visit(collateClause),
            CurrentOfExpr currentOfExpr => Visit(currentOfExpr),
            SQLValueFunction sqlValueFunction => Visit(sqlValueFunction),
            MinMaxExpr minMaxExpr => Visit(minMaxExpr),
            BooleanTest booleanTest => Visit(booleanTest),
            GroupingFunc groupingFunc => Visit(groupingFunc),
            List list => Visit(list),
            NamedArgExpr namedArgExpr => Visit(namedArgExpr),
            JoinExpr joinExpr => Visit(joinExpr),
            CommonTableExpr commonTableExpr => Visit(commonTableExpr),
            _ => throw new NotSupportedException($"Not supported expression node: '{node.GetType().Name}'")
        };
    }

    public static QsiExpressionNode Visit(A_Expr node)
    {
        var left = node.Lexpr is null ? null : VisitExpression(node.Lexpr);
        var right = node.Rexpr is null ? null : VisitExpression(node.Rexpr);

        if (right is null)
            throw CreateInternalException("Atomic expression right expression is null");

        if (left is null)
        {
            return new QsiUnaryExpressionNode
            {
                Expression = { Value = right },
                Operator = GetOperator()
            };
        }

        return new QsiBinaryExpressionNode
        {
            Left = { Value = left },
            Right = { Value = right },
            Operator = GetOperator()
        };

        string GetOperator()
        {
            var kind = node.Kind;
            var builder = new StringBuilder();

            switch (kind)
            {
                case A_Expr_Kind.AexprOpAny:
                    builder.Append("ANY ");
                    break;

                case A_Expr_Kind.AexprOpAll:
                    builder.Append("ALL ");
                    break;

                default:
                    builder.AppendJoin(' ', node.Name.Select(n => n.String.Sval));
                    break;
            }

            if (kind is not A_Expr_Kind.AexprOp)
            {
                if (builder.Length != 0)
                    builder.Append(' ');

                builder.Append(kind.ToString()[5..]);
            }

            Console.WriteLine(builder.ToString());
            return builder.ToString();
        }
    }

    // GROUPING( expr[, expr].. )
    public static QsiInvokeExpressionNode Visit(GroupingFunc node)
    {
        return new PgInvokeExpressionNode
        {
            Member = { Value = TreeHelper.CreateFunction("GROUPING") },
            Parameters = { node.Args.Select(VisitExpression) },
            IsBulitIn = true
        };
    }

    public static PgNamedParameterExpressionNode Visit(NamedArgExpr node)
    {
        return new PgNamedParameterExpressionNode
        {
            Name = node.Name,
            Expression = { Value = VisitExpression(node.Arg) }
        };
    }

    // Explicit: ROW (1,2)
    // Implicit: (1,2)
    public static PgRowValueExpressionNode Visit(RowExpr node)
    {
        return new PgRowValueExpressionNode
        {
            ColumnValues = { node.Args.Select(VisitExpression) },
            IsExplicit = node.RowFormat is CoercionForm.CoerceExplicitCall
        };
    }

    // COALESCE( expr[, expr].. )
    public static IQsiTreeNode Visit(CoalesceExpr node)
    {
        return new PgInvokeExpressionNode
        {
            Member = { Value = TreeHelper.CreateFunction("COALESCE") },
            Parameters = { node.Args.Select(VisitExpression) },
            IsBulitIn = true
        };
    }

    // GREATEST ( expr[, expr].. )
    // LEAST ( expr[, expr].. )
    public static IQsiTreeNode Visit(MinMaxExpr node)
    {
        return new PgInvokeExpressionNode
        {
            Member = { Value = TreeHelper.CreateFunction(node.Op is MinMaxOp.IsGreatest ? "GREATEST" : "LEAST") },
            Parameters = { node.Args.Select(VisitExpression) },
            IsBulitIn = true
        };
    }

    public static PgSqlValueInvokeExpressionNode Visit(SQLValueFunction node)
    {
        string functionName = node.Op switch
        {
            SQLValueFunctionOp.SvfopCurrentDate => "CURRENT_DATE",
            SQLValueFunctionOp.SvfopCurrentTime => "CURRENT_TIME",
            SQLValueFunctionOp.SvfopCurrentTimeN => "CURRENT_TIME",
            SQLValueFunctionOp.SvfopCurrentTimestamp => "CURRENT_TIMESTAMP",
            SQLValueFunctionOp.SvfopCurrentTimestampN => "CURRENT_TIMESTAMP",
            SQLValueFunctionOp.SvfopLocaltime => "LOCALTIME",
            SQLValueFunctionOp.SvfopLocaltimeN => "LOCALTIME",
            SQLValueFunctionOp.SvfopLocaltimestamp => "LOCALTIMESTAMP",
            SQLValueFunctionOp.SvfopLocaltimestampN => "LOCALTIMESTAMP",
            SQLValueFunctionOp.SvfopCurrentRole => "CURRENT_ROLE",
            SQLValueFunctionOp.SvfopCurrentUser => "CURRENT_USER",
            SQLValueFunctionOp.SvfopSessionUser => "SESSION_USER",
            SQLValueFunctionOp.SvfopUser => "USER",
            SQLValueFunctionOp.SvfopCurrentCatalog => "CURRENT_CATALOG",
            SQLValueFunctionOp.SvfopCurrentSchema => "CURRENT_SCHEMA",

            _ => throw new NotSupportedException($"Not supported function operator: {node.Op}")
        };

        return new PgSqlValueInvokeExpressionNode
        {
            Member = { Value = TreeHelper.CreateFunction(functionName) },
            Parameters =
            {
                node.Typmod is -1
                    ? Enumerable.Empty<QsiExpressionNode>()
                    : new[]
                    {
                        new QsiLiteralExpressionNode
                        {
                            Value = node.Typmod,
                            Type = QsiDataType.Numeric
                        }
                    }
            },
            IsBulitIn = true
        };
    }

    public static IQsiTreeNode Visit(XmlExpr node)
    {
        throw TreeHelper.NotSupportedFeature("Xml Expression");
    }

    // Arg IS [NOT] {TRUE | FALSE | UNKNOWN}
    public static PgBooleanTestExpressionNode Visit(BooleanTest node)
    {
        return new PgBooleanTestExpressionNode
        {
            Target = { Value = VisitExpression(node.Arg) },
            BoolTestType = node.Booltesttype
        };
    }

    // WHERE CURRENT OF {cursor_name}
    public static IQsiTreeNode Visit(CurrentOfExpr node)
    {
        throw TreeHelper.NotSupportedFeature("Current of cursor");
    }

    public static IQsiTreeNode Visit(ParamRef node)
    {
        var bindParameter = new QsiBindParameterExpressionNode
        {
            Type = QsiParameterType.Index
        };

        if (node.Number > 0)
        {
            // $1
            bindParameter.Prefix = "$";
            bindParameter.Index = node.Number - 1;
        }
        else
        {
            // ?
            bindParameter.Prefix = "?";
            bindParameter.NoSuffix = true;
        }

        return bindParameter;
    }

    // {Arg} COLLATE {CollName}
    public static PgCollateExpressionNode Visit(CollateClause node)
    {
        return new PgCollateExpressionNode
        {
            Column = CreateQualifiedIdentifier(node.Collname),
            Expression = { Value = VisitExpression(node.Arg) }
        };
    }

    public static IQsiTreeNode Visit(XmlSerialize node)
    {
        throw TreeHelper.NotSupportedFeature("Xml Serialize");
    }

    // {Source} :: {Type}
    // CAST({Source} AS {Type})
    public static PgCastExpressionNode Visit(TypeCast node)
    {
        return new PgCastExpressionNode
        {
            Source = { Value = VisitExpression(node.Arg) },
            Type = { Value = Visit(node.TypeName) }
        };
    }

    public static PgTypeExpressionNode Visit(TypeName node)
    {
        return new PgTypeExpressionNode
        {
            Identifier = CreateQualifiedIdentifier(node.Names),
            PctType = node.PctType,
            Setof = node.Setof,
            TypMods = { node.Typmods.Select(VisitExpression)! },
            ArrayBounds = { node.ArrayBounds.Select(VisitExpression)! }
        };
    }

    public static PgIndirectionExpressionNode Visit(A_Indirection node)
    {
        return new PgIndirectionExpressionNode
        {
            Target = { Value = new QsiDerivedColumnNode { Expression = { Value = VisitExpression(node.Arg) } } },
            Indirections = { node.Indirection.Select(VisitExpression)! }
        };
    }

    public static PgIndexExpressionNode Visit(A_Indices node)
    {
        var indexExpr = new PgIndexExpressionNode
        {
            Index = { Value = VisitExpression(node.Uidx) }
        };

        if (node.IsSlice)
            indexExpr.IndexEnd.Value = VisitExpression(node.Lidx);

        return indexExpr;
    }

    public static PgInvokeExpressionNode Visit(FuncCall node)
    {
        return new PgInvokeExpressionNode
        {
            Member = { Value = CreateFunction(node.Funcname) },
            FunctionFormat = node.Funcformat,
            Parameters = { node.Args.Select(VisitExpression) },
            AggregateStar = node.AggStar,
            AggregateDistinct = node.AggDistinct,
            AggregateOrder = { node.AggOrder.Select(VisitExpression) },
            AggregateFilter = { Value = VisitExpression(node.AggFilter) },
            AggregateWithInGroup = node.AggWithinGroup,
            FunctionVariadic = node.FuncVariadic,
            Over = { Value = Visit(node.Over) },
            IsBulitIn = false
        };
    }

    public static PgSubLinkExpressionNode Visit(SubLink node)
    {
        // NOTE: SubLinkId is used for PG internal optimizer.

        var sublink = new PgSubLinkExpressionNode
        {
            Expression = { Value = VisitExpression(node.Testexpr) },
            Table = { Value = Visit<QsiTableNode>(node.Subselect) },
            OperatorName = string.Join(' ', node.OperName.Select(v => v.String.Sval)),
            SubLinkType = node.SubLinkType switch
            {
                SubLinkType.ExprSublink => string.Empty, // (subselect)
                SubLinkType.AllSublink => "ALL", // testexpr opername ALL (subselect)
                SubLinkType.AnySublink => "ANY", // testexpr opername {ANY | SOME} (subselect)
                SubLinkType.ExistsSublink => "EXISTS", // EXISTS (subselect)
                SubLinkType.ArraySublink => "ARRAY", // ARRAY (subselect)
                _ => throw CreateInternalException($"Not supported SubLinkType: {node.SubLinkType}")
            }
        };

        return sublink;
    }

    public static IQsiTreeNode Visit(BoolExpr node)
    {
        var op = node.Boolop switch
        {
            BoolExprType.AndExpr => "AND",
            BoolExprType.NotExpr => "NOT",
            BoolExprType.OrExpr => "OR",
            _ => throw new NotSupportedException($"Not supported BoolExprType.{node.Boolop}")
        };

        var expr = VisitExpression(node.Args[0]);

        if (node.Args.Count == 1)
            return TreeHelper.CreateUnary(op, expr);

        for (int i = 1; i < node.Args.Count; i++)
        {
            expr = new QsiBinaryExpressionNode
            {
                Operator = op,
                Left = { Value = expr },
                Right = { Value = VisitExpression(node.Args[i]) }
            };
        }

        return expr;
    }

    // <Arg> IS [NOT] NULL
    public static PgNullTestExpressionNode Visit(NullTest node)
    {
        return new PgNullTestExpressionNode
        {
            IsNot = node.Nulltesttype == NullTestType.IsNotNull,
            Target = { Value = VisitExpression(node.Arg) }
        };
    }

    public static QsiMultipleExpressionNode Visit(A_ArrayExpr node)
    {
        return new QsiMultipleExpressionNode
        {
            Elements = { node.Elements.Select(VisitExpression) }
        };
    }

    public static QsiAllColumnNode Visit(A_Star node)
    {
        return new QsiAllColumnNode();
    }

    public static QsiRowValueExpressionNode Visit(List node)
    {
        return CreateRowValueExpression(node.Items);
    }

    public static QsiSwitchExpressionNode Visit(CaseExpr node)
    {
        var switchExp = new QsiSwitchExpressionNode();

        // CASE <arg>
        if (node.Arg is not null)
            switchExp.Value.Value = VisitExpression(node.Arg);

        // WHEN <expr> THEN <result> 
        switchExp.Cases.AddRange(node.Args.Select(i => i.CaseWhen).Select(Visit));

        // ELSE <else>
        if (node.Defresult is { } defResult)
            switchExp.Cases.Add(new QsiSwitchCaseExpressionNode { Consequent = { Value = VisitExpression(defResult) } });

        return switchExp;
    }

    public static QsiSwitchCaseExpressionNode Visit(CaseWhen node)
    {
        return new QsiSwitchCaseExpressionNode
        {
            Condition = { Value = VisitExpression(node.Expr) },
            Consequent = { Value = VisitExpression(node.Result) }
        };
    }

    public static PgDefaultExpressionNode Visit(SetToDefault node)
    {
        return new PgDefaultExpressionNode();
    }

    public static PgWindowDefExpressionNode Visit(WindowDef node)
    {
        return new PgWindowDefExpressionNode
        {
            Name = CreateIdentifier(node.Name),
            Refname = CreateIdentifier(node.Refname),
            PartitionClause = { node.PartitionClause.Select(VisitExpression) },
            OrderClause = { node.OrderClause.Select(VisitExpression) },
            FrameOptions = (FrameOptions)node.FrameOptions,
            StartOffset = { Value = VisitExpression(node.StartOffset) },
            EndOffset = { Value = VisitExpression(node.EndOffset) }
        };
    }

    private static QsiRowValueExpressionNode CreateRowValueExpression(IEnumerable<Node> items)
    {
        return new QsiRowValueExpressionNode
        {
            ColumnValues = { items.Select(VisitExpression) }
        };
    }

    [return: NotNullIfNotNull("exprNode")]
    private static QsiExpressionNode? VisitExpression(Node? exprNode)
    {
        if (exprNode is null)
            return null;

        return Visit(exprNode) switch
        {
            QsiExpressionNode qsiExprNode => qsiExprNode,
            QsiColumnNode qsiColumnNode => new QsiColumnExpressionNode { Column = { Value = qsiColumnNode } },
            QsiTableNode qsiTableNode => new QsiTableExpressionNode { Table = { Value = qsiTableNode } },
            _ => throw CreateInternalException($"Cannot convert '{exprNode.NodeCase}' to expression")
        };
    }
}

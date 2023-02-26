using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using PgQuery;
using Qsi.Data;
using Qsi.PostgreSql.Extensions;
using Qsi.PostgreSql.Tree.Nodes;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PostgreSql.Tree;

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
        var op = string.Join('.', node.Name.Select(n => n.String.Sval));

        if (right is null)
            throw CreateInternalException("Atomic expression right expression is null");

        if (left is null)
        {
            return new PgUnaryExpressionNode
            {
                Expression = { Value = right },
                Operator = op,
                ExprKind = node.Kind
            };
        }

        return new PgBinaryExpressionNode
        {
            Left = { Value = left },
            Right = { Value = right },
            Operator = op,
            ExprKind = node.Kind
        };
    }

    // GROUPING( expr[, expr].. )
    public static PgInvokeExpressionNode Visit(GroupingFunc node)
    {
        return new PgInvokeExpressionNode
        {
            Member = { Value = TreeHelper.CreateFunction("GROUPING") },
            Parameters = { node.Args.Select(VisitExpression) },
            IsBuiltIn = true
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
    public static PgInvokeExpressionNode Visit(CoalesceExpr node)
    {
        return new PgInvokeExpressionNode
        {
            Member = { Value = TreeHelper.CreateFunction("COALESCE") },
            Parameters = { node.Args.Select(VisitExpression) },
            IsBuiltIn = true
        };
    }

    // GREATEST ( expr[, expr].. )
    // LEAST ( expr[, expr].. )
    public static PgInvokeExpressionNode Visit(MinMaxExpr node)
    {
        return new PgInvokeExpressionNode
        {
            Member = { Value = TreeHelper.CreateFunction(node.Op is MinMaxOp.IsGreatest ? "GREATEST" : "LEAST") },
            Parameters = { node.Args.Select(VisitExpression) },
            IsBuiltIn = true
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
            FunctionOp = node.Op,
            IsBuiltIn = true
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
            Member =
            {
                Value = new QsiFunctionExpressionNode
                {
                    Identifier = CreateQualifiedIdentifier(node.Funcname)
                }
            },
            FunctionFormat = node.Funcformat,
            Parameters = { node.Args.Select(VisitExpression) },
            AggregateStar = node.AggStar,
            AggregateDistinct = node.AggDistinct,
            AggregateOrder = { node.AggOrder.Select(VisitExpression) },
            AggregateFilter = { Value = VisitExpression(node.AggFilter) },
            AggregateWithInGroup = node.AggWithinGroup,
            FunctionVariadic = node.FuncVariadic,
            Over = { Value = node.Over is not null ? Visit(node.Over) : null },
            IsBuiltIn = false
        };
    }

    public static PgSubLinkExpressionNode Visit(SubLink node)
    {
        // NOTE: SubLinkId is used for PG internal optimizer.

        return new PgSubLinkExpressionNode
        {
            Expression = { Value = VisitExpression(node.Testexpr) },
            Table = { Value = Visit<QsiTableNode>(node.Subselect) },
            OperatorName = CreateQualifiedIdentifier(node.OperName),
            SubLinkType = node.SubLinkType
        };
    }

    public static IQsiTreeNode Visit(BoolExpr node)
    {
        // NOT <EXPR>
        if (node.Args.Count == 1)
        {
            return new PgUnaryExpressionNode
            {
                Expression = { Value = VisitExpression(node.Args[0]) },
                BoolKind = node.Boolop
            };
        }

        return new PgBooleanMultipleExpressionNode
        {
            Elements = { node.Args.Select(VisitExpression) },
            Type = node.Boolop
        };
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

    public static PgGroupingSetExpressionNode Visit(GroupingSet node)
    {
        return new PgGroupingSetExpressionNode
        {
            Kind = node.Kind,
            Expressions = { node.Content.Select(VisitExpression) }
        };
    }

    public static PgDefinitionElementNode Visit(DefElem node)
    {
        return new PgDefinitionElementNode
        {
            DefinitionName = node.Defname,
            DefinitionNamespace = node.Defnamespace,
            Action = node.Defaction,
            Expression = { Value = VisitExpression(node.Arg) }
        };
    }

    public static PgOnConflictNode Visit(OnConflictClause node)
    {
        return new PgOnConflictNode
        {
            Action = node.Action,
            Infer = { Value = node.Infer is null ? null : Visit(node.Infer) },
            Where = { Value = VisitExpression(node.WhereClause) },
            TargetList = { node.TargetList.Select(VisitExpression) }
        };
    }

    public static PgInferExpressionNode Visit(InferClause node)
    {
        return new PgInferExpressionNode
        {
            Name = node.Conname,
            IndexElems = { node.IndexElems.Select(VisitExpression) },
            Where = { Value = VisitExpression(node.WhereClause) },
        };
    }

    public static PgFunctionParameterExpressionNode Visit(FunctionParameter node)
    {
        return new PgFunctionParameterExpressionNode
        {
            Name = new QsiIdentifier(node.Name, false),
            TypeName = { Value = node.ArgType is null ? null : Visit(node.ArgType) },
            Mode = node.Mode,
            DefinitionExpression = { Value = VisitExpression(node.Defexpr) }
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

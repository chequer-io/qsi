using System;
using System.Linq;
using Qsi.Analyzers.Expression.Models;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Analyzers.Expression;

public class QsiExpressionAnalyzer
{
    public QsiExpression Resolve(TableCompileContext context, IQsiExpressionNode node)
    {
        if (ResolveCore(context, node) is { } expr)
        {
            expr.SetNode(node);
            return expr;
        }

        return null;
    }

    public virtual QsiExpression ResolveCore(TableCompileContext context, IQsiExpressionNode node)
    {
        switch (node)
        {
            case null:
                return null;

            // Expressions
            case IQsiSetColumnExpressionNode setColumn:
                return ResolveSetColumnExpression(context, setColumn);

            case IQsiColumnExpressionNode column:
                return ResolveColumnExpression(context, column);

            case QsiExpressionFragmentNode fragment:
                return ResolveExpressionFragment(context, fragment);

            case IQsiLiteralExpressionNode e:
                return ResolveLiteralExpression(context, e);

            case IQsiBinaryExpressionNode e:
                return ResolveBinaryExpression(context, e);

            case IQsiTableExpressionNode e:
                return ResolveTableExpression(context, e);

            case IQsiUnaryExpressionNode e:
                return ResolveUnaryExpression(context, e);

            case IQsiSetVariableExpressionNode e:
                return ResolveSetVariableExpression(context, e);

            case IQsiInvokeExpressionNode e:
                return ResolveInvokeExpression(context, e);

            case IQsiParametersExpressionNode e:
                return ResolveParametersExpression(context, e);

            case IQsiMultipleExpressionNode e:
                return ResolveMultipleExpression(context, e);

            case IQsiSwitchExpressionNode e:
                return ResolveSwitchExpression(context, e);

            case IQsiSwitchCaseExpressionNode e:
                return ResolveSwitchCaseExpression(context, e);

            case IQsiMemberAccessExpressionNode e:
                return ResolveMemberAccessExpression(context, e);

            case IQsiOrderExpressionNode e:
                return ResolveOrderExpression(context, e);

            case IQsiVariableExpressionNode e:
                return ResolveVariableExpression(context, e);

            case IQsiFunctionExpressionNode e:
                return ResolveFunctionExpression(context, e);

            case IQsiMemberExpressionNode e:
                return ResolveMemberExpression(context, e);

            case IQsiBindParameterExpressionNode e:
                return ResolveBindParameterExpression(context, e);

            default:
                throw new NotSupportedException($"Not supported to resolve expression: '{node.GetType().Name}'");
        }
    }

    protected virtual QsiExpression ResolveSetColumnExpression(TableCompileContext context, IQsiSetColumnExpressionNode node)
    {
        var left = ResolveColumnReferenceAsExpression(context, CreateColumnReferenceNode(node.Target));
        var right = Resolve(context, node.Value);

        return new BinaryExpression(left, right, "=");
    }

    #region Column Expression
    protected virtual QsiExpression ResolveColumnExpression(TableCompileContext context, IQsiColumnExpressionNode node)
    {
        return ResolveColumnAsExpression(context, node.Column);
    }

    protected virtual QsiExpression ResolveColumnAsExpression(TableCompileContext context, IQsiColumnNode node)
    {
        switch (node)
        {
            case IQsiColumnReferenceNode columnReference:
                return ResolveColumnReferenceAsExpression(context, columnReference);

            case IQsiDerivedColumnNode derivedColumn:
            {
                var expr = derivedColumn.IsColumn
                    ? ResolveColumnAsExpression(context, derivedColumn.Column)
                    : Resolve(context, derivedColumn.Expression);

                return new DerivedExpression(expr);
            }

            default:
                throw new NotSupportedException($"Not supported to resolve column expression: '{node.GetType().Name}'");
        }
    }

    protected virtual QsiExpression ResolveColumnReferenceAsExpression(TableCompileContext context, IQsiColumnReferenceNode node)
    {
        var analyzer = context.Engine.GetAnalyzer<QsiTableAnalyzer>();
        QsiTableColumn[] columns = analyzer.ResolveColumnReference(context, node);

        return columns.Length switch
        {
            0 => throw new QsiException(QsiError.UnableResolveColumn, node.Name.ToString()),
            1 => new ColumnExpression(columns[0]),
            _ => throw new NotSupportedException()
        };
    }
    #endregion

    protected virtual QsiExpression ResolveExpressionFragment(TableCompileContext context, QsiExpressionFragmentNode node)
    {
        return new QsiExpression();
    }

    protected virtual QsiExpression ResolveLiteralExpression(TableCompileContext context, IQsiLiteralExpressionNode node)
    {
        return new LiteralExpression(node.Value, node.Type);
    }

    protected virtual QsiExpression ResolveBinaryExpression(TableCompileContext context, IQsiBinaryExpressionNode node)
    {
        var left = Resolve(context, node.Left);
        var right = Resolve(context, node.Right);

        return new BinaryExpression(left, right, node.Operator);
    }

    protected virtual TableExpression ResolveTableExpression(TableCompileContext context, IQsiTableExpressionNode node)
    {
        using var scopedContext = new TableCompileContext(context);
        var structure = context.Engine.GetAnalyzer<QsiTableAnalyzer>().BuildTableStructure(scopedContext, node.Table).Result;

        // NOTE: Add Columns where has expressions
        return new TableExpression(Enumerable.Empty<QsiExpression>(), structure.Filter);
    }

    protected virtual DerivedExpression ResolveUnaryExpression(TableCompileContext context, IQsiUnaryExpressionNode node)
    {
        return new DerivedExpression(Resolve(context, node.Expression));
    }

    private static QsiColumnReferenceNode CreateColumnReferenceNode(QsiQualifiedIdentifier name)
    {
        return new QsiColumnReferenceNode
        {
            Name = name
        };
    }

    protected virtual BinaryExpression ResolveSetVariableExpression(TableCompileContext context, IQsiSetVariableExpressionNode node)
    {
        var left = ResolveColumnAsExpression(context, new QsiColumnReferenceNode { Name = node.Target });
        var right = Resolve(context, node.Value);
        return new BinaryExpression(left, right, "=");
    }

    protected virtual InvokeExpression ResolveInvokeExpression(TableCompileContext context, IQsiInvokeExpressionNode node)
    {
        return new InvokeExpression(ResolveParametersExpression(context, node.Parameters));
    }

    protected virtual MultipleExpression ResolveParametersExpression(TableCompileContext context, IQsiParametersExpressionNode node)
    {
        return new MultipleExpression(node.Expressions.Select(e => Resolve(context, e)));
    }

    protected virtual MultipleExpression ResolveMultipleExpression(TableCompileContext context, IQsiMultipleExpressionNode node)
    {
        return new MultipleExpression(node.Elements.Select(e => Resolve(context, e)));
    }

    protected virtual SwitchExpression ResolveSwitchExpression(TableCompileContext context, IQsiSwitchExpressionNode node)
    {
        return new SwitchExpression(Resolve(context, node.Value), node.Cases.Select(c => ResolveSwitchCaseExpression(context, c)));
    }

    protected virtual SwitchCaseExpression ResolveSwitchCaseExpression(TableCompileContext context, IQsiSwitchCaseExpressionNode node)
    {
        return new SwitchCaseExpression(Resolve(context, node.Condition), Resolve(context, node.Consequent));
    }

    protected virtual MultipleExpression ResolveMemberAccessExpression(TableCompileContext context, IQsiMemberAccessExpressionNode node)
    {
        return new MultipleExpression(Resolve(context, node.Target), Resolve(context, node.Member));
    }

    protected virtual DerivedExpression ResolveOrderExpression(TableCompileContext context, IQsiOrderExpressionNode node)
    {
        return new DerivedExpression(Resolve(context, node.Expression));
    }

    protected virtual QsiExpression ResolveVariableExpression(TableCompileContext context, IQsiVariableExpressionNode node)
    {
        return new QsiExpression();
    }

    protected virtual QsiExpression ResolveFunctionExpression(TableCompileContext context, IQsiFunctionExpressionNode node)
    {
        return new QsiExpression();
    }

    protected virtual QsiExpression ResolveMemberExpression(TableCompileContext context, IQsiMemberExpressionNode node)
    {
        return new QsiExpression();
    }

    protected virtual QsiExpression ResolveBindParameterExpression(TableCompileContext context, IQsiBindParameterExpressionNode node)
    {
        return new QsiExpression();
    }
}

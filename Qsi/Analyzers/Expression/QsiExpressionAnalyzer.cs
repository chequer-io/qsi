using System;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Analyzers.Expression;

public class QsiExpressionAnalyzer
{
    protected virtual IQsiExpression ResolveSetColumnExpression(TableCompileContext context, IQsiSetColumnExpressionNode node)
    {
        return new QsiBinaryExpression(
            QsiExpressionType.SetColumn,
            ResolveColumnExpression(context, node.Target),
            ResolveExpression(context, node.Value)
        );
    }

    protected virtual IQsiExpression ResolveSetVariableExpression(TableCompileContext context, IQsiSetVariableExpressionNode node)
    {
        return new QsiBinaryExpression(
            QsiExpressionType.SetVariable,
            ResolveColumnExpression(context, node.Target),
            ResolveExpression(context, node.Value)
        );
    }

    protected virtual IQsiExpression ResolveInvokeExpression(TableCompileContext context, IQsiInvokeExpressionNode node)
    {
        return new QsiAtomicExpression(QsiExpressionType.Invoke);
    }

    protected virtual IQsiExpression ResolveLiteralExpression(TableCompileContext context, IQsiLiteralExpressionNode node)
    {
        return new QsiLiteralExpression
        {
            Value = node.Value,
            DataType = node.Type
        };
    }

    protected virtual IQsiExpression ResolveBinaryExpression(TableCompileContext context, IQsiBinaryExpressionNode node)
    {
        return new QsiBinaryExpression(
            QsiExpressionType.Binary,
            ResolveExpression(context, node.Left),
            ResolveExpression(context, node.Right)
        );
    }

    protected virtual QsiColumnExpression ResolveColumnExpression(TableCompileContext context, QsiQualifiedIdentifier identifier)
    {
        return new QsiColumnExpression(identifier);
    }

    protected virtual IQsiExpression ResolveAllColumnExpression(TableCompileContext context, IQsiAllColumnNode node)
    {
        return new QsiAtomicExpression(QsiExpressionType.AllColumn);
    }

    protected virtual IQsiExpression ResolveColumnReferenceExpression(TableCompileContext context, IQsiColumnReferenceNode node)
    {
        return ResolveColumnExpression(context, node.Name);
    }

    public virtual IQsiExpression ResolveExpression(TableCompileContext context, IQsiExpressionNode expression)
    {
        context.ThrowIfCancellationRequested();

        if (expression == null)
            return null;

        switch (expression)
        {
            case QsiExpressionFragmentNode:
                return new QsiAtomicExpression(QsiExpressionType.ExpressionFragment);

            case IQsiSetColumnExpressionNode e:
                return ResolveSetColumnExpression(context, e);

            case IQsiSetVariableExpressionNode e:
                return ResolveSetVariableExpression(context, e);

            case IQsiInvokeExpressionNode e:
                return ResolveInvokeExpression(context, e);

            case IQsiLiteralExpressionNode e:
                return ResolveLiteralExpression(context, e);

            case IQsiBinaryExpressionNode e:
                return ResolveBinaryExpression(context, e);

            // case IQsiParametersExpressionNode e:
            // {
            //     foreach (var c in e.Expressions.SelectMany(x => ResolveColumnsInExpression(context, x)))
            //         yield return c;
            //
            //     break;
            // }
            //
            // case IQsiMultipleExpressionNode e:
            // {
            //     foreach (var c in e.Elements.SelectMany(x => ResolveColumnsInExpression(context, x)))
            //         yield return c;
            //
            //     break;
            // }
            //
            // case IQsiSwitchExpressionNode e:
            // {
            //     foreach (var c in ResolveColumnsInExpression(context, e.Value))
            //         yield return c;
            //
            //     foreach (var c in e.Cases.SelectMany(c => ResolveColumnsInExpression(context, c)))
            //         yield return c;
            //
            //     break;
            // }
            //
            // case IQsiSwitchCaseExpressionNode e:
            // {
            //     foreach (var c in ResolveColumnsInExpression(context, e.Condition))
            //         yield return c;
            //
            //     foreach (var c in ResolveColumnsInExpression(context, e.Consequent))
            //         yield return c;
            //
            //     break;
            // }
            //
            // case IQsiTableExpressionNode e:
            // {
            //     using var scopedContext = new TableCompileContext(context);
            //     var structure = BuildTableStructure(scopedContext, e.Table).Result;
            //
            //     foreach (var c in structure.Columns)
            //         yield return c;
            //
            //     break;
            // }
            //
            // case IQsiUnaryExpressionNode e:
            // {
            //     foreach (var c in ResolveColumnsInExpression(context, e.Expression))
            //         yield return c;
            //
            //     break;
            // }
            //
            case IQsiColumnExpressionNode e:
            {
                switch (e.Column)
                {
                    case IQsiAllColumnNode allColumnNode:
                        return ResolveAllColumnExpression(context, allColumnNode);

                    case IQsiColumnReferenceNode columnReferenceNode:
                        return ResolveColumnReferenceExpression(context, columnReferenceNode);

                    default:
                        throw new InvalidOperationException();
                }
            }
            //
            // case IQsiMemberAccessExpressionNode e:
            // {
            //     foreach (var c in ResolveColumnsInExpression(context, e.Target))
            //         yield return c;
            //
            //     foreach (var c in ResolveColumnsInExpression(context, e.Member))
            //         yield return c;
            //
            //     break;
            // }
            //
            // case IQsiOrderExpressionNode e:
            // {
            //     foreach (var c in ResolveColumnsInExpression(context, e.Expression))
            //         yield return c;
            //
            //     break;
            // }
            //
            // case IQsiVariableExpressionNode e:
            // {
            //     // TODO: Analyze variable
            //     break;
            // }
            //
            // case IQsiFunctionExpressionNode e:
            // {
            //     // TODO: Analyze function
            //     break;
            // }
            //
            // case IQsiMemberExpressionNode _:
            // {
            //     // Skip unknown member access
            //     break;
            // }
            //
            // case IQsiBindParameterExpressionNode:
            //     break;

            default:
                throw new InvalidOperationException();
        }
    }
}

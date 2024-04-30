using System;
using System.Collections.Generic;
using System.Linq;
using Qsi.Analyzers.Table.Context;
using Qsi.Tree;

namespace Qsi.Analyzers.Expression;

public abstract class ExpressionResolver<T>
{
    public IEnumerable<T> GetValues(TableCompileContext context, IQsiExpressionNode expression)
    {
        context.ThrowIfCancellationRequested();

        if (expression is null)
            yield break;

        switch (expression)
        {
            case IQsiTableExpressionNode table:
                foreach (var value in ResolveTableExpression(context, table))
                    yield return value;

                break;

            case IQsiColumnExpressionNode column:
                foreach (var value in ResolveColumnExpression(context, column))
                    yield return value;

                break;

            default:
                foreach (var value in ResolveCommonExpression(context, expression))
                    yield return value;

                break;
        }
    }

    protected abstract IEnumerable<T> ResolveTableExpression(TableCompileContext context, IQsiTableExpressionNode expression);

    protected abstract IEnumerable<T> ResolveColumnExpression(TableCompileContext context, IQsiColumnExpressionNode expression);

    private IEnumerable<T> ResolveCommonExpression(TableCompileContext context, IQsiExpressionNode expression)
    {
        switch (expression)
        {
            case QsiExpressionFragmentNode:
                break;

            case IQsiSetColumnExpressionNode e:
            {
                foreach (var c in GetValues(context, e.Value))
                    yield return c;

                break;
            }

            case IQsiSetVariableExpressionNode e:
            {
                foreach (var c in GetValues(context, e.Value))
                    yield return c;

                break;
            }

            case IQsiInvokeExpressionNode e:
            {
                foreach (var c in GetValues(context, e.Member))
                    yield return c;

                foreach (var c in GetValues(context, e.Parameters))
                    yield return c;

                break;
            }

            case IQsiLiteralExpressionNode e:
            {
                break;
            }

            case IQsiBinaryExpressionNode e:
            {
                foreach (var c in GetValues(context, e.Left))
                    yield return c;

                foreach (var c in GetValues(context, e.Right))
                    yield return c;

                break;
            }

            case IQsiParametersExpressionNode e:
            {
                foreach (var c in e.Expressions.SelectMany(x => GetValues(context, x)))
                    yield return c;

                break;
            }

            case IQsiMultipleExpressionNode e:
            {
                foreach (var c in e.Elements.SelectMany(x => GetValues(context, x)))
                    yield return c;

                break;
            }

            case IQsiSwitchExpressionNode e:
            {
                foreach (var c in GetValues(context, e.Value))
                    yield return c;

                foreach (var c in e.Cases.SelectMany(c => GetValues(context, c)))
                    yield return c;

                break;
            }

            case IQsiSwitchCaseExpressionNode e:
            {
                foreach (var c in GetValues(context, e.Condition))
                    yield return c;

                foreach (var c in GetValues(context, e.Consequent))
                    yield return c;

                break;
            }

            case IQsiUnaryExpressionNode e:
            {
                foreach (var c in GetValues(context, e.Expression))
                    yield return c;

                break;
            }

            case IQsiMemberAccessExpressionNode e:
            {
                foreach (var c in GetValues(context, e.Target))
                    yield return c;

                foreach (var c in GetValues(context, e.Member))
                    yield return c;

                break;
            }

            case IQsiOrderExpressionNode e:
            {
                foreach (var c in GetValues(context, e.Expression))
                    yield return c;

                break;
            }

            case IQsiVariableExpressionNode e:
            {
                // TODO: Analyze variable
                break;
            }

            case IQsiFunctionExpressionNode e:
            {
                // TODO: Analyze function
                break;
            }

            case IQsiMemberExpressionNode _:
            {
                // Skip unknown member access
                break;
            }

            case IQsiBindParameterExpressionNode:
                break;

            default:
                throw new InvalidOperationException();
        }
    }
}

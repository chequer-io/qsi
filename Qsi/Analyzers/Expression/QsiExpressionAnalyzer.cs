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
    public virtual QsiExpression Resolve(TableCompileContext context, IQsiExpressionNode node)
    {
        switch (node)
        {
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

            //         case IQsiSetVariableExpressionNode e:
            //             return ResolveSetVariableExpression(context, e);
            //
            //         case IQsiInvokeExpressionNode e:
            //             return ResolveInvokeExpression(context, e);

            // case IQsiParametersExpressionNode e:
            //     return ResolveParametersExpression(context, e);
            //
            // case IQsiMultipleExpressionNode e:
            //     return ResolveMultipleExpression(context, e);
            //
            // case IQsiSwitchExpressionNode e:
            //     return ResolveSwitchExpression(context, e);
            //
            // case IQsiSwitchCaseExpressionNode e:
            //     return ResolveSwitchCaseExpression(context, e);

            //
            //         case IQsiColumnExpressionNode e:
            //             return ResolveColumnExpression(context, e);
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
                throw new NotSupportedException($"Not supported to resolve expression: '{node.GetType().Name}'");
        }
    }

    protected virtual QsiExpression ResolveSetColumnExpression(TableCompileContext context, IQsiSetColumnExpressionNode node)
    {
        var left = ResolveColumnReference(context, CreateColumnReferenceNode(node.Target));
        var right = Resolve(context, node.Value);

        return new BinaryExpression(left, right, "=");
    }

    #region Column Expression
    protected virtual QsiExpression ResolveColumnExpression(TableCompileContext context, IQsiColumnExpressionNode node)
    {
        return ResolveColumn(context, node.Column);
    }

    protected virtual QsiExpression ResolveColumn(TableCompileContext context, IQsiColumnNode node)
    {
        switch (node)
        {
            case IQsiColumnReferenceNode columnReference:
                return ResolveColumnReference(context, columnReference);

            case IQsiDerivedColumnNode derivedColumn:
                return new DerivedExpression(derivedColumn.IsColumn
                    ? ResolveColumn(context, derivedColumn.Column)
                    : Resolve(context, derivedColumn.Expression));

            default:
                throw new NotSupportedException($"Not supported to resolve column expression: '{node.GetType().Name}'");
        }
    }

    protected virtual QsiExpression ResolveColumnReference(TableCompileContext context, IQsiColumnReferenceNode node)
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

    // ==============================================

    // protected virtual IQsiExpression ResolveSetColumnExpression(TableCompileContext context, IQsiSetColumnExpressionNode node)
    // {
    //     return new QsiBinaryExpression(
    //         QsiExpressionType.SetColumn,
    //         ResolveColumnReferenceExpression(context, new QsiColumnReferenceNode { Name = node.Target }),
    //         ResolveExpression(context, node.Value)
    //     );
    // }
    //
    // protected virtual IQsiExpression ResolveSetVariableExpression(TableCompileContext context, IQsiSetVariableExpressionNode node)
    // {
    //     return new QsiBinaryExpression(
    //         QsiExpressionType.SetVariable,
    //         ResolveColumnReferenceExpression(context, new QsiColumnReferenceNode { Name = node.Target }),
    //         ResolveExpression(context, node.Value)
    //     );
    // }
    //
    // protected virtual IQsiExpression ResolveInvokeExpression(TableCompileContext context, IQsiInvokeExpressionNode node)
    // {
    //     return new QsiAtomicExpression(QsiExpressionType.Invoke);
    // }
    //

    // protected virtual IQsiExpression ResolveBinaryExpression(TableCompileContext context, IQsiBinaryExpressionNode node)
    // {
    //     return new QsiBinaryExpression(
    //         QsiExpressionType.Binary,
    //         ResolveExpression(context, node.Left),
    //         ResolveExpression(context, node.Right)
    //     );
    // }
    //
    // protected virtual IQsiExpression ResolveColumnExpression(TableCompileContext context, IQsiColumnExpressionNode node)
    // {
    //     if (node.Column is { } column)
    //     {
    //         if (column is IQsiColumnReferenceNode referenceNode)
    //             return ResolveColumnReferenceExpression(context, referenceNode);
    //     }
    //
    //     throw new InvalidOperationException();
    // }
    //
    // protected virtual IQsiExpression ResolveAllColumnExpression(TableCompileContext context, IQsiAllColumnNode node)
    // {
    //     return new QsiAtomicExpression(QsiExpressionType.AllColumn);
    // }
    //
    // protected virtual IQsiExpression ResolveColumnReferenceExpression(TableCompileContext context, IQsiColumnReferenceNode node)
    // {
    //     var analyzer = context.Engine.GetAnalyzer<QsiTableAnalyzer>();
    //     QsiTableColumn[] columns = analyzer.ResolveColumnReference(context, node);
    //
    //     switch (columns.Length)
    //     {
    //         case 0:
    //             throw new QsiException(QsiError.UnableResolveColumn, node.Name.ToString());
    //
    //         case 1:
    //             return new QsiColumnExpression(node.Name, columns[0]);
    //
    //         default:
    //             // TODO: All Columns compile
    //             throw new NotSupportedException();
    //     }
    // }
    //
    // public virtual IQsiExpression ResolveExpression(TableCompileContext context, IQsiExpressionNode expression)
    // {
    //     context.ThrowIfCancellationRequested();
    //
    //     if (expression == null)
    //         return null;
    //
    //     switch (expression)
    //     {
    //         case QsiExpressionFragmentNode:
    //             return new QsiAtomicExpression(QsiExpressionType.ExpressionFragment);
    //
    //         case IQsiSetColumnExpressionNode e:
    //             return ResolveSetColumnExpression(context, e);
    //
    //         case IQsiSetVariableExpressionNode e:
    //             return ResolveSetVariableExpression(context, e);
    //
    //         case IQsiInvokeExpressionNode e:
    //             return ResolveInvokeExpression(context, e);
    //
    //         case IQsiLiteralExpressionNode e:
    //             return ResolveLiteralExpression(context, e);
    //
    //         case IQsiBinaryExpressionNode e:
    //             return ResolveBinaryExpression(context, e);
    //         //
    //         // case IQsiParametersExpressionNode e:
    //         //     return ResolveParametersExpression(context, e);
    //         //
    //         // case IQsiMultipleExpressionNode e:
    //         //     return ResolveMultipleExpression(context, e);
    //         //
    //         // case IQsiSwitchExpressionNode e:
    //         //     return ResolveSwitchExpression(context, e);
    //         //
    //         // case IQsiSwitchCaseExpressionNode e:
    //         //     return ResolveSwitchCaseExpression(context, e);
    //
    //         case IQsiTableExpressionNode e:
    //             return ResolveTableExpression(context, e);
    //
    //         case IQsiUnaryExpressionNode e:
    //             return ResolveUnaryExpression(context, e);
    //
    //         case IQsiColumnExpressionNode e:
    //             return ResolveColumnExpression(context, e);
    //
    //         // case IQsiMemberAccessExpressionNode e:
    //         // {
    //         //     foreach (var c in ResolveColumnsInExpression(context, e.Target))
    //         //         yield return c;
    //         //
    //         //     foreach (var c in ResolveColumnsInExpression(context, e.Member))
    //         //         yield return c;
    //         //
    //         //     break;
    //         // }
    //         //
    //         // case IQsiOrderExpressionNode e:
    //         // {
    //         //     foreach (var c in ResolveColumnsInExpression(context, e.Expression))
    //         //         yield return c;
    //         //
    //         //     break;
    //         // }
    //         //
    //         // case IQsiVariableExpressionNode e:
    //         // {
    //         //     // TODO: Analyze variable
    //         //     break;
    //         // }
    //         //
    //         // case IQsiFunctionExpressionNode e:
    //         // {
    //         //     // TODO: Analyze function
    //         //     break;
    //         // }
    //         //
    //         // case IQsiMemberExpressionNode _:
    //         // {
    //         //     // Skip unknown member access
    //         //     break;
    //         // }
    //         //
    //         // case IQsiBindParameterExpressionNode:
    //         //     break;
    //
    //         default:
    //             throw new InvalidOperationException();
    //     }
    // }
}

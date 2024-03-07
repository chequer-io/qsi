using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Analyzers.Action.Models;

public sealed class SetColumnTarget : ColumnTarget
{
    public IQsiExpressionNode ValueNode { get; }

    public SetColumnTarget(
        int declaredOrder,
        QsiQualifiedIdentifier declaredName,
        QsiTableColumn sourceColumn,
        QsiTableColumn affectedColumn,
        IQsiExpressionNode valueNode)
        : base(declaredOrder, declaredName, sourceColumn, affectedColumn)
    {
        ValueNode = valueNode;
    }
}
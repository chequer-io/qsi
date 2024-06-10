using System.Linq;
using System.Collections.Generic;
using Qsi.Data;
using Qsi.Tree.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable;

public readonly struct ImmutableDataInsertActionNode : IQsiDataInsertActionNode
{
    public IQsiTreeNode Parent { get; }

    public IQsiTableDirectivesNode Directives { get; }

    public IQsiTableReferenceNode Target { get; }

    public QsiQualifiedIdentifier[] Partitions { get; }

    public QsiQualifiedIdentifier[] Columns { get; }

    public IQsiRowValueExpressionNode[] Values { get; }

    public IQsiSetColumnExpressionNode[] SetValues { get; }

    public QsiIdentifier FileValue { get; }

    public IQsiTableNode ValueTable { get; }

    public QsiDataConflictBehavior ConflictBehavior { get; }

    public IQsiDataConflictActionNode ConflictAction { get; }

    public IUserDataHolder UserData { get; }

    public IEnumerable<IQsiTreeNode> Children =>
        TreeHelper.YieldChildren(Directives, Target)
            .Concat(Values)
            .Concat(SetValues)
            .Concat(TreeHelper.YieldChildren(ValueTable, ConflictAction));

    public ImmutableDataInsertActionNode(
        IQsiTreeNode parent,
        IQsiTableDirectivesNode directives,
        IQsiTableReferenceNode target,
        QsiQualifiedIdentifier[] partitions,
        QsiQualifiedIdentifier[] columns,
        IQsiRowValueExpressionNode[] values,
        IQsiSetColumnExpressionNode[] setValues,
        QsiIdentifier fileValue,
        IQsiTableNode valueTable,
        QsiDataConflictBehavior conflictBehavior,
        IQsiDataConflictActionNode conflictAction,
        IUserDataHolder userData)
    {
        Parent = parent;
        Directives = directives;
        Target = target;
        Partitions = partitions;
        Columns = columns;
        Values = values;
        SetValues = setValues;
        FileValue = fileValue;
        ValueTable = valueTable;
        ConflictBehavior = conflictBehavior;
        ConflictAction = conflictAction;
        UserData = userData;
    }
}
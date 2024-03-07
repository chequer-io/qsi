using Qsi.Data;

namespace Qsi.Tree;

public interface IQsiDataInsertActionNode : IQsiActionNode
{
    IQsiTableDirectivesNode Directives { get; }

    IQsiTableReferenceNode Target { get; }

    QsiQualifiedIdentifier[] Partitions { get; }

    QsiQualifiedIdentifier[] Columns { get; }

    IQsiRowValueExpressionNode[] Values { get; }

    IQsiSetColumnExpressionNode[] SetValues { get; }

    IQsiTableNode ValueTable { get; }

    QsiDataConflictBehavior ConflictBehavior { get; }

    IQsiDataConflictActionNode ConflictAction { get; }
}
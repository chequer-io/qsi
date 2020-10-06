using Qsi.Data;

namespace Qsi.Tree
{
    public interface IQsiDataInsertActionNode : IQsiActionNode
    {
        IQsiTableDirectivesNode Directives { get; }

        IQsiTableAccessNode Target { get; }

        QsiQualifiedIdentifier[] Partitions { get; }

        IQsiColumnsDeclarationNode Columns { get; }

        IQsiRowValueExpressionNode[] Rows { get; }

        IQsiAssignExpressionNode[] Elements { get; }

        QsiDataConflictAction ConflictAction { get; }

        IQsiDataConflictActionNode[] ConflictActions { get; }
    }
}

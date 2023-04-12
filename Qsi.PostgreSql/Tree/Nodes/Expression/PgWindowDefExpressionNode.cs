using System.Collections.Generic;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgWindowDefExpressionNode : QsiExpressionNode
{
    public QsiIdentifier? Name { get; set; }

    public QsiIdentifier? Refname { get; set; }

    public QsiTreeNodeList<QsiExpressionNode?> PartitionClause { get; }

    public QsiTreeNodeList<QsiExpressionNode?> OrderClause { get; }

    public FrameOptions FrameOptions { get; set; }

    public QsiTreeNodeProperty<QsiExpressionNode> StartOffset { get; }

    public QsiTreeNodeProperty<QsiExpressionNode> EndOffset { get; }

    public PgWindowDefExpressionNode()
    {
        PartitionClause = new QsiTreeNodeList<QsiExpressionNode?>(this);
        OrderClause = new QsiTreeNodeList<QsiExpressionNode?>(this);
        StartOffset = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        EndOffset = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }

    public override IEnumerable<IQsiTreeNode> Children
    {
        get
        {
            foreach (var item in PartitionClause)
            {
                if (item is { })
                    yield return item;
            }

            foreach (var item in OrderClause)
            {
                if (item is { })
                    yield return item;
            }

            if (!StartOffset.IsEmpty)
                yield return StartOffset.Value;

            if (!EndOffset.IsEmpty)
                yield return EndOffset.Value;
        }
    }
}

public enum FrameOptions
{
    Unknown = 0x00000,
    NonDefault = 0x00001,
    Range = 0x00002,
    Rows = 0x00004,
    Groups = 0x00008,
    Between = 0x00010,
    StartUnboundedPreceding = 0x00020,
    EndUnboundedPreceding = 0x00040,
    StartUnboundedFollowing = 0x00080,
    EndUnboundedFollowing = 0x00100,
    StartCurrentRow = 0x00200,
    EndCurrentRow = 0x00400,
    StartOffsetPreceding = 0x00800,
    EndOffsetPreceding = 0x01000,
    StartOffsetFollowing = 0x02000,
    EndOffsetFollowing = 0x04000,
    ExcludeCurrentRow = 0x08000,
    ExcludeGroup = 0x10000,
    ExcludeTies = 0x20000,
}

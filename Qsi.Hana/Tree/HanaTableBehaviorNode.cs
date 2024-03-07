using System;
using System.Collections.Generic;
using System.Linq;
using Qsi.Tree;

namespace Qsi.Hana.Tree;

public abstract class HanaTableBehaviorNode : QsiTreeNode
{
}

public sealed class HanaTableShareLockBehaviorNode : HanaTableBehaviorNode
{
    public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();
}

public sealed class HanaTableUpdateBehaviorNode : HanaTableBehaviorNode
{
    public QsiTreeNodeProperty<QsiColumnsDeclarationNode> Columns { get; }

    // not null: WAIT <UNSIGNED_INTEGER | ?>
    //     null: NOWAIT
    public QsiTreeNodeProperty<QsiExpressionNode> WaitTime { get; }

    public bool IgnoreLocked { get; set; }

    public override IEnumerable<IQsiTreeNode> Children
    {
        get
        {
            if (!Columns.IsEmpty)
                yield return Columns.Value;
        }
    }

    public HanaTableUpdateBehaviorNode()
    {
        Columns = new QsiTreeNodeProperty<QsiColumnsDeclarationNode>(this);
        WaitTime = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }
}

public sealed class HanaTableSerializeBehaviorNode : HanaTableBehaviorNode
{
    public HanaTableSerializeType Type { get; set; }

    public Dictionary<string, string> Options { get; } = new();

    public string ReturnType { get; set; }

    public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();
}

public sealed class HanaTableSystemTimeBehaviorNode : HanaTableBehaviorNode
{
    public string Time { get; set; }

    public ValueTuple<string, string>? FromTo { get; set; }

    public ValueTuple<string, string>? Between { get; set; }

    public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();
}

public sealed class HanaTableApplicationTimeBehaviorNode : HanaTableBehaviorNode
{
    public string Time { get; set; }

    public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();
}

public enum HanaTableSerializeType
{
    Json,
    Xml
}
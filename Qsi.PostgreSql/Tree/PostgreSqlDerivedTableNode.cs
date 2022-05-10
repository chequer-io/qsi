using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.PostgreSql.Tree;

public sealed class PostgreSqlDerivedTableNode : QsiDerivedTableNode
{
    public QsiTreeNodeProperty<PostgreSqlSelectOptionNode> SelectOptions { get; }

    public QsiTreeNodeProperty<PostgreSqlLockingNode> Locking { get; }

    public override IEnumerable<IQsiTreeNode> Children
    {
        get
        {
            foreach (var child in base.Children)
            {
                yield return child;
            }

            if (!SelectOptions.IsEmpty)
            {
                yield return SelectOptions.Value;   
            }
            
            if (!Locking.IsEmpty)
            {
                yield return Locking.Value;
            }
        }
    }

    public PostgreSqlDerivedTableNode()
    {
        SelectOptions = new QsiTreeNodeProperty<PostgreSqlSelectOptionNode>(this);
        Locking = new QsiTreeNodeProperty<PostgreSqlLockingNode>(this);
    }
}

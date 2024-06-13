using System.Collections.Generic;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.SqlServer.Tree;

public class SqlServerVariableSetActionNode : QsiVariableSetActionNode
{
    public QsiTreeNodeProperty<QsiTableNode> Source { get; }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(base.Children, Source);

    public SqlServerVariableSetActionNode()
    {
        Source = new QsiTreeNodeProperty<QsiTableNode>(this);
    }
}

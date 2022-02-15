using System.Collections.Generic;
using System.Linq;
using Qsi.Tree;

namespace Qsi.SqlServer.Tree;

public class SqlServerPhyslocExpressionNode : QsiExpressionNode
{
    public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();
}

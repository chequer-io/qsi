using System.Collections.Generic;
using System.Linq;
using Qsi.Tree;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgDefaultExpressionNode : QsiExpressionNode
{
    public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();
}

using System.Collections.Generic;
using System.Linq;
using Qsi.Tree;

namespace Qsi.Hana.Tree;

public sealed class HanaCollateExpressionNode : QsiExpressionNode
{
    public string Name { get; set; }

    public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();
}
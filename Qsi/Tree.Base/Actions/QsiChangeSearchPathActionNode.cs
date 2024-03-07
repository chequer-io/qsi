using System.Collections.Generic;
using System.Linq;
using Qsi.Data;

namespace Qsi.Tree;

public class QsiChangeSearchPathActionNode : QsiActionNode, IQsiChangeSearchPathActionNode
{
    public QsiQualifiedIdentifier[] Identifiers { get; set; }

    public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();
}
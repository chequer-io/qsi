using System.Collections.Generic;
using System.Linq;
using Qsi.Data;

namespace Qsi.Tree;

public class QsiDropPrepareActionNode : QsiActionNode, IQsiDropPrepareActionNode
{
    public QsiQualifiedIdentifier Identifier { get; set; }

    public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();
}
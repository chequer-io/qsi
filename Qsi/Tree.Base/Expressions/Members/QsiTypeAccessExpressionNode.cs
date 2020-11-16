using System.Collections.Generic;
using System.Linq;
using Qsi.Data;

namespace Qsi.Tree
{
    public class QsiTypeAccessExpressionNode : QsiExpressionNode, IQsiMemberAccessExpressionNode, IQsiTerminalNode
    {
        public QsiQualifiedIdentifier Identifier { get; set; }

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();
    }
}

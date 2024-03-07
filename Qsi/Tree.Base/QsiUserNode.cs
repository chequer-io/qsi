using System.Collections.Generic;
using System.Linq;
using Qsi.Utilities;

namespace Qsi.Tree;

public class QsiUserNode : QsiTreeNode, IQsiUserNode
{
    public string Username { get; set; }

    public QsiTreeNodeProperty<QsiExpressionNode> Password { get; }

    #region Explicit
    IQsiExpressionNode IQsiUserNode.Password => Password.Value;
    #endregion

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Password);

    public QsiUserNode()
    {
        Password = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }
}

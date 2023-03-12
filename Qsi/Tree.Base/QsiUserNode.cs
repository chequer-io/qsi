using System.Collections.Generic;
using System.Linq;
using Qsi.Data;

namespace Qsi.Tree;

public class QsiUserNode : QsiTreeNode, IQsiUserNode
{
    public QsiQualifiedIdentifier UserName { get; set; }

    public QsiTreeNodeProperty<QsiExpressionNode> Password { get; }

    public QsiTreeNodeList<QsiSetValueExpressionNode> Properties { get; }

    #region Explicit
    IQsiExpressionNode IQsiUserNode.Password => Password.Value;

    IQsiSetValueExpressionNode[] IQsiUserNode.Properties => Properties.Cast<IQsiSetValueExpressionNode>().ToArray();
    #endregion

    public override IEnumerable<IQsiTreeNode> Children => Properties;

    public QsiUserNode()
    {
        Password = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        Properties = new QsiTreeNodeList<QsiSetValueExpressionNode>(this);
    }
}

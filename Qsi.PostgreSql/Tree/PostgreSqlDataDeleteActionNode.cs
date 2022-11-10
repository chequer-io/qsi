using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.PostgreSql.Tree;

public class PostgreSqlDataDeleteActionNode : QsiDataDeleteActionNode, IQsiTableNode
{
    public QsiTreeNodeProperty<QsiColumnsDeclarationNode> Returning { get; }

    public override IEnumerable<IQsiTreeNode> Children
    {
        get
        {
            foreach (var node in base.Children)
            {
                yield return node;
            }

            if (!Returning.IsEmpty)
            {
                yield return Returning.Value;    
            }
        }
    }

    public PostgreSqlDataDeleteActionNode()
    {
        Returning = new QsiTreeNodeProperty<QsiColumnsDeclarationNode>(this);
    }
}

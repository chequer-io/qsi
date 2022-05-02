using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.PostgreSql.Data;

public class PostgreSqlSubscriptExpressionNode : QsiExpressionNode
{
    public QsiTreeNodeProperty<QsiExpressionNode> Index { get; }
    
    public QsiTreeNodeProperty<QsiExpressionNode> Start { get; }
    public QsiTreeNodeProperty<QsiExpressionNode> End { get; }

    public PostgreSqlSubscriptExpressionNode()
    {
        Index = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        
        Start = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        End = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }

    public override IEnumerable<IQsiTreeNode> Children
    {
        get
        {
            yield return Index.Value;
            
            yield return Start.Value;
            yield return End.Value;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Qsi.PostgreSql.Data;
using Qsi.Tree;

namespace Qsi.PostgreSql.Tree;

public sealed class PostgreSqlSelectOptionNode : QsiTreeNode
{
    public PostgreSqlSelectOption Option { get; set; }
    
    public QsiTreeNodeList<QsiExpressionNode> DistinctExpressionList { get; }

    public override IEnumerable<IQsiTreeNode> Children 
        => DistinctExpressionList;

    public PostgreSqlSelectOptionNode()
    {
        DistinctExpressionList = new QsiTreeNodeList<QsiExpressionNode>(this);
    }
}

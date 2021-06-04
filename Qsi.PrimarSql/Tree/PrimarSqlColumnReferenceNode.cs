using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.PrimarSql.Tree
{
    public class PrimarSqlColumnReferenceNode : QsiColumnReferenceNode
    {
        public QsiTreeNodeList<QsiExpressionNode> Accessors { get; }

        public override IEnumerable<IQsiTreeNode> Children => Accessors;

        public PrimarSqlColumnReferenceNode()
        {
            Accessors = new QsiTreeNodeList<QsiExpressionNode>(this);
        }
    }
}

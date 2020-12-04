using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.PrimarSql.Tree
{
    public class PrimarSqlDeclaredColumnNode : QsiDeclaredColumnNode
    {
        public QsiTreeNodeList<QsiExpressionNode> Accessors { get; }

        public override IEnumerable<IQsiTreeNode> Children => Accessors;

        public PrimarSqlDeclaredColumnNode()
        {
            Accessors = new QsiTreeNodeList<QsiExpressionNode>(this);
        }
    }
}

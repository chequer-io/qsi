using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Oracle.Tree
{
    public class OracleNamedParameterExpressionNode : QsiExpressionNode
    {
        public QsiQualifiedIdentifier Identifier { get; set; }
        
        public QsiTreeNodeProperty<QsiExpressionNode> Expression { get; set; }

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();

        public OracleNamedParameterExpressionNode()
        {
            Expression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}

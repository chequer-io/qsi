using Qsi.Tree;

namespace Qsi.Cql.Tree
{
    public sealed class CqlDataInsertActionNode : QsiDataInsertActionNode
    {
        public string DefaultValue { get; set; }

        public QsiTreeNodeProperty<CqlMultipleUsingExpressionNode> Usings { get; }

        public CqlDataInsertActionNode()
        {
            Usings = new QsiTreeNodeProperty<CqlMultipleUsingExpressionNode>(this);
        }
    }
}

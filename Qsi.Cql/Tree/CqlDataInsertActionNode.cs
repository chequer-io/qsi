using Qsi.Tree;

namespace Qsi.Cql.Tree
{
    public sealed class CqlDataInsertActionNode : QsiDataInsertActionNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> Using { get; }

        public string Json { get; set; }

        public string DefaultValue { get; set; }
        
        public bool IfNotExists { get; set; }

        public CqlDataInsertActionNode()
        {
            Using = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}

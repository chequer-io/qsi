using Qsi.Tree;
using Qsi.Tree.Definition;

namespace Qsi.MySql.Tree
{
    public sealed class MySqlViewDefinitionNode : QsiViewDefinitionNode
    {
        public QsiTreeNodeProperty<QsiExpressionFragmentNode> ViewAlgorithm { get; }

        public QsiTreeNodeProperty<QsiExpressionFragmentNode> ViewSuid { get; }

        public string Definer { get; set; }

        public bool Replace { get; set; }

        public MySqlViewDefinitionNode()
        {
            ViewAlgorithm = new QsiTreeNodeProperty<QsiExpressionFragmentNode>(this);
            ViewSuid = new QsiTreeNodeProperty<QsiExpressionFragmentNode>(this);
        }
    }
}

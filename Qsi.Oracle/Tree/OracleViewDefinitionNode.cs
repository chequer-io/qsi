using Qsi.Tree;
using Qsi.Tree.Definition;

namespace Qsi.Oracle.Tree
{
    public class OracleViewDefinitionNode : QsiViewDefinitionNode
    {
        public string DefaultCollationName { get; set; }

        public bool Replace { get; set; }

        public bool Force { get; set; }

        public QsiTreeNodeProperty<QsiExpressionFragmentNode> EditionOption { get; }

        public QsiTreeNodeProperty<QsiExpressionFragmentNode> SharingOption { get; }

        public QsiTreeNodeProperty<QsiExpressionFragmentNode> Bequeath { get; }

        public QsiTreeNodeProperty<QsiExpressionFragmentNode> SubqueryRestriction { get; }

        public QsiTreeNodeProperty<QsiExpressionFragmentNode> ContainerOption { get; }

        public OracleViewDefinitionNode()
        {
            EditionOption = new QsiTreeNodeProperty<QsiExpressionFragmentNode>(this);
            SharingOption = new QsiTreeNodeProperty<QsiExpressionFragmentNode>(this);
            Bequeath = new QsiTreeNodeProperty<QsiExpressionFragmentNode>(this);
            SubqueryRestriction = new QsiTreeNodeProperty<QsiExpressionFragmentNode>(this);
            ContainerOption = new QsiTreeNodeProperty<QsiExpressionFragmentNode>(this);
        }
    }
}

using Qsi.Data;
using Qsi.Tree;
using Qsi.Tree.Definition;

namespace Qsi.Impala.Tree
{
    public class ImpalaTableDefinitionNode : QsiTableDefinitionNode
    {
        public string PlanHints { get; set; }

        public bool IsExternal { get; set; }

        public QsiIdentifier[] PrimaryKeyColumnNames { get; set; }

        public QsiIdentifier[] PartitionColumnNames { get; set; }

        public QsiTreeNodeProperty<QsiExpressionFragmentNode> KuduPartitionParams { get; }

        public QsiTreeNodeProperty<QsiExpressionFragmentNode> IcebergPartitionSpecs { get; }

        public QsiTreeNodeProperty<QsiExpressionFragmentNode> Options { get; }

        public ImpalaTableDefinitionNode()
        {
            KuduPartitionParams = new QsiTreeNodeProperty<QsiExpressionFragmentNode>(this);
            IcebergPartitionSpecs = new QsiTreeNodeProperty<QsiExpressionFragmentNode>(this);
            Options = new QsiTreeNodeProperty<QsiExpressionFragmentNode>(this);
        }
    }
}

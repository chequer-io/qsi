using System.Collections.Generic;
using System.Linq;
using Qsi.Tree;

namespace Qsi.SingleStore.Tree;

public sealed class SingleStoreDerivedTableNode : QsiDerivedTableNode
{
    public QsiTreeNodeList<SingleStoreSelectOptionNode> SelectOptions { get; }

    public QsiTreeNodeProperty<SingleStoreProcedureAnalyseNode> ProcedureAnalyse { get; }

    public QsiTreeNodeList<SingleStoreLockingNode> Lockings { get; }

    public override IEnumerable<IQsiTreeNode> Children
    {
        get
        {
            foreach (var child in SelectOptions.Concat(base.Children))
                yield return child;

            if (!ProcedureAnalyse.IsEmpty)
                yield return ProcedureAnalyse.Value;

            foreach (var locking in Lockings)
                yield return locking;
        }
    }

    public SingleStoreDerivedTableNode()
    {
        SelectOptions = new QsiTreeNodeList<SingleStoreSelectOptionNode>(this);
        ProcedureAnalyse = new QsiTreeNodeProperty<SingleStoreProcedureAnalyseNode>(this);
        Lockings = new QsiTreeNodeList<SingleStoreLockingNode>(this);
    }
}

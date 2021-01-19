using System.Collections.Generic;
using System.Linq;
using Qsi.Tree;

namespace Qsi.MySql.Tree
{
    public sealed class MySqlDerivedTableNode : QsiDerivedTableNode
    {
        public QsiTreeNodeList<MySqlSelectOptionNode> SelectOptions { get; }

        public QsiTreeNodeProperty<MySqlProcedureAnalyseNode> ProcedureAnalyse { get; }

        public QsiTreeNodeList<MySqlLockingNode> Lockings { get; }

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

        public MySqlDerivedTableNode()
        {
            SelectOptions = new QsiTreeNodeList<MySqlSelectOptionNode>(this);
            ProcedureAnalyse = new QsiTreeNodeProperty<MySqlProcedureAnalyseNode>(this);
            Lockings = new QsiTreeNodeList<MySqlLockingNode>(this);
        }
    }
}

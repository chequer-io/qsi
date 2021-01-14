using System.Collections.Generic;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.SqlServer.Tree
{
    public class SqlServerAlterUserActionNode : QsiActionNode
    {
        public QsiIdentifier TargetUser { get; set; }

        public QsiIdentifier DefaultSchema { get; set; }

        public QsiIdentifier NewUserName { get; set; } 

        public override IEnumerable<IQsiTreeNode> Children { get; }
    }
}

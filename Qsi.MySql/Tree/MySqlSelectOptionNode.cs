using System.Collections.Generic;
using System.Linq;
using Qsi.MySql.Data;
using Qsi.Tree;

namespace Qsi.MySql.Tree
{
    public sealed class MySqlSelectOptionNode : QsiTreeNode
    {
        public MySqlSelectOption Option { get; set; }

        public long? MaxStatementTime { get; set; }

        public override IEnumerable<IQsiTreeNode> Children { get; } = Enumerable.Empty<IQsiTreeNode>();
    }
}

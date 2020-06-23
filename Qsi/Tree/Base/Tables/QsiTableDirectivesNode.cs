using System.Collections.Generic;
using System.Linq;

namespace Qsi.Tree.Base
{
    public sealed class QsiTableDirectivesNode : QsiTableNode, IQsiTableDirectivesNode
    {
        public List<QsiTableNode> Tables { get; } = new List<QsiTableNode>();

        #region Explicit
        IQsiTableNode[] IQsiTableDirectivesNode.Tables => Tables.Cast<IQsiTableNode>().ToArray();
        #endregion
    }
}

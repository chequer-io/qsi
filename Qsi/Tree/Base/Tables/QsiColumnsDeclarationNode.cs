using System.Collections.Generic;
using System.Linq;

namespace Qsi.Tree.Base
{
    public sealed class QsiColumnsDeclarationNode : QsiTreeNode, IQsiColumnsDeclarationNode
    {
        public List<QsiColumnNode> Columns { get; } = new List<QsiColumnNode>();

        #region Explicit
        IQsiColumnNode[] IQsiColumnsDeclarationNode.Columns => Columns.Cast<IQsiColumnNode>().ToArray();
        #endregion
    }
}

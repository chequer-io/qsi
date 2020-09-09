using System.Collections.Generic;

namespace Qsi.Tree
{
    public interface IQsiTreeNode
    {
        IQsiTreeNode Parent { get; }

        IEnumerable<IQsiTreeNode> Children { get; }
    }
}
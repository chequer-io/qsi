using System.Collections.Generic;
using Qsi.Tree.Data;

namespace Qsi.Tree;

public interface IQsiTreeNode
{
    IQsiTreeNode Parent { get; }

    // TODO: Change to visitor pattern
    IEnumerable<IQsiTreeNode> Children { get; }

    IUserDataHolder UserData { get; }
}
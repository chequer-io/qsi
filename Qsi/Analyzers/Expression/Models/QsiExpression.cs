using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.Analyzers.Expression.Models;

public class QsiExpression
{
    public int StartIndex { get; private set; }

    public int EndIndex { get; private set; }

    internal IQsiTreeNode Node { get; private set; }

    public void SetIndex(int start, int end)
    {
        StartIndex = start;
        EndIndex = end;
    }

    internal void SetNode(IQsiTreeNode node)
    {
        Node = node;
    }

    public virtual IEnumerable<QsiExpression> GetChildren()
    {
        yield break;
    }
}

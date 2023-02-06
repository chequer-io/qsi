using System.Collections.Generic;

namespace Qsi.Analyzers.Expression.Models;

public class QsiExpression
{
    public int StartIndex { get; private set; }

    public int EndIndex { get; private set; }

    public void SetIndex(int start, int end)
    {
        StartIndex = start;
        EndIndex = end;
    }

    public virtual IEnumerable<QsiExpression> GetChildren()
    {
        yield break;
    }
}

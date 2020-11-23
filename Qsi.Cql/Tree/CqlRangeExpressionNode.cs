using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.Cql.Tree
{
    public sealed class CqlRangeExpressionNode : QsiExpressionNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> Start { get; }

        public QsiTreeNodeProperty<QsiExpressionNode> End { get; }

        public override IEnumerable<IQsiTreeNode> Children
        {
            get
            {
                if (!Start.IsEmpty)
                    yield return Start.Value;

                if (!End.IsEmpty)
                    yield return End.Value;
            }
        }
    }
}

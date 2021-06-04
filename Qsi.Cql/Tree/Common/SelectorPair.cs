using Qsi.Tree;

namespace Qsi.Cql.Tree.Common
{
    internal readonly ref struct SelectorPair
    {
        public QsiColumnReferenceNode Column { get; }

        public QsiExpressionNode Expression { get; }

        public SelectorPair(QsiColumnReferenceNode column)
        {
            Column = column;
            Expression = null;
        }

        public SelectorPair(QsiExpressionNode expression)
        {
            Column = null;
            Expression = expression;
        }
    }
}

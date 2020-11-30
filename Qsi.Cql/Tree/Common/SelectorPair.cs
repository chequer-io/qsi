using Qsi.Tree;

namespace Qsi.Cql.Tree.Common
{
    internal readonly ref struct SelectorPair
    {
        public QsiDeclaredColumnNode Column { get; }

        public QsiExpressionNode Expression { get; }

        public SelectorPair(QsiDeclaredColumnNode column)
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

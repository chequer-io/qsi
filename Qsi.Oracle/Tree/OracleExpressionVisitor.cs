using net.sf.jsqlparser.schema;
using Qsi.JSql.Tree;
using Qsi.Tree;

namespace Qsi.Oracle.Tree
{
    internal sealed class OracleExpressionVisitor : JSqlExpressionVisitor
    {
        public OracleExpressionVisitor(IJSqlVisitorSet set) : base(set)
        {
        }

        public override QsiExpressionNode VisitColumn(Column expression)
        {
            var expressionNode = base.VisitColumn(expression);

            if (expressionNode is QsiColumnExpressionNode columnExpression &&
                columnExpression.Column.Value is QsiDerivedColumnNode { Expression: { } } derivedColumn)
            {
                return derivedColumn.Expression.Value;
            }

            return expressionNode;
        }
    }
}

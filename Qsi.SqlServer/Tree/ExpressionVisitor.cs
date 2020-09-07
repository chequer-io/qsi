using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;
using Qsi.Tree.Base;
using Qsi.Utilities;

namespace Qsi.SqlServer.Tree
{
    internal static class ExpressionVisitor
    {
        public static QsiExpressionNode Visit(SqlSelectExpression expression)
        {
            switch (expression)
            {

            }

            throw TreeHelper.NotSupportedTree(expression);
        }
    }
}

using System.Linq;
using Qsi.MySql.Data;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Utilities;
using static Qsi.MySql.Internal.MySqlParserInternal;

namespace Qsi.MySql.Tree
{
    internal static class TableVisitor
    {
        public static QsiTableNode VisitSelectStatement(SelectStatementContext context)
        {
            switch (context.children[0])
            {
                case QueryExpressionContext queryExpression:
                    return VisitQueryExpression(queryExpression, context.lockingClauseList());

                case QueryExpressionParensContext queryExpressionParens:
                    return VisitQueryExpressionParens(queryExpressionParens);

                case SelectStatementWithIntoContext selectStatementWithInto:
                    return VisitSelectStatementWithInto(selectStatementWithInto);

                default:
                    throw TreeHelper.NotSupportedTree(context.children[0]);
            }
        }

        public static QsiTableNode VisitQueryExpression(QueryExpressionContext context, LockingClauseListContext lockingClauseList)
        {
            throw new System.NotImplementedException();
        }

        public static QsiTableNode VisitQueryExpressionParens(QueryExpressionParensContext context)
        {
            throw new System.NotImplementedException();
        }

        public static QsiTableNode VisitSelectStatementWithInto(SelectStatementWithIntoContext context)
        {
            throw new System.NotImplementedException();
        }

        public static MySqlLockingNode VisitLockingClause(LockingClauseContext context)
        {
            var node = new MySqlLockingNode();
            var lockStrengh = context.lockStrengh();

            if (lockStrengh != null)
            {
                node.TableLockType = lockStrengh.HasToken(UPDATE_SYMBOL) ? 
                    MySqlTableLockType.Update : MySqlTableLockType.Share;

                node.Tables = context.tableAliasRefList()?
                    .tableRefWithWildcard()
                    .Select(IdentifierVisitor.VisitTableRefWithWildcard)
                    .ToArray();

                var lockedRowAction = context.lockedRowAction();

                if (lockedRowAction != null)
                {
                    node.RowLockType = lockedRowAction.HasToken(SKIP_SYMBOL) ? 
                        MySqlRowLockType.SkipLocked : MySqlRowLockType.NoWait;
                }
            }
            else
            {
                node.TableLockType = MySqlTableLockType.ShareMode;
            }

            MySqlTree.PutContextSpan(node, context);

            return node;
        }
    }
}

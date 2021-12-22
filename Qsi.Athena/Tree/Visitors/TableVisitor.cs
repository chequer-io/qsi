using Qsi.Athena.Internal;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Athena.Tree.Visitors
{
    using static SqlBaseParser;

    internal static class TableVisitor
    {
        public static QsiTableNode VisitQuery(QueryContext context)
        {
            var with = context.with();
            var queryNoWith = context.queryNoWith();

            var queryNoWithNode = VisitQueryNoWith(queryNoWith);

            if (with is null)
            {
                return queryNoWithNode;
            }

            if (queryNoWithNode is not QsiDerivedTableNode derivedTable)
            {
                derivedTable = new QsiDerivedTableNode
                {
                    Source =
                    {
                        Value = queryNoWithNode
                    },
                    Columns =
                    {
                        Value = TreeHelper.CreateAllColumnsDeclaration()
                    }
                };

                queryNoWithNode = derivedTable;
            }
            else
            {
                AthenaTree.PutContextSpan(queryNoWithNode, context);
            }

            derivedTable.Directives.Value = VisitWith(with);

            return queryNoWithNode;
        }

        private static QsiTableNode VisitQueryNoWith(QueryNoWithContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        private static QsiTableDirectivesNode VisitWith(WithContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }
    }
}

using System.Threading;
using Qsi.Data;
using Qsi.Parsing;
using Qsi.Tree;
using Qsi.Trino.Internal;
using Qsi.Trino.Tree.Visitors;
using Qsi.Utilities;

namespace Qsi.Trino
{
    using static SqlBaseParser;

    public class TrinoParser : IQsiTreeParser
    {
        public IQsiTreeNode Parse(QsiScript script, CancellationToken cancellationToken = default)
        {
            var parser = TrinoUtility.CreateParser(script.Script);

            var statement = parser.singleStatement().statement();

            return ParseTrinoStatement(statement);
        }

        private static IQsiTreeNode ParseTrinoStatement(StatementContext context)
        {
            switch (context)
            {
                case InsertIntoContext insertInto:
                    return ActionVisitor.VisitInsertInto(insertInto);

                case UpdateContext update:
                    return ActionVisitor.VisitUpdate(update);

                case DeleteContext delete:
                    return ActionVisitor.VisitDelete(delete);

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
        }
    }
}

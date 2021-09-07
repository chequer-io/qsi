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
            var (_, result) = SqlParser.Parse(script.Script, p => p.singleStatement());

            var statement = result.statement();

            switch (statement)
            {
                case InsertIntoContext insertInto:
                    return ActionVisitor.VisitInsertInto(insertInto);

                case UpdateContext update:
                    return ActionVisitor.VisitUpdate(update);

                case DeleteContext delete:
                    return ActionVisitor.VisitDelete(delete);

                default:
                    throw TreeHelper.NotSupportedTree(statement);
            }
        }
    }
}

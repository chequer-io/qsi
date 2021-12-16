using System.Threading;
using Qsi.Athena.Internal;
using Qsi.Athena.Tree.Visitors;
using Qsi.Data;
using Qsi.Parsing;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Athena
{
    using static SqlBaseParser;

    public class AthenaParser : IQsiTreeParser
    {
        public IQsiTreeNode Parse(QsiScript script, CancellationToken cancellationToken = default)
        {
            var (_, result) = SqlParser.Parse(script.Script, parser => parser.singleStatement());

            var statement = result.statement();

            switch (statement)
            {
                case StatementDefaultContext query:
                    return TableVisitor.VisitQuery(query.query());

                case CreateViewContext createView:
                    return ActionVisitor.VisitCreateView(createView);

                case InsertIntoContext insertInto:
                    return ActionVisitor.VisitInsertInto(insertInto);

                case DeleteContext delete:
                    return ActionVisitor.VisitDelete(delete);

                default:
                    throw TreeHelper.NotSupportedTree(statement);
            }
        }
    }
}

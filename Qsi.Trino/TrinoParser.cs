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

                case StatementDefaultContext statementDefault:
                    return TableVisitor.VisitQuery(statementDefault.query());

                case MergeContext merge:
                    return ActionVisitor.VisitMerge(merge);

                case CreateViewContext createView:
                    return ActionVisitor.VisitCreateView(createView);

                case UseContext use:
                    return ActionVisitor.VisitUse(use);
                
                default:
                    throw TreeHelper.NotSupportedTree(statement);
            }
        }
    }
}

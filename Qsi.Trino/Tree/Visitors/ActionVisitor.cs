using System.Linq;
using Qsi.Data;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Trino.Internal;

namespace Qsi.Trino.Tree.Visitors
{
    using static SqlBaseParser;

    internal static class ActionVisitor
    {
        public static QsiDataInsertActionNode VisitInsertInto(InsertIntoContext context)
        {
            var name = context.qualifiedName();
            var columnAliases = context.columnAliases();
            var query = context.query();

            var node = TrinoTree.CreateWithSpan<QsiDataInsertActionNode>(context);

            var targetNode = TrinoTree.CreateWithSpan<QsiTableReferenceNode>(context);
            targetNode.Identifier = name.qqi;
            node.Target.Value = targetNode;

            if (columnAliases is not null)
                node.Columns.AddRange(columnAliases.identifier().Select(identifierContext => new QsiQualifiedIdentifier(identifierContext.qi)));

            return node;
        }
    }
}

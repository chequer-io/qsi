using Qsi.Cql.Tree.Common;
using Qsi.Tree.Definition;
using static Qsi.Cql.Internal.CqlParserInternal;

namespace Qsi.Cql.Tree;

internal static class DefinitionVisitor
{
    public static QsiViewDefinitionNode VisitCreateMaterializedViewStatement(CreateMaterializedViewStatementContext context)
    {
        var node = new QsiViewDefinitionNode
        {
            Identifier = context.cf.id
        };

        node.Source.SetValue(TableVisitor.VisitCommonSelectStatement(new CommonSelectStatementContext(context)));

        CqlTree.PutContextSpan(node, context);

        return node;
    }
}
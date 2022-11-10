using System;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Tree.Definition;
using Qsi.Utilities;

using static Qsi.PostgreSql.Internal.PostgreSqlParserInternal;

namespace Qsi.PostgreSql.Tree.Visitors;

internal static class DefinitionVisitor
{
    public static IQsiDefinitionNode VisitCreateStatement(CreateStatementContext context)
    {
        var node = context.children[0] switch
        {
            CreateViewStatementContext createViewContext => VisitCreateViewStatement(createViewContext),
            _ => throw TreeHelper.NotSupportedTree(context.children[0])
        };
        
        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static IQsiDefinitionNode VisitCreateViewStatement(CreateViewStatementContext context)
    {
        var viewNameContext = context.qualifiedIdentifier();
        var viewIdentifier = IdentifierVisitor.VisitQualifiedIdentifier(viewNameContext);

        var conflictBehavior = context.REPLACE() != null
            ? QsiDefinitionConflictBehavior.Replace
            : QsiDefinitionConflictBehavior.None;
        
        var node = new QsiViewDefinitionNode
        {
            Identifier = viewIdentifier,
            ConflictBehavior = conflictBehavior,
        };

        var sequentialList = context.columnIdentifierList();

        node.Columns.Value = sequentialList != null
            ? TableVisitor.CreateSequentialColumns(sequentialList)
            : TreeHelper.CreateAllColumnsDeclaration();
        
        node.Source.Value = TableVisitor.VisitSelectStatement(context.selectStatement());
        
        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }
}

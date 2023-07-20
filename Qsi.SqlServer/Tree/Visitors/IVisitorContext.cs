namespace Qsi.SqlServer.Tree;

internal interface IVisitorContext
{
    TableVisitor TableVisitor { get; }

    ExpressionVisitor ExpressionVisitor { get; }

    IdentifierVisitor IdentifierVisitor { get; }

    ActionVisitor ActionVisitor { get; }

    DefinitionVisitor DefinitionVisitor { get; }
}
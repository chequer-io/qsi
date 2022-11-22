using Qsi.PostgreSql.Tree.PG10;

namespace Qsi.PostgreSql.Tree
{
    internal interface IPgVisitorSet
    {
        PgActionVisitor ActionVisitor { get; }

        PgTableVisitor TableVisitor { get; }

        PgExpressionVisitor ExpressionVisitor { get; }

        PgDefinitionVisitor DefinitionVisitor { get; }

        PgIdentifierVisitor IdentifierVisitor { get; }
    }
}

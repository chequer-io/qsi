using Qsi.PostgreSql.OldTree.PG10;

namespace Qsi.PostgreSql.OldTree
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

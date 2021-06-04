using Qsi.PostgreSql.Tree.PG10;

namespace Qsi.PostgreSql.Tree
{
    internal interface IPgVisitorSet
    {
        PgActionVisitor ActionVisitor { get; }

        PgTableVisitor TableVisitor { get; }

        PgExpressionVisitor ExpressionVisitor { get; }

        PgIdentifierVisitor IdentifierVisitor { get; }
    }
}

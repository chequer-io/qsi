using Qsi.PostgreSql.OldTree.PG10;

namespace Qsi.PostgreSql.OldTree
{
    internal abstract class PgVisitorBase
    {
        protected PgActionVisitor ActionVisitor => _set.ActionVisitor;

        protected PgTableVisitor TableVisitor => _set.TableVisitor;

        protected PgExpressionVisitor ExpressionVisitor => _set.ExpressionVisitor;

        protected PgDefinitionVisitor DefinitionVisitor => _set.DefinitionVisitor;

        protected PgIdentifierVisitor IdentifierVisitor => _set.IdentifierVisitor;

        private readonly IPgVisitorSet _set;

        protected PgVisitorBase(IPgVisitorSet set)
        {
            _set = set;
        }
    }
}

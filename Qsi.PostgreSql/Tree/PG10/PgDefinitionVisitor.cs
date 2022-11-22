using System.Linq;
using Qsi.Data;
using Qsi.PostgreSql.Internal;
using Qsi.PostgreSql.Internal.PG10.Types;
using Qsi.PostgreSql.Tree.PG10.Nodes;
using Qsi.Tree;
using Qsi.Tree.Definition;
using Qsi.Utilities;

namespace Qsi.PostgreSql.Tree.PG10
{
    internal class PgDefinitionVisitor : PgVisitorBase
    {
        public PgDefinitionVisitor(IPgVisitorSet set) : base(set)
        {
        }

        public IQsiDefinitionNode VisitViewStmt(ViewStmt stmt)
        {
            var node = new PgViewDefinitionNode
            {
                Identifier = IdentifierVisitor.VisitRangeVar(stmt.view[0]),
                Source = { Value = TableVisitor.Visit(stmt.query[0]) },
                CheckOption = stmt.withCheckOption?.ToString()
            };

            if (stmt.replace ?? false)
                node.ConflictBehavior = QsiDefinitionConflictBehavior.Replace;

            // stmt.options: DefElem[]
            // Syntax: WITH ( key=<value_expression> [, ...] )

            if (!ListUtility.IsNullOrEmpty(stmt.aliases))
            {
                node.Columns.Value = new QsiColumnsDeclarationNode();
                node.Columns.Value.Columns.AddRange(TableVisitor.CreateSequentialColumnNodes(stmt.aliases.Cast<PgString>()));
            }

            return node;
        }

        public IQsiDefinitionNode VisitCreateTableAsStmt(CreateTableAsStmt stmt)
        {
            if (stmt.relkind != ObjectType.OBJECT_MATVIEW)
                throw TreeHelper.NotSupportedTree($"{nameof(CreateTableAsStmt)}({stmt.relkind})");

            var node = new QsiViewDefinitionNode
            {
                Identifier = IdentifierVisitor.VisitRangeVar(stmt.into[0].rel[0]),
                Source = { Value = TableVisitor.Visit(stmt.query[0]) }
            };

            if (stmt.if_not_exists ?? false)
                node.ConflictBehavior = QsiDefinitionConflictBehavior.Ignore;

            return node;
        }
    }
}

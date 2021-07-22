using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Antlr4.Runtime;
using Qsi.Data;
using Qsi.Impala.Internal;
using Qsi.Impala.Tree.Visitors;
using Qsi.Impala.Utilities;
using Qsi.Parsing;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Impala
{
    using static ImpalaParserInternal;

    public sealed class ImpalaParser : IQsiTreeParser
    {
        public ImpalaDialect Dialect { get; }

        public ImpalaParser(ImpalaDialect dialect)
        {
            Dialect = dialect ?? throw new ArgumentNullException(nameof(dialect));
        }

        public IQsiTreeNode Parse(QsiScript script, CancellationToken cancellationToken = default)
        {
            var parser = ImpalaUtility.CreateParserInternal(
                script.Script,
                Dialect
            );

            var stmt = parser.root().stmt();

            switch (stmt.children[0])
            {
                case Query_stmtContext queryStmt:
                    return TableVisitor.VisitQueryStmt(queryStmt);

                case Create_view_stmtContext createViewStmt:
                    return ActionVisitor.VisitCreateViewStmt(createViewStmt);

                case Create_tbl_as_select_stmtContext createTblAsSelectStmt:
                    return ActionVisitor.VisitCreateTblAsSelectStmt(createTblAsSelectStmt);

                case Use_stmtContext useStmt:
                    return ActionVisitor.VisitUseStmt(useStmt);

                case Upsert_stmtContext upsertStmt:
                    return ActionVisitor.VisitUpsertStmt(upsertStmt);

                case Update_stmtContext updateStmt:
                    return ActionVisitor.VisitUpdateStmt(updateStmt);

                case Insert_stmtContext insertStmt:
                    return ActionVisitor.VisitInsertStmt(insertStmt);

                case Delete_stmtContext deleteStmt:
                    return ActionVisitor.VisitDeleteStmt(deleteStmt);

                default:
                    throw TreeHelper.NotSupportedTree(stmt.children[0]);
            }
        }
    }
}

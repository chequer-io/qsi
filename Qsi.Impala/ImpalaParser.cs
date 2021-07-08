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
        public Version Version { get; }

        private ISet<string> _builtInFunctions;

        public ImpalaParser(Version version)
        {
            Version = version;
        }

        public void SetupBuiltInFunctions(IEnumerable<string> functions)
        {
            _builtInFunctions = new HashSet<string>(functions);
        }

        public IQsiTreeNode Parse(QsiScript script, CancellationToken cancellationToken = default)
        {
            var parser = ImpalaUtility.CreateParserInternal(
                script.Script,
                Version,
                _builtInFunctions ?? Enumerable.Empty<string>()
            );

            var stmt = parser.root().stmt();

            switch (stmt.children[0])
            {
                case Query_stmtContext queryStmt:
                    return TableVisitor.VisitQueryStmt(queryStmt);

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

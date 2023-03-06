using System;
using System.Threading;
using Qsi.Data;
using Qsi.Parsing;
using Qsi.PostgreSql.Internal;
using Qsi.PostgreSql.Internal.PG10;
using Qsi.PostgreSql.Internal.PG10.Types;
using Qsi.PostgreSql.OldTree;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PostgreSql
{
    public class PostgreSqlLegacyParser : IQsiTreeParser, IDisposable
    {
        private IPgParser? _pgParser;

        public IQsiTreeNode Parse(QsiScript script, CancellationToken cancellationToken = default)
        {
            _pgParser ??= new PgQuery10();

            var pgTree = (IPg10Node)_pgParser.Parse(script.Script, cancellationToken) ?? throw new QsiException(QsiError.NotSupportedScript, script.ScriptType);
            var pgVisitorSet = _pgParser.CreateVisitorSet();

            switch (pgTree)
            {
                case RawStmt rawStmt:
                    return ParseRawStmt(pgVisitorSet, rawStmt);

                default:
                    throw TreeHelper.NotSupportedTree(pgTree);
            }
        }

        private IQsiTreeNode ParseRawStmt(IPgVisitorSet visitorSet, RawStmt rawStmt)
        {
            switch (rawStmt.stmt[0])
            {
                case VariableSetStmt variableSetStmt:
                    return visitorSet.ActionVisitor.VisitVariableSetStmt(variableSetStmt);

                case SelectStmt selectStmt:
                    return visitorSet.TableVisitor.VisitSelectStmt(selectStmt);

                case InsertStmt insertStmt:
                    return visitorSet.ActionVisitor.VisitInsertStmt(insertStmt);

                case UpdateStmt updateStmt:
                    return visitorSet.ActionVisitor.VisitUpdateStmt(updateStmt);

                case DeleteStmt deleteStmt:
                    return visitorSet.ActionVisitor.VisitDeleteStmt(deleteStmt);

                case ViewStmt viewStmt:
                    return visitorSet.DefinitionVisitor.VisitViewStmt(viewStmt);

                case CreateTableAsStmt createTableAsStmt:
                    return visitorSet.DefinitionVisitor.VisitCreateTableAsStmt(createTableAsStmt);
            }

            throw TreeHelper.NotSupportedTree(rawStmt.stmt[0]);
        }

        void IDisposable.Dispose()
        {
            _pgParser?.Dispose();
        }
    }
}

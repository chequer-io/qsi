using System.Threading;
using Qsi.Athena.Internal;
using Qsi.Athena.Tree.Visitors;
using Qsi.Data;
using Qsi.Parsing;
using Qsi.Tree;
using Qsi.Utilities;
using static Qsi.Athena.Internal.SqlBaseParser;

namespace Qsi.Athena
{
    public class AthenaParser : IQsiTreeParser
    {
        public IQsiTreeNode Parse(QsiScript script, CancellationToken cancellationToken = default)
        {
            var (_, result) = SqlParser.Parse(script.Script, parser => parser.singleStatement());

            var statement = result.statement();

            switch (statement)
            {
                case CreateDatabaseContext createDatabase:
                    return ActionVisitor.VisitCreateDatabase(createDatabase);

                case DropDatabaseContext dropDatabase:
                    return ActionVisitor.VisitDropDatabase(dropDatabase);

                case SetDbPropertiesContext setDbProperties:
                    return ActionVisitor.VisitSetDbProperties(setDbProperties);

                case CreateTableContext createTable:
                    return ActionVisitor.VisitCreateTable(createTable);

                case CreateTableLikeContext createTableLike:
                    return ActionVisitor.VisitCreateTableLike(createTableLike);

                case DropTableContext dropTable:
                    return ActionVisitor.VisitDropTable(dropTable);

                case SetTblPropertiesContext setTblProperties:
                    return ActionVisitor.VisitSetTblProperties(setTblProperties);

                case RepairTableContext repairTable:
                    return ActionVisitor.VisitRepairTable(repairTable);

                case SetLocationContext setLocation:
                    return ActionVisitor.VisitSetLocation(setLocation);

                case AddPartitionsContext addPartitions:
                    return ActionVisitor.VisitAddPartitions(addPartitions);

                case RenamePartitionContext renamePartition:
                    return ActionVisitor.VisitRenamePartition(renamePartition);

                case DropPartitionContext dropPartition:
                    return ActionVisitor.VisitDropPartition(dropPartition);

                case ChangeColumnContext changeColumn:
                    return ActionVisitor.VisitChangeColumn(changeColumn);

                case AddColumnsContext addColumns:
                    return ActionVisitor.VisitAddColumns(addColumns);

                case ReplaceColumnsContext replaceColumns:
                    return ActionVisitor.VisitReplaceColumns(replaceColumns);

                case CreateViewContext createView:
                    return ActionVisitor.VisitCreateView(createView);

                case DropViewContext dropView:
                    return ActionVisitor.VisitDropView(dropView);

                case SetViewTblPropertiesContext setViewTblProperties:
                    return ActionVisitor.VisitSetViewTblProperties(setViewTblProperties);

                case AlterViewAsSelectContext alterViewAsSelect:
                    return ActionVisitor.VisitAlterViewAsSelect(alterViewAsSelect);

                case ShowDatabasesContext showDatabases:
                    return ActionVisitor.VisitShowDatabases(showDatabases);

                case ShowTablesContext showTables:
                    return ActionVisitor.VisitShowTables(showTables);

                case ShowViewsContext showViews:
                    return ActionVisitor.VisitShowViews(showViews);

                case ShowPartitionsContext showPartitions:
                    return ActionVisitor.VisitShowPartitions(showPartitions);

                case ShowTableExtendedContext showTableExtended:
                    return ActionVisitor.VisitShowTableExtended(showTableExtended);

                case ShowTablePropertiesContext showTableProperties:
                    return ActionVisitor.VisitShowTableProperties(showTableProperties);

                case ShowCreateTableContext showCreateTable:
                    return ActionVisitor.VisitShowCreateTable(showCreateTable);

                case ShowColumnsContext showColumns:
                    return ActionVisitor.VisitShowColumns(showColumns);

                case DescribeTableContext describeTable:
                    return ActionVisitor.VisitDescribeTable(describeTable);

                case DeallocateContext deallocate:
                    return ActionVisitor.VisitDeallocate(deallocate);

                case ExecuteContext execute:
                    return ActionVisitor.VisitExecute(execute);

                case ExplainContext explain:
                    return ActionVisitor.VisitExplain(explain);

                case ExplainAnalyzeContext explainAnalyze:
                    return ActionVisitor.VisitExplainAnalyze(explainAnalyze);

                case InsertIntoContext insertInto:
                    return ActionVisitor.VisitInsertInto(insertInto);

                case PrepareContext prepare:
                    return ActionVisitor.VisitPrepare(prepare);

                case StatementDefaultContext statementDefault:
                    return ActionVisitor.VisitStatementDefault(statementDefault);

                case UnloadContext unload:
                    return ActionVisitor.VisitUnload(unload);

                default:
                    throw TreeHelper.NotSupportedTree(statement);
            }
        }
    }
}

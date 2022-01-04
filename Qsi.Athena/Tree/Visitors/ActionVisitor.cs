using Qsi.Athena.Internal;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Athena.Tree.Visitors
{
    using static SqlBaseParser;

    internal static class ActionVisitor
    {
        public static IQsiTreeNode VisitCreateDatabase(CreateDatabaseContext context)
        {
            // TODO: Question: How to create node of CreateDatabase?, Other vendors(Mysql, Trino, Oracle) isn't support?
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitDropDatabase(DropDatabaseContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitSetDbProperties(SetDbPropertiesContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitCreateTable(CreateTableContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitCreateTableLike(CreateTableLikeContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitDropTable(DropTableContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitSetTblProperties(SetTblPropertiesContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitRepairTable(RepairTableContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitSetLocation(SetLocationContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitAddPartitions(AddPartitionsContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitRenamePartition(RenamePartitionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitDropPartition(DropPartitionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitChangeColumn(ChangeColumnContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitAddColumns(AddColumnsContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitReplaceColumns(ReplaceColumnsContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitCreateView(CreateViewContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitDropView(DropViewContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitSetViewTblProperties(SetViewTblPropertiesContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitAlterViewAsSelect(AlterViewAsSelectContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitShowDatabases(ShowDatabasesContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitShowTables(ShowTablesContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitShowViews(ShowViewsContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitShowPartitions(ShowPartitionsContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitShowTableExtended(ShowTableExtendedContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitShowTableProperties(ShowTablePropertiesContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitShowCreateTable(ShowCreateTableContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitShowColumns(ShowColumnsContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitDescribeTable(DescribeTableContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitDeallocate(DeallocateContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitExecute(ExecuteContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitExplain(ExplainContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitExplainAnalyze(ExplainAnalyzeContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitInsertInto(InsertIntoContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitPrepare(PrepareContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitStatementDefault(StatementDefaultContext context)
        {
            return TableVisitor.VisitQuery(context.query());
        }

        public static IQsiTreeNode VisitUnload(UnloadContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }
    }
}

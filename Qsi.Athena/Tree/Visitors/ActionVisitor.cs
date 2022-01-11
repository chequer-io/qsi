using System.Collections.Generic;
using System.Linq;
using Qsi.Athena.Internal;
using Qsi.Athena.Tree.Nodes;
using Qsi.Data;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Tree.Definition;
using Qsi.Utilities;

namespace Qsi.Athena.Tree.Visitors
{
    using static SqlBaseParser;

    internal static class ActionVisitor
    {
        public static IQsiTreeNode VisitCreateDatabase(CreateDatabaseContext context)
        {
            // Ignore CreateDatabase Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitDropDatabase(DropDatabaseContext context)
        {
            // Ignore DropDatabase Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitSetDbProperties(SetDbPropertiesContext context)
        {
            // Ignore SetDbProperties Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitCreateTable(CreateTableContext context)
        {
            var tableName = context.tableName;
            
            var node = AthenaTree.CreateWithSpan<QsiTableDefinitionNode>(context);
            node.Identifier = tableName.qqi;
            
            // Ignore specific options of CreateTable Action

            return node;
        }

        public static IQsiTreeNode VisitCreateTableAs(CreateTableAsContext context)
        {
            var tableName = context.tableName;
            var query = context.query();

            // Ignore specific options CreateTable Action

            var node = AthenaTree.CreateWithSpan<QsiTableDefinitionNode>(context);
            node.Identifier = tableName.qqi;
            
            var queryNode = TableVisitor.VisitQuery(query);
            node.ColumnSource.Value = queryNode;
            
            // check WITH NO DATA
            if (!context.HasToken(NO))
                node.DataSource.Value = queryNode;

            return node;
        }

        public static IQsiTreeNode VisitCreateTableLike(CreateTableLikeContext context)
        {
            var tableName = context.tableName;
            var likeTable = context.likeTableName;
            
            var node = AthenaTree.CreateWithSpan<QsiTableDefinitionNode>(context);
            node.Identifier = tableName.qqi;
            
            node.ColumnSource.Value = TableVisitor.VisitQualifiedName(likeTable);

            return node;
        }

        public static IQsiTreeNode VisitDropTable(DropTableContext context)
        {
            // Ignore DropTable Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitSetTblProperties(SetTblPropertiesContext context)
        {
            // Ignore SetTblProperties Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitRepairTable(RepairTableContext context)
        {
            // Ignore RepairTable Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitSetLocation(SetLocationContext context)
        {
            // Ignore SetLocation Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitAddPartitions(AddPartitionsContext context)
        {
            // Ignore AddPartitions Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitRenamePartition(RenamePartitionContext context)
        {
            // Ignore RenamePartition Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitDropPartition(DropPartitionContext context)
        {
            // Ignore DropPartition Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitChangeColumn(ChangeColumnContext context)
        {
            // Ignore ChangeColumn Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitAddColumns(AddColumnsContext context)
        {
            // Ignore AddColumns Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitReplaceColumns(ReplaceColumnsContext context)
        {
            // Ignore ReplaceColumns Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitCreateView(CreateViewContext context)
        {
            // Ignore CreateView Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitDropView(DropViewContext context)
        {
            // Ignore DropView Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitSetViewTblProperties(SetViewTblPropertiesContext context)
        {
            // Ignore SetViewTblProperties Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitAlterViewAsSelect(AlterViewAsSelectContext context)
        {
            // Ignore AlterViewSelect Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitShowDatabases(ShowDatabasesContext context)
        {
            // Ignore ShowDatabases Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitShowTables(ShowTablesContext context)
        {
            // Ignore ShowTables Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitShowViews(ShowViewsContext context)
        {
            // Ignore ShowViews Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitShowPartitions(ShowPartitionsContext context)
        {
            // Ignore ShowPartitions Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitShowTableExtended(ShowTableExtendedContext context)
        {
            // Ignore ShowTableExtended Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitShowTableProperties(ShowTablePropertiesContext context)
        {
            // Ignore ShowTableProperties Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitShowCreateTable(ShowCreateTableContext context)
        {
            // Ignore ShowCreateTable Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitShowColumns(ShowColumnsContext context)
        {
            // Ignore ShowColumns Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitDescribeTable(DescribeTableContext context)
        {
            // Ignore DescribeTable Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitDeallocate(DeallocateContext context)
        {
            // Ignore Deallocate Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitExecute(ExecuteContext context)
        {
            // TODO: Support Prepared statement
            throw TreeHelper.NotSupportedFeature("PREPARED STATEMENT");
        }

        public static IQsiTreeNode VisitExplain(ExplainContext context)
        {
            // Ignore Explain Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitExplainAnalyze(ExplainAnalyzeContext context)
        {
            // Ignore ExplainAnalyze Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static IQsiTreeNode VisitInsertInto(InsertIntoContext context)
        {
            var qualifiedName = context.qualifiedName();
            var columnAliases = context.columnAliases();
            var query = context.query();

            var node = AthenaTree.CreateWithSpan<QsiDataInsertActionNode>(context);
            node.Target.Value = TableVisitor.VisitQualifiedName(qualifiedName);

            if (columnAliases is not null)
            {
                node.Columns.AddRange(
                    columnAliases.identifier().Select(identifier => new QsiQualifiedIdentifier(identifier.qi))
                );
            }

            node.ValueTable.Value = TableVisitor.VisitQuery(query);

            return node;
        }

        public static IQsiTreeNode VisitPrepare(PrepareContext context)
        {
            // TODO: Support Prepared statement
            throw TreeHelper.NotSupportedFeature("PREPARED STATEMENT");
        }

        public static IQsiTreeNode VisitStatementDefault(StatementDefaultContext context)
        {
            return TableVisitor.VisitQuery(context.query());
        }

        public static IQsiTreeNode VisitUnload(UnloadContext context)
        {
            var location = context.location;
            var query = context.query();
            
            // ignore properties

            var node = AthenaTree.CreateWithSpan<AthenaUnloadActionNode>(context);
            node.Query.Value = TableVisitor.VisitQuery(query);
            node.Location = location.GetText();
            
            return node;
        }
    }
}

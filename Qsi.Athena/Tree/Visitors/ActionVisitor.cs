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
        public static QsiTreeNode VisitStatement(StatementContext context)
        {
            return context switch
            {
                CreateDatabaseContext createDatabase => VisitCreateDatabase(createDatabase),
                DropDatabaseContext dropDatabase => VisitDropDatabase(dropDatabase),
                SetDbPropertiesContext setDbProperties => VisitSetDbProperties(setDbProperties),
                CreateTableContext createTable => VisitCreateTable(createTable),
                CreateTableAsContext createTableAs => VisitCreateTableAs(createTableAs),
                CreateTableLikeContext createTableLike => VisitCreateTableLike(createTableLike),
                DropTableContext dropTable => VisitDropTable(dropTable),
                SetTblPropertiesContext setTblProperties => VisitSetTblProperties(setTblProperties),
                RepairTableContext repairTable => VisitRepairTable(repairTable),
                SetLocationContext setLocation => VisitSetLocation(setLocation),
                AddPartitionsContext addPartitions => VisitAddPartitions(addPartitions),
                RenamePartitionContext renamePartition => VisitRenamePartition(renamePartition),
                DropPartitionContext dropPartition => VisitDropPartition(dropPartition),
                ChangeColumnContext changeColumn => VisitChangeColumn(changeColumn),
                AddColumnsContext addColumns => VisitAddColumns(addColumns),
                ReplaceColumnsContext replaceColumns => VisitReplaceColumns(replaceColumns),
                CreateViewContext createView => VisitCreateView(createView),
                DropViewContext dropView => VisitDropView(dropView),
                SetViewTblPropertiesContext setViewTblProperties => VisitSetViewTblProperties(setViewTblProperties),
                AlterViewAsSelectContext alterViewAsSelect => VisitAlterViewAsSelect(alterViewAsSelect),
                ShowDatabasesContext showDatabases => VisitShowDatabases(showDatabases),
                ShowTablesContext showTables => VisitShowTables(showTables),
                ShowViewsContext showViews => VisitShowViews(showViews),
                ShowPartitionsContext showPartitions => VisitShowPartitions(showPartitions),
                ShowTableExtendedContext showTableExtended => VisitShowTableExtended(showTableExtended),
                ShowTablePropertiesContext showTableProperties => VisitShowTableProperties(showTableProperties),
                ShowCreateTableContext showCreateTable => VisitShowCreateTable(showCreateTable),
                ShowColumnsContext showColumns => VisitShowColumns(showColumns),
                DescribeTableContext describeTable => VisitDescribeTable(describeTable),
                DeallocateContext deallocate => VisitDeallocate(deallocate),
                ExecuteContext execute => VisitExecute(execute),
                ExplainContext explain => VisitExplain(explain),
                ExplainAnalyzeContext explainAnalyze => VisitExplainAnalyze(explainAnalyze),
                InsertIntoContext insertInto => VisitInsertInto(insertInto),
                PrepareContext prepare => VisitPrepare(prepare),
                StatementDefaultContext statementDefault => VisitStatementDefault(statementDefault),
                UnloadContext unload => VisitUnload(unload),
                _ => throw TreeHelper.NotSupportedTree(context)
            };
        }
        
        public static QsiTreeNode VisitCreateDatabase(CreateDatabaseContext context)
        {
            // Ignore CreateDatabase Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiTreeNode VisitDropDatabase(DropDatabaseContext context)
        {
            // Ignore DropDatabase Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiTreeNode VisitSetDbProperties(SetDbPropertiesContext context)
        {
            // Ignore SetDbProperties Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiTreeNode VisitCreateTable(CreateTableContext context)
        {
            var tableName = context.tableName;

            var node = AthenaTree.CreateWithSpan<QsiTableDefinitionNode>(context);
            node.Identifier = tableName.qqi;

            // Ignore specific options of CreateTable Action

            return node;
        }

        public static QsiTreeNode VisitCreateTableAs(CreateTableAsContext context)
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

        public static QsiTreeNode VisitCreateTableLike(CreateTableLikeContext context)
        {
            var tableName = context.tableName;
            var likeTable = context.likeTableName;

            var node = AthenaTree.CreateWithSpan<QsiTableDefinitionNode>(context);
            node.Identifier = tableName.qqi;

            node.ColumnSource.Value = TableVisitor.VisitQualifiedName(likeTable);

            return node;
        }

        public static QsiTreeNode VisitDropTable(DropTableContext context)
        {
            // Ignore DropTable Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiTreeNode VisitSetTblProperties(SetTblPropertiesContext context)
        {
            // Ignore SetTblProperties Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiTreeNode VisitRepairTable(RepairTableContext context)
        {
            // Ignore RepairTable Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiTreeNode VisitSetLocation(SetLocationContext context)
        {
            // Ignore SetLocation Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiTreeNode VisitAddPartitions(AddPartitionsContext context)
        {
            // Ignore AddPartitions Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiTreeNode VisitRenamePartition(RenamePartitionContext context)
        {
            // Ignore RenamePartition Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiTreeNode VisitDropPartition(DropPartitionContext context)
        {
            // Ignore DropPartition Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiTreeNode VisitChangeColumn(ChangeColumnContext context)
        {
            // Ignore ChangeColumn Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiTreeNode VisitAddColumns(AddColumnsContext context)
        {
            // Ignore AddColumns Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiTreeNode VisitReplaceColumns(ReplaceColumnsContext context)
        {
            // Ignore ReplaceColumns Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiTreeNode VisitCreateView(CreateViewContext context)
        {
            // Ignore CreateView Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiTreeNode VisitDropView(DropViewContext context)
        {
            // Ignore DropView Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiTreeNode VisitSetViewTblProperties(SetViewTblPropertiesContext context)
        {
            // Ignore SetViewTblProperties Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiTreeNode VisitAlterViewAsSelect(AlterViewAsSelectContext context)
        {
            // Ignore AlterViewSelect Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiTreeNode VisitShowDatabases(ShowDatabasesContext context)
        {
            // Ignore ShowDatabases Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiTreeNode VisitShowTables(ShowTablesContext context)
        {
            // Ignore ShowTables Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiTreeNode VisitShowViews(ShowViewsContext context)
        {
            // Ignore ShowViews Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiTreeNode VisitShowPartitions(ShowPartitionsContext context)
        {
            // Ignore ShowPartitions Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiTreeNode VisitShowTableExtended(ShowTableExtendedContext context)
        {
            // Ignore ShowTableExtended Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiTreeNode VisitShowTableProperties(ShowTablePropertiesContext context)
        {
            // Ignore ShowTableProperties Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiTreeNode VisitShowCreateTable(ShowCreateTableContext context)
        {
            // Ignore ShowCreateTable Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiTreeNode VisitShowColumns(ShowColumnsContext context)
        {
            // Ignore ShowColumns Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiTreeNode VisitDescribeTable(DescribeTableContext context)
        {
            // Ignore DescribeTable Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiTreeNode VisitDeallocate(DeallocateContext context)
        {
            var identifier = context.identifier();

            var node = AthenaTree.CreateWithSpan<QsiDropPrepareActionNode>(context);
            node.Identifier = new QsiQualifiedIdentifier(identifier.qi);

            return node;
        }

        public static QsiTreeNode VisitExecute(ExecuteContext context)
        {
            var identifier = context.identifier();
            ExpressionContext[] expressions = context.expression();

            var variablesNode = AthenaTree.CreateWithSpan<QsiMultipleExpressionNode>(context.USING().Symbol, context.Stop);
            variablesNode.Elements.AddRange(expressions.Select(ExpressionVisitor.VisitExpression));
            
            var node = AthenaTree.CreateWithSpan<QsiExecutePrepareActionNode>(context);
            node.Identifier = new QsiQualifiedIdentifier(identifier.qi);
            node.Variables.Value = variablesNode;

            return node;
        }

        public static QsiTreeNode VisitExplain(ExplainContext context)
        {
            // Ignore Explain Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiTreeNode VisitExplainAnalyze(ExplainAnalyzeContext context)
        {
            // Ignore ExplainAnalyze Action
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiTreeNode VisitInsertInto(InsertIntoContext context)
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

        public static QsiTreeNode VisitPrepare(PrepareContext context)
        {
            var identifier = context.identifier();
            var statement = context.statement();

            var node = AthenaTree.CreateWithSpan<QsiPrepareActionNode>(context);
            node.Identifier = new QsiQualifiedIdentifier(identifier.qi);

            var queryNode = AthenaTree.CreateWithSpan<AthenaStatementExpressionNode>(statement);
            queryNode.Expression.Value = VisitStatement(statement); 
            node.Query.Value = queryNode;

            return node;
        }

        public static QsiTreeNode VisitStatementDefault(StatementDefaultContext context)
        {
            return TableVisitor.VisitQuery(context.query());
        }

        public static QsiTreeNode VisitUnload(UnloadContext context)
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

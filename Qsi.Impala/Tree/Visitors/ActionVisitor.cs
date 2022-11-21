using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Qsi.Data;
using Qsi.Impala.Internal;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Tree.Definition;
using Qsi.Utilities;

namespace Qsi.Impala.Tree.Visitors
{
    using static ImpalaParserInternal;

    internal static class ActionVisitor
    {
        public static IQsiTreeNode VisitUseStmt(Use_stmtContext context)
        {
            var node = ImpalaTree.CreateWithSpan<QsiChangeSearchPathActionNode>(context);
            node.Identifiers = new[]
            {
                new QsiQualifiedIdentifier(IdentifierVisitor.VisitIdentOrDefault(context.ident_or_default()))
            };
            return node;
        }

        public static IQsiTreeNode VisitCreateViewStmt(Create_view_stmtContext context)
        {
            var node = ImpalaTree.CreateWithSpan<QsiViewDefinitionNode>(context);

            node.ConflictBehavior = context.HasRule<If_not_exists_valContext>() ?
                QsiDefinitionConflictBehavior.Ignore :
                QsiDefinitionConflictBehavior.None;

            node.Identifier = IdentifierVisitor.VisitTableName(context.table_name());

            node.Columns.Value = context.TryGetRuleContext<View_column_defsContext>(out var viewColumnDefs) ?
                VisitViewColumnDefs(viewColumnDefs) :
                TreeHelper.CreateAllVisibleColumnsDeclaration();

            node.Source.Value = TableVisitor.VisitQueryStmt(context.query_stmt());

            return node;
        }

        private static QsiColumnsDeclarationNode VisitViewColumnDefs(View_column_defsContext context)
        {
            var node = ImpalaTree.CreateWithSpan<QsiColumnsDeclarationNode>(context);
            node.Columns.AddRange(VisitViewColumnDefList(context.view_column_def_list()));
            return node;
        }

        private static IEnumerable<QsiColumnNode> VisitViewColumnDefList(View_column_def_listContext context)
        {
            return context.view_column_def().Select(VisitViewColumnDef);
        }

        private static QsiColumnNode VisitViewColumnDef(View_column_defContext context)
        {
            var identOrDefault = context.ident_or_default();
            var node = ImpalaTree.CreateWithSpan<QsiColumnReferenceNode>(identOrDefault);
            node.Name = new QsiQualifiedIdentifier(IdentifierVisitor.VisitIdentOrDefault(identOrDefault));
            return node;
        }

        public static IQsiTreeNode VisitCreateTblAsSelectStmt(Create_tbl_as_select_stmtContext context)
        {
            var node = ImpalaTree.CreateWithSpan<ImpalaTableDefinitionNode>(context);
            node.PlanHints = context.plan_hints()?.GetInputText();
            VisitCreateTblAsSelectParams(node, context.create_tbl_as_select_params());
            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void VisitCreateTblAsSelectParams(ImpalaTableDefinitionNode node, Create_tbl_as_select_paramsContext context)
        {
            node.IsExternal = context.tblDef.external;
            node.ConflictBehavior = context.tblDef.ifNotExists ? QsiDefinitionConflictBehavior.Ignore : QsiDefinitionConflictBehavior.None;
            node.Identifier = IdentifierVisitor.VisitTableName(context.tblDef.table_name());
            node.DataSource.Value = TableVisitor.VisitQueryStmt(context.query);

            if (context.options.children?.Count > 0)
            {
                var fragment = ImpalaTree.CreateWithSpan<QsiExpressionFragmentNode>(context.options);
                fragment.Text = context.options.GetInputText();

                node.Options.Value = fragment;
            }

            if (context.TryGetRuleContext<Primary_keysContext>(out var primaryKeys))
                node.PrimaryKeyColumnNames = IdentifierVisitor.VisitIdentList(primaryKeys.ident_list()).ToArray();

            if (context.TryGetRuleContext<Partitioned_data_layoutContext>(out var partitionedDataLayout))
            {
                var fragment = ImpalaTree.CreateWithSpan<QsiExpressionFragmentNode>(context.options);
                fragment.Text = partitionedDataLayout.GetInputText();

                node.KuduPartitionParams.Value = fragment;
            }

            if (context.TryGetRuleContext<Iceberg_partition_spec_listContext>(out var icebergPartitionSpecList))
            {
                var fragment = ImpalaTree.CreateWithSpan<QsiExpressionFragmentNode>(context.options);
                fragment.Text = icebergPartitionSpecList.GetInputText();

                node.IcebergPartitionSpecs.Value = fragment;
            }

            if (context.TryGetRuleContext<Ident_listContext>(out var identList))
                node.PartitionColumnNames = IdentifierVisitor.VisitIdentList(identList).ToArray();
        }

        public static IQsiTreeNode VisitUpsertStmt(Upsert_stmtContext context)
        {
            var node = ImpalaTree.CreateWithSpan<ImpalaDataInsertActionNode>(context);

            if (context.with is not null)
                node.Directives.Value = TableVisitor.VisitWithClause(context.with);

            if (context.hint is not null)
                node.PlanHints = context.hint.GetInputText();

            node.ConflictBehavior = QsiDataConflictBehavior.Update;
            node.Target.Value = TableVisitor.VisitTableName(context.name);

            if (context.columns is not null)
            {
                node.Columns = IdentifierVisitor.VisitIdentList(context.columns)
                    .Select(i => new QsiQualifiedIdentifier(i))
                    .ToArray();
            }

            node.ValueTable.Value = TableVisitor.VisitQueryStmt(context.query);

            return node;
        }

        public static IQsiTreeNode VisitUpdateStmt(Update_stmtContext context)
        {
            var node = ImpalaTree.CreateWithSpan<QsiDataUpdateActionNode>(context);
            var wildcard = new QsiAllColumnNode();

            var targetTable = new QsiDerivedTableNode
            {
                Columns =
                {
                    Value = new QsiColumnsDeclarationNode
                    {
                        Columns =
                        {
                            wildcard
                        }
                    }
                }
            };

            if (context.from is not null)
            {
                if (context.target.children.Count != 1)
                    throw new QsiException(QsiError.SyntaxError, $"'{context.target.GetInputText()}' is not a valid table alias or reference.");

                wildcard.Path = IdentifierVisitor.VisitDottedPath(context.target);
                targetTable.Source.Value = TableVisitor.VisitFromClause(context.from);
            }
            else
            {
                targetTable.Source.Value = TableVisitor.VisitDottedPath(context.target);
            }

            if (context.where is not null)
                targetTable.Where.Value = ExpressionVisitor.VisitWhereClause(context.where);

            node.Target.Value = targetTable;
            node.SetValues.AddRange(ExpressionVisitor.VisitUpdateSetExprList(context.values));

            return node;
        }

        public static IQsiTreeNode VisitInsertStmt(Insert_stmtContext context)
        {
            var node = ImpalaTree.CreateWithSpan<ImpalaDataInsertActionNode>(context);

            if (context.with is not null)
                node.Directives.Value = TableVisitor.VisitWithClause(context.with);

            if (context.hint is not null)
                node.PlanHints = context.hint.GetInputText();

            node.ConflictBehavior = context.HasToken(KW_OVERWRITE) ?
                QsiDataConflictBehavior.Update :
                QsiDataConflictBehavior.None;

            node.Target.Value = TableVisitor.VisitTableName(context.name);

            if (context.columns is not null)
            {
                node.Columns = IdentifierVisitor.VisitIdentList(context.columns)
                    .Select(i => new QsiQualifiedIdentifier(i))
                    .ToArray();
            }

            if (context.partition is not null)
                node.Partition.Value = ExpressionVisitor.VisitPartitionClause(context.partition);

            node.ValueTable.Value = TableVisitor.VisitQueryStmt(context.query);

            return node;
        }

        public static IQsiTreeNode VisitDeleteStmt(Delete_stmtContext context)
        {
            var node = ImpalaTree.CreateWithSpan<QsiDataDeleteActionNode>(context);
            var wildcard = new QsiAllColumnNode();

            var targetTable = new QsiDerivedTableNode
            {
                Columns =
                {
                    Value = new QsiColumnsDeclarationNode
                    {
                        Columns =
                        {
                            wildcard
                        }
                    }
                }
            };

            if (context.from is not null)
            {
                if (context.target.children.Count != 1)
                    throw new QsiException(QsiError.SyntaxError, $"'{context.target.GetInputText()}' is not a valid table alias or reference.");

                wildcard.Path = IdentifierVisitor.VisitDottedPath(context.target);
                targetTable.Source.Value = TableVisitor.VisitFromClause(context.from);
            }
            else
            {
                targetTable.Source.Value = TableVisitor.VisitDottedPath(context.target);
            }

            if (context.where is not null)
                targetTable.Where.Value = ExpressionVisitor.VisitWhereClause(context.where);

            node.Target.Value = targetTable;

            return node;
        }
    }
}

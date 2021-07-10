using System;
using System.Collections.Generic;
using System.Linq;
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
            node.Identifiers = new[] { IdentifierVisitor.VisitIdentOrDefault(context.ident_or_default()) };
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
                TreeHelper.CreateAllColumnsDeclaration();

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
            throw new NotImplementedException();
        }

        public static IQsiTreeNode VisitUpsertStmt(Upsert_stmtContext context)
        {
            throw new NotImplementedException();
        }

        public static IQsiTreeNode VisitUpdateStmt(Update_stmtContext context)
        {
            throw new NotImplementedException();
        }

        public static IQsiTreeNode VisitInsertStmt(Insert_stmtContext context)
        {
            throw new NotImplementedException();
        }

        public static IQsiTreeNode VisitDeleteStmt(Delete_stmtContext context)
        {
            throw new NotImplementedException();
        }
    }
}

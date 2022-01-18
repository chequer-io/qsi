using System.Linq;
using Qsi.Data;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Tree.Definition;
using Qsi.Trino.Internal;
using Qsi.Utilities;

namespace Qsi.Trino.Tree.Visitors
{
    using static SqlBaseParser;

    internal static class ActionVisitor
    {
        public static QsiViewDefinitionNode VisitCreateView(CreateViewContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiViewDefinitionNode>(context);

            if (context.HasToken(REPLACE))
                node.ConflictBehavior = QsiDefinitionConflictBehavior.Replace;

            node.Identifier = context.qualifiedName().qqi;
            node.Source.Value = TableVisitor.VisitQuery(context.query());

            // ignored comment, security

            return node;
        }

        // TODO: Test
        public static TrinoMergeActionNode VisitMerge(MergeContext context)
        {
            var targetRef = TableVisitor.VisitQualifiedName(context.qualifiedName());
            var target = targetRef;
            var dataSource = TableVisitor.VisitRelation(context.relation());
            var onCondition = ExpressionVisitor.VisitExpression(context.expression());
            QsiQualifiedIdentifier dataSourceIdentifier;

            switch (dataSource)
            {
                case QsiTableReferenceNode dataRefSource:
                    dataSourceIdentifier = dataRefSource.Identifier;
                    break;

                case QsiDerivedTableNode { Alias: { IsEmpty: false } } dataDerivedSource:
                    dataSourceIdentifier = new QsiQualifiedIdentifier(dataDerivedSource.Alias.Value.Name);
                    break;

                default:
                    throw TreeHelper.NotSupportedFeature("Merge without subquery alias");
            }

            if (context.identifier() is not null)
            {
                target = new QsiTableReferenceNode { Identifier = new QsiQualifiedIdentifier(context.identifier().qi) };
            }

            var node = TrinoTree.CreateWithSpan<TrinoMergeActionNode>(context);

            foreach (var mergeCase in context.mergeCase())
            {
                node.ActionNodes.Add(mergeCase switch
                {
                    MergeInsertContext mergeInsert => VisitMergeInsert(mergeInsert, target, dataSource, dataSourceIdentifier, onCondition),
                    MergeDeleteContext mergeDelete => VisitMergeDelete(mergeDelete, target, dataSource, onCondition),
                    MergeUpdateContext mergeUpdate => VisitMergeUpdate(mergeUpdate, target, dataSource, onCondition),
                    _ => throw TreeHelper.NotSupportedTree(mergeCase)
                });
            }

            return node;
        }

        public static QsiActionNode VisitMergeDelete(
            MergeDeleteContext mergeDeleteContext,
            QsiTableReferenceNode left,
            QsiTableNode right,
            QsiExpressionNode onCondition)
        {
            var joinedTable = TrinoHelper.CreateJoinedTable(left, right, onCondition);

            var deleteTarget = new TrinoDerivedTableNode();
            deleteTarget.Source.Value = joinedTable;
            deleteTarget.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();

            var where = new QsiWhereExpressionNode();
            where.Expression.Value = ExpressionVisitor.VisitExpression(mergeDeleteContext.condition);
            deleteTarget.Where.Value = where;

            var deleteActionNode = new QsiDataDeleteActionNode();
            deleteActionNode.Target.Value = deleteTarget;

            return deleteActionNode;
        }

        public static QsiActionNode VisitMergeUpdate(
            MergeUpdateContext mergeUpdateContext,
            QsiTableReferenceNode left,
            QsiTableNode right,
            QsiExpressionNode onCondition)
        {
            var joinedTable = TrinoHelper.CreateJoinedTable(left, right, onCondition);
            QsiTableNode updateTarget = joinedTable;

            if (mergeUpdateContext.condition is not null)
            {
                var derivedTable = new TrinoDerivedTableNode();
                derivedTable.Source.Value = updateTarget;
                derivedTable.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();
                var where = new QsiWhereExpressionNode();
                where.Expression.Value = ExpressionVisitor.VisitExpression(mergeUpdateContext.condition);
                derivedTable.Where.Value = where;

                updateTarget = derivedTable;
            }

            var updateActionNode = new QsiDataUpdateActionNode();
            updateActionNode.Target.Value = updateTarget;

            for (int i = 0; i < mergeUpdateContext._targets.Count; i++)
            {
                var target = mergeUpdateContext._targets[i];
                var value = mergeUpdateContext._values[i];

                var setValueNode = TrinoTree.CreateWithSpan<QsiSetColumnExpressionNode>(target.Start, value.Stop);
                setValueNode.Target = new QsiQualifiedIdentifier(target.qi);
                setValueNode.Value.Value = ExpressionVisitor.VisitExpression(value);

                updateActionNode.SetValues.Add(setValueNode);
            }

            return updateActionNode;
        }

        public static QsiActionNode VisitMergeInsert(
            MergeInsertContext mergeInsertContext,
            QsiTableReferenceNode left,
            QsiTableNode right,
            QsiQualifiedIdentifier rightPath,
            QsiExpressionNode onCondition)
        {
            QsiTableDirectivesNode directives = null;
            var rightSource = right;

            if (right is not QsiTableReferenceNode)
            {
                var subqueryAlias = new QsiIdentifier($"\"SUBQUERY_{rightPath}\"", true);

                directives = CreateDirectiveTable(subqueryAlias, right);

                var rightDerivedSource = new QsiTableReferenceNode { Identifier = new QsiQualifiedIdentifier(subqueryAlias) };
                var rightAliasNode = new QsiAliasNode { Name = rightPath[0] };

                rightSource = TrinoHelper.CreateDerivedTable(rightDerivedSource, rightAliasNode);
            }

            var exceptJoinTableLeftSource = TrinoHelper.CreateDerivedTable(rightSource);

            var matchedItemsNode = TrinoHelper.CreateJoinedTable(left, rightSource, onCondition);
            var selectInJoinedTableNode = TrinoHelper.CreateDerivedTableWithPath(matchedItemsNode, rightPath);

            var exceptJoinTable = new QsiCompositeTableNode();
            exceptJoinTable.Sources.Add(exceptJoinTableLeftSource);
            exceptJoinTable.Sources.Add(selectInJoinedTableNode);
            exceptJoinTable.CompositeType = "EXCEPT";

            var minusJoinAliasedTable = TrinoHelper.CreateDerivedTable(exceptJoinTable, new QsiAliasNode { Name = rightPath[^1] });

            var itemSelectionSubqueryTable = new TrinoDerivedTableNode();

            if (directives is not null)
                itemSelectionSubqueryTable.Directives.Value = directives;

            itemSelectionSubqueryTable.Source.Value = minusJoinAliasedTable;

            var columnsDeclaration = new QsiColumnsDeclarationNode();

            foreach (var mergeValue in mergeInsertContext._values)
            {
                QsiColumnNode columnNode;

                var exprNode = ExpressionVisitor.VisitExpression(mergeValue);

                if (exprNode is QsiColumnExpressionNode columnExprNode)
                {
                    columnNode = columnExprNode.Column.Value;
                }
                else
                {
                    QsiDerivedColumnNode derivedColumnNode = new();
                    derivedColumnNode.Expression.Value = exprNode;

                    columnNode = derivedColumnNode;
                }

                columnsDeclaration.Columns.Add(columnNode);
            }

            itemSelectionSubqueryTable.Columns.Value = columnsDeclaration;

            // MERGE INSERT
            var deleteActionNode = new QsiDataInsertActionNode();
            deleteActionNode.ValueTable.Value = itemSelectionSubqueryTable;
            deleteActionNode.Target.Value = left;

            deleteActionNode.Columns = mergeInsertContext
                ._targets
                .Select(c => new QsiQualifiedIdentifier(c.qi))
                .ToArray();

            return deleteActionNode;
        }

        public static QsiDataInsertActionNode VisitInsertInto(InsertIntoContext context)
        {
            var name = context.qualifiedName();
            var columnAliases = context.columnAliases();
            var query = context.query();

            var node = TrinoTree.CreateWithSpan<QsiDataInsertActionNode>(context);

            var targetNode = TableVisitor.VisitQualifiedName(name);
            node.Target.Value = targetNode;

            if (columnAliases is not null)
            {
                node.Columns = columnAliases.identifier()
                    .Select(i => new QsiQualifiedIdentifier(i.qi))
                    .ToArray();
            }

            node.ValueTable.Value = TableVisitor.VisitQuery(query);

            return node;
        }

        public static QsiDataUpdateActionNode VisitUpdate(UpdateContext context)
        {
            var name = context.qualifiedName();
            UpdateAssignmentContext[] updateAssignment = context.updateAssignment();
            var where = context.where;

            var node = TrinoTree.CreateWithSpan<QsiDataUpdateActionNode>(context);

            var tableNode = TrinoTree.CreateWithSpan<QsiDerivedTableNode>(context);
            tableNode.Source.Value = TableVisitor.VisitQualifiedName(name);
            tableNode.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();

            if (where is not null)
                tableNode.Where.Value = ExpressionVisitor.VisitWhere(where, context.WHERE().Symbol);

            node.SetValues.AddRange(updateAssignment.Select(ExpressionVisitor.VisitUpdateAssignment));

            node.Target.Value = tableNode;

            return node;
        }

        public static QsiDataDeleteActionNode VisitDelete(DeleteContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiDataDeleteActionNode>(context);

            var tableNode = TrinoTree.CreateWithSpan<QsiDerivedTableNode>(context);
            tableNode.Source.Value = TableVisitor.VisitQualifiedName(context.qualifiedName());
            tableNode.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();

            if (context.HasToken(WHERE))
                tableNode.Where.Value = ExpressionVisitor.VisitWhere(context.booleanExpression(), context.WHERE().Symbol);

            node.Target.Value = tableNode;

            return node;
        }

        public static QsiChangeSearchPathActionNode VisitUse(UseContext context)
        {
            var path = context.catalog is not null
                ? new QsiQualifiedIdentifier(context.catalog.qi, context.schema.qi)
                : new QsiQualifiedIdentifier(context.schema.qi);

            var node = TrinoTree.CreateWithSpan<QsiChangeSearchPathActionNode>(context);

            node.Identifiers = new[] { path };

            return node;
        }

        private static QsiTableDirectivesNode CreateDirectiveTable(QsiIdentifier alias, QsiTableNode tableNode)
        {
            var directives = new QsiTableDirectivesNode();
            var node = new QsiDerivedTableNode();
            var aliasNode = new QsiAliasNode { Name = alias };

            node.Alias.Value = aliasNode;
            node.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();
            node.Source.Value = tableNode;

            directives.Tables.Add(node);

            return directives;
        }
    }
}

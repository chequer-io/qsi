using System.Linq;
using Qsi.Data;
using Qsi.Oracle.Common;
using Qsi.Oracle.Internal;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Oracle.Tree.Visitors
{
    using static OracleParserInternal;

    internal static class ActionVisitor
    {
        public static IQsiTreeNode VisitCreate(CreateContext context)
        {
            switch (context.children[0])
            {
                case CreateViewContext createViewContext:
                    return VisitCreateView(createViewContext);

                default:
                    throw TreeHelper.NotSupportedTree(context.children[0]);
            }
        }

        public static IQsiTreeNode VisitInsert(InsertContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleDataInsertActionNode>(context);

            if (context.hint() is not null)
                node.Hint = context.hint().GetInputText();

            var singleTableInsert = context.singleTableInsert();

            if (singleTableInsert is not null)
            {
                var insertIntoClause = singleTableInsert.insertIntoClause();
                var valuesClause = singleTableInsert.valuesClause();

                var targetNode = TableVisitor.VisitDmlTableExpressionClause(insertIntoClause.dmlTableExpressionClause());

                if (targetNode is not QsiTableReferenceNode referenceNode)
                    throw TreeHelper.NotSupportedFeature("Expression Target in Update");

                node.Target.Value = referenceNode;

                if (insertIntoClause.columnList() is not null)
                {
                    node.Columns = insertIntoClause
                        .columnList()
                        ._columns.Select(c => IdentifierVisitor.CreateQualifiedIdentifier(c))
                        .ToArray();
                }

                if (valuesClause is not null)
                {
                    var rowValueNode = OracleTree.CreateWithSpan<QsiRowValueExpressionNode>(valuesClause);

                    rowValueNode.ColumnValues.AddRange(valuesClause
                        .valuesClauseValue()
                        .Select(v =>
                            v.expr() is not null
                                ? ExpressionVisitor.VisitExpr(v.expr())
                                : TreeHelper.CreateDefaultLiteral())
                    );

                    node.Values.Add(rowValueNode);
                }
            }
            else
            {
                TreeHelper.NotSupportedFeature("Multi Table Insert");
            }

            return node;
        }

        public static IQsiTreeNode VisitDelete(DeleteContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleDataDeleteActionNode>(context);

            if (context.hint() is not null)
                node.Hint = context.hint().GetInputText();

            var targetNode = TableVisitor.VisitDmlTableExpressionClause(context.dmlTableExpressionClause());

            if (targetNode is not QsiTableReferenceNode referenceNode)
                throw TreeHelper.NotSupportedFeature("Expression Target in Update");

            if (context.whereClause() is not null)
            {
                var derivedTableNode = new OracleDerivedTableNode();

                derivedTableNode.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();
                derivedTableNode.Source.Value = referenceNode;
                derivedTableNode.Where.Value = ExpressionVisitor.VisitWhereClause(context.whereClause());

                node.Target.Value = derivedTableNode;
            }
            else
            {
                node.Target.Value = referenceNode;
            }

            if (node.Target.Value is IOracleTableNode oracleTableNode)
                oracleTableNode.IsOnly = context.HasToken(ONLY);

            // tAlias ignored
            // returningClause, errorLoggingClause ignored

            return node;
        }

        public static IQsiTreeNode VisitUpdate(UpdateContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleDataUpdateActionNode>(context);

            if (context.hint() is not null)
                node.Hint = context.hint().GetInputText();

            var targetNode = TableVisitor.VisitDmlTableExpressionClause(context.dmlTableExpressionClause());

            if (targetNode is not QsiTableReferenceNode referenceNode)
                throw TreeHelper.NotSupportedFeature("Expression Target in Update");

            if (context.whereClause() is not null)
            {
                var derivedTableNode = new OracleDerivedTableNode();

                derivedTableNode.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();
                derivedTableNode.Source.Value = referenceNode;
                derivedTableNode.Where.Value = ExpressionVisitor.VisitWhereClause(context.whereClause());

                node.Target.Value = derivedTableNode;
            }
            else
            {
                node.Target.Value = referenceNode;
            }

            if (node.Target.Value is IOracleTableNode oracleTableNode)
                oracleTableNode.IsOnly = context.HasToken(ONLY);

            node.SetValueExpressions.AddRange(ExpressionVisitor.VisitUpdateSetClause(context.updateSetClause()));

            // tAlias ignored
            // returningClause, errorLoggingClause ignored

            return node;
        }

        public static IQsiTreeNode VisitCreateView(CreateViewContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleViewDefinitionNode>(context);

            if (context.schema() is not null)
            {
                node.Identifier = IdentifierVisitor.CreateQualifiedIdentifier(
                    context.schema().identifier(),
                    context.view().identifier()
                );
            }
            else
            {
                node.Identifier = IdentifierVisitor.CreateQualifiedIdentifier(
                    context.view().identifier()
                );
            }

            node.Source.Value = TableVisitor.VisitSubquery(context.subquery());

            node.Replace = context.HasToken(REPLACE);
            node.Force = context.HasToken(FORCE) && !context.HasToken(NO);

            if (context.createViewEditionOption() is not null)
                node.EditionOption.Value = TreeHelper.Fragment(context.createViewEditionOption().GetInputText());

            if (context.createViewSharingOption() is not null)
                node.SharingOption.Value = TreeHelper.Fragment(context.createViewEditionOption().GetInputText());

            if (context.subqueryRestrictionClause() is not null)
                node.SubqueryRestriction.Value = TreeHelper.Fragment(context.subqueryRestrictionClause().GetInputText());

            if (context.createViewBequeathOption() is not null)
                node.Bequeath.Value = TreeHelper.Fragment(context.createViewEditionOption().GetInputText());

            if (context.containerOption is not null)
                node.ContainerOption.Value = TreeHelper.Fragment(context.containerOption.Text);

            if (context.defaultCollationOption() is not null)
                node.DefaultCollationName = context.defaultCollationOption().collationName().GetText();

            return node;
        }

        //
        // -- MERGE Statement
        //
        // MERGE INTO
        //     table1
        // USING
        //     (SELECT column1, column2, column3 FROM table2) table2_alias
        // ON
        //     (table1.name = table2_alias.column2)
        // WHEN NOT MATCHED THEN INSERT
        //     (name, age, birthday)
        // VALUES
        //     (table2_alias.column2, 23, table2_alias.column3);
        //
        // -- To SELECT Statement
        // 
        // WITH "SUBQUERY_table2_alias" AS (SELECT column1, column2, column3 FROM table2)
        // SELECT
        //     table2_alias.column2, 23, table2_alias.column3
        // FROM (
        //     SELECT
        //         *
        //     FROM
        //         "SUBQUERY_table2_alias" table2_alias
        //     MINUS
        //     SELECT
        //         table2_alias.*
        //     FROM
        //         table1
        //     JOIN
        //         "SUBQUERY_table2_alias" table2_alias
        //     ON (table1.name = table2_alias.column2)
        // ) table2_alias;
        public static QsiActionNode VisitMergeInsertClause(
            MergeContext context,
            MergeInsertClauseContext mergeInsertClause,
            QsiTableReferenceNode leftNode,
            QsiTableNode rightNode,
            QsiExpressionNode onCondition
        )
        {
            QsiTableNode rightSource;
            QsiQualifiedIdentifier rightPath;
            QsiTableDirectivesNode directives = null;

            var rightAlias = context.rightAlias is not null ?
                IdentifierVisitor.VisitIdentifier(context.rightAlias.identifier())
                : null;

            if (rightNode is QsiTableReferenceNode rightReferenceNode)
            {
                rightSource = rightReferenceNode;

                rightPath = rightAlias is null
                    ? rightReferenceNode.Identifier
                    : new QsiQualifiedIdentifier(rightAlias);
            }
            else
            {
                // subquery needs alias always
                if (rightAlias is null)
                    throw TreeHelper.NotSupportedFeature("Merge Insert without subquery alias");

                var subqueryAlias = new QsiIdentifier($"\"SUBQUERY_{rightAlias}\"", true);

                rightPath = new QsiQualifiedIdentifier(rightAlias);
                directives = CreateDirectiveTable(subqueryAlias, rightNode);

                var rightDerivedNode = new OracleDerivedTableNode();

                rightDerivedNode.Source.Value = new OracleTableReferenceNode
                {
                    Identifier = new QsiQualifiedIdentifier(subqueryAlias)
                };

                rightDerivedNode.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();

                rightDerivedNode.Alias.Value = new QsiAliasNode
                {
                    Name = rightAlias
                };

                rightSource = rightDerivedNode;
            }

            var matchedItemsNode = new OracleJoinedTableNode();
            matchedItemsNode.Left.Value = leftNode;
            matchedItemsNode.Right.Value = rightSource;
            matchedItemsNode.JoinType = "JOIN";
            matchedItemsNode.OnCondition.Value = onCondition;

            var selectInJoinedTableNode = new OracleDerivedTableNode();
            selectInJoinedTableNode.Source.Value = matchedItemsNode;

            var columns = new QsiColumnsDeclarationNode();

            var allNode = new QsiAllColumnNode
            {
                Path = rightPath
            };

            columns.Columns.Add(allNode);

            selectInJoinedTableNode.Columns.Value = columns;

            var minusJoinTableLeftSource = new OracleDerivedTableNode();

            minusJoinTableLeftSource.Source.Value = rightSource;
            minusJoinTableLeftSource.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();

            var minusJoinTable = new OracleBinaryTableNode();
            minusJoinTable.Left.Value = minusJoinTableLeftSource;
            minusJoinTable.Right.Value = selectInJoinedTableNode;
            minusJoinTable.BinaryTableType = OracleBinaryTableType.Minus;

            var minusJoinAliasedTable = new OracleDerivedTableNode();
            minusJoinAliasedTable.Source.Value = minusJoinTable;
            minusJoinAliasedTable.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();

            minusJoinAliasedTable.Alias.Value = new QsiAliasNode
            {
                Name = rightAlias
            };

            var itemSelectionSubqueryTable = new OracleDerivedTableNode();

            if (directives is not null)
                itemSelectionSubqueryTable.Directives.Value = directives;

            itemSelectionSubqueryTable.Source.Value = minusJoinAliasedTable;

            var columnsDeclaration = new QsiColumnsDeclarationNode();

            foreach (var mergeValue in mergeInsertClause.mergeValue())
            {
                QsiColumnNode columnNode;

                if (mergeValue.HasToken(DEFAULT))
                {
                    QsiDerivedColumnNode derivedColumnNode = new();
                    derivedColumnNode.Expression.Value = TreeHelper.CreateDefaultLiteral();

                    columnNode = derivedColumnNode;
                }
                else
                {
                    var exprNode = ExpressionVisitor.VisitExpr(mergeValue.expr());

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
                }

                columnsDeclaration.Columns.Add(columnNode);
            }

            itemSelectionSubqueryTable.Columns.Value = columnsDeclaration;

            // MERGE INSERT
            var deleteActionNode = new OracleDataInsertActionNode();
            deleteActionNode.ValueTable.Value = itemSelectionSubqueryTable;
            deleteActionNode.Target.Value = leftNode;

            deleteActionNode.Columns = mergeInsertClause
                .column()
                .Select(c => IdentifierVisitor.CreateQualifiedIdentifier(c.identifier()))
                .ToArray();

            return deleteActionNode;
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

        public static OracleMergeActionNode VisitMerge(MergeContext context)
        {
            var targetTable = TableVisitor.VisitTableReference(context.leftTable);
            QsiTableNode leftTable = targetTable;

            var onCondition = ExpressionVisitor.VisitCondition(context.condition());

            if (context.leftAlias is not null)
            {
                var derivedTable = OracleTree.CreateWithSpan<QsiDerivedTableNode>(context.leftTable.Start, context.leftAlias.Stop);
                derivedTable.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();
                derivedTable.Alias.Value = IdentifierVisitor.VisitAlias(context.leftAlias);

                leftTable = derivedTable;
            }

            var sourceTable = context.rightReference is not null
                ? TableVisitor.VisitTableReference(context.rightReference)
                : TableVisitor.VisitSubquery(context.rightSubquery);

            var rightTable = sourceTable;

            if (context.rightAlias is not null)
            {
                var rightStart = context.rightReference is not null ? context.rightReference.Start : context.OPEN_PAR_SYMBOL(0).Symbol;
                var derivedTable = OracleTree.CreateWithSpan<QsiDerivedTableNode>(rightStart, context.rightAlias.Stop);
                derivedTable.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();
                derivedTable.Alias.Value = IdentifierVisitor.VisitAlias(context.rightAlias);

                rightTable = derivedTable;
            }

            var joinedTable = TreeHelper.Create<OracleJoinedTableNode>(n =>
            {
                n.Left.SetValue(leftTable);
                n.Right.SetValue(rightTable);
                n.JoinType = "JOIN";
                n.OnCondition.Value = onCondition;
            });

            var node = OracleTree.CreateWithSpan<OracleMergeActionNode>(context);

            if (context.hint() is not null)

                node.Hint = context.hint().GetInputText();

            var mergeUpdateClause = context.mergeUpdateClause();

            var mergeInsertClause = context.mergeInsertClause();

            if (mergeUpdateClause is not null)
            {
                QsiTableNode updateTarget = joinedTable;

                if (mergeUpdateClause.where is not null)
                {
                    var derivedTable = new OracleDerivedTableNode();
                    derivedTable.Source.Value = updateTarget;
                    derivedTable.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();
                    derivedTable.Where.Value = ExpressionVisitor.VisitWhereClause(mergeUpdateClause.where);

                    updateTarget = derivedTable;
                }

                // MERGE DELETE
                if (mergeUpdateClause.HasToken(DELETE))
                {
                    var deleteTarget = new OracleDerivedTableNode();
                    deleteTarget.Source.Value = joinedTable;
                    deleteTarget.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();
                    deleteTarget.Where.Value = ExpressionVisitor.VisitWhereClause(mergeUpdateClause.deleteWhere);

                    var deleteActionNode = new OracleDataDeleteActionNode();
                    deleteActionNode.Target.Value = deleteTarget;

                    node.ActionNodes.Add(deleteActionNode);
                }

                // MERGE UPDATE
                var updateActionNode = new OracleDataUpdateActionNode();
                updateActionNode.Target.Value = updateTarget;

                foreach (var setValue in mergeUpdateClause.mergeSetValue())
                {
                    var setValueNode = OracleTree.CreateWithSpan<QsiSetColumnExpressionNode>(setValue);
                    setValueNode.Target = IdentifierVisitor.CreateQualifiedIdentifier(setValue.column().identifier());

                    var setValueExprNode = new OracleSetValueExpressionNode();
                    setValueExprNode.SetValue.Value = setValueNode;

                    updateActionNode.SetValueExpressions.Add(setValueExprNode);
                }

                node.ActionNodes.Add(updateActionNode);
            }

            if (mergeInsertClause is not null)
            {
                node.ActionNodes.Add(VisitMergeInsertClause(context, mergeInsertClause, targetTable, sourceTable, onCondition));
            }

            return node;
        }
    }
}

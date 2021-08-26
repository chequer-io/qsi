using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Qsi.Oracle.Common;
using Qsi.Oracle.Internal;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Oracle.Tree.Visitors
{
    using static OracleParserInternal;

    internal static class TableVisitor
    {
        public static QsiTableNode VisitSelect(SelectContext context)
        {
            var node = VisitSubquery(context.subquery());

            // forUpdateClause ignored

            return node;
        }

        public static QsiTableNode VisitSubquery(SubqueryContext context)
        {
            return context switch
            {
                QueryBlockSubqueryContext queryBlocksubquery => VisitQueryBlockSubquery(queryBlocksubquery),
                CompositeSubqueryContext joinedSubquery => VisitCompositeSubquery(joinedSubquery),
                ParenthesisSubqueryContext parenthesisSubquery => VisitParenthesisSubquery(parenthesisSubquery),
                _ => throw new NotSupportedException()
            };
        }

        public static QsiTableNode VisitQueryBlockSubquery(QueryBlockSubqueryContext context)
        {
            var node = VisitQueryBlock(context.queryBlock());

            var orderByClause = context.orderByClause();
            var rowOffset = context.rowOffset();
            var rowFetchOption = context.rowFetchOption();

            if (orderByClause is not null)
                node.Order.Value = ExpressionVisitor.VisitOrderByClause(orderByClause);

            if (rowOffset is not null || rowFetchOption is not null)
                node.Limit.Value = ExpressionVisitor.VisitRowlimitingContexts(rowOffset, rowFetchOption);

            return node;
        }

        public static QsiTableNode VisitCompositeSubquery(CompositeSubqueryContext context)
        {
            SubqueryContext[] subqueryItems = context.subquery();
            var source = VisitSubquery(subqueryItems[0]);

            for (int i = 1; i < subqueryItems.Length; i++)
            {
                var leftContext = subqueryItems[i - 1];
                var rightContext = subqueryItems[i];

                var joinedTable = OracleTree.CreateWithSpan<OracleBinaryTableNode>(leftContext.Start, rightContext.Stop);

                joinedTable.Left.Value = source;
                joinedTable.BinaryTableType = VisitSubqueryCompositeType(context.subqueryCompositeType(i - 1));
                joinedTable.Right.Value = VisitSubquery(rightContext);

                source = joinedTable;
            }

            if (source is OracleBinaryTableNode binaryTableNode)
            {
                var orderByClause = context.orderByClause();
                var rowOffset = context.rowOffset();
                var rowFetchOption = context.rowFetchOption();

                if (orderByClause is not null)
                    binaryTableNode.Order.Value = ExpressionVisitor.VisitOrderByClause(orderByClause);

                if (rowOffset is not null || rowFetchOption is not null)
                    binaryTableNode.Limit.Value = ExpressionVisitor.VisitRowlimitingContexts(rowOffset, rowFetchOption);

                source = ExtractDirectivesToBinaryTableNode(binaryTableNode);
            }

            return source;
        }

        // ┌──────────────────────────────────────────────────────────────────────────────────────────────────────────┐
        // │             ┌ Derived (Directives: 2)   ┌ WITH A AS (SELECT * FROM table1), B AS (SELECT * FROM table2)  │
        // │ BinaryTable │                           └ SELECT * FROM A                                                │
        // │             │ UNION ALL                                                                                  │
        // │             └ Derived (Directive: 0)    ─ SELECT * FROM B                                                │
        // └──────────────────────────────────────────────────────────────────────────────────────────────────────────┘
        //                                   ▼                ▼                ▼
        //            ┌───────────────────────────────────────────────────────────────────────────────────┐
        //            │                 ┌  WITH A AS (SELECT * FROM table1), B AS (SELECT * FROM table2)  │
        //            │ Derived         │              ┌ Derived    ─ SELECT * FROM A                     │     
        //            │ (Directives: 2) │  BinaryTable │ UNION ALL                                        │ 
        //            │                 └              └ Derived    ─ SELECT * FROM B                     │             
        //            └───────────────────────────────────────────────────────────────────────────────────┘
        public static QsiTableNode ExtractDirectivesToBinaryTableNode(OracleBinaryTableNode node)
        {
            IEnumerable<QsiDerivedTableNode> leftItems = null;
            IEnumerable<QsiDerivedTableNode> rightItems = null;

            if (node.Left.Value is QsiDerivedTableNode left)
            {
                if (!left.Directives.IsEmpty)
                {
                    leftItems = left.Directives.Value.Tables.ToArray();
                    left.Directives.Value.Tables.Clear();
                }
            }

            if (node.Right.Value is QsiDerivedTableNode right)
            {
                if (!right.Directives.IsEmpty)
                {
                    rightItems = right.Directives.Value.Tables.ToArray();
                    right.Directives.Value.Tables.Clear();
                }
            }

            if (leftItems is null && rightItems is null)
                return node;

            var directivesNode = new QsiTableDirectivesNode();

            if (leftItems is not null)
            {
                foreach (var item in leftItems)
                    directivesNode.Tables.Add(item);
            }

            if (rightItems is not null)
            {
                foreach (var item in rightItems)
                    directivesNode.Tables.Add(item);
            }

            var derivedTableNode = new QsiDerivedTableNode();
            derivedTableNode.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();
            derivedTableNode.Source.Value = node;
            derivedTableNode.Directives.Value = directivesNode;

            return derivedTableNode;
        }

        public static OracleBinaryTableType VisitSubqueryCompositeType(SubqueryCompositeTypeContext context)
        {
            switch (context.children[0])
            {
                case ITerminalNode { Symbol: { Type: UNION } }:
                    return context.ALL() != null
                        ? OracleBinaryTableType.UnionAll
                        : OracleBinaryTableType.Union;

                case ITerminalNode { Symbol: { Type: INTERSECT } }:
                    return OracleBinaryTableType.Intersect;

                case ITerminalNode { Symbol: { Type: MINUS } }:
                    return OracleBinaryTableType.Minus;

                case ITerminalNode { Symbol: { Type: EXCEPT } }:
                    return OracleBinaryTableType.Except;

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
        }

        public static QsiTableNode VisitParenthesisSubquery(ParenthesisSubqueryContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleDerivedTableNode>(context);

            node.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();
            node.Source.Value = VisitSubquery(context.subquery());

            var orderByClause = context.orderByClause();
            var rowOffset = context.rowOffset();
            var rowFetchOption = context.rowFetchOption();

            if (orderByClause is not null)
                node.Order.Value = ExpressionVisitor.VisitOrderByClause(orderByClause);

            if (rowOffset is not null)
                node.Limit.Value = ExpressionVisitor.VisitRowlimitingContexts(rowOffset, rowFetchOption);

            return node;
        }

        public static OracleDerivedTableNode VisitQueryBlock(QueryBlockContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleDerivedTableNode>(context);
            var withClause = context.withClause();

            if (withClause is not null)
                node.Directives.Value = VisitWithClause(withClause);

            if (context.hint() is not null)
                node.Hint = VisitHint(context.hint());

            if (context.queryBehavior() is not null)
                node.QueryBehavior = VisitQueryBehavior(context.queryBehavior());

            node.Columns.Value = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.selectList());

            foreach (var selectItem in ExpressionVisitor.VisitSelectList(context.selectList()))
                node.Columns.Value.Columns.Add(selectItem);

            if (context._tables.Count == 1)
            {
                node.Source.Value = VisitTableSource(context._tables[0]);
            }
            else
            {
                var source = VisitTableSource(context._tables[0]);

                for (int i = 1; i < context._tables.Count; i++)
                {
                    var leftContext = context._tables[i - 1];
                    var rightContext = context._tables[i];

                    var joinedTable = OracleTree.CreateWithSpan<QsiJoinedTableNode>(leftContext.Start, rightContext.Stop);

                    joinedTable.IsComma = true;
                    joinedTable.Left.Value = source;
                    joinedTable.Right.Value = VisitTableSource(rightContext);

                    source = joinedTable;
                }

                node.Source.Value = source;
            }

            var whereClause = context.whereClause();

            if (whereClause is not null)
                node.Where.Value = ExpressionVisitor.VisitWhereClause(whereClause);

            var groupByClause = context.groupByClause();

            if (groupByClause is not null)
                node.Grouping.Value = ExpressionVisitor.VisitGroupByClause(groupByClause);

            var windowClause = context.windowClause();

            if (windowClause is not null)
                node.Window.Value = ExpressionVisitor.VisitWindowClause(windowClause);
            
            // hierarchicalQueryClause, modelClause, windowClause ignored

            return node;
        }

        public static string VisitHint(HintContext context)
        {
            return context.GetInputText();
        }

        public static OracleQueryBehavior VisitQueryBehavior(QueryBehaviorContext context)
        {
            switch (context.children[0])
            {
                case ITerminalNode { Symbol: { Type: DISTINCT } }:
                    return OracleQueryBehavior.Distinct;

                case ITerminalNode { Symbol: { Type: UNIQUE } }:
                    return OracleQueryBehavior.Unique;

                case ITerminalNode { Symbol: { Type: ALL } }:
                    return OracleQueryBehavior.All;

                default:
                    throw TreeHelper.NotSupportedTree(context.children[0]);
            }
        }

        public static QsiTableNode VisitTableSource(TableSourceContext context)
        {
            while (context is ParenthesisTableSourceContext parenthesisSource)
                context = parenthesisSource.tableSource();

            return context switch
            {
                TableOrJoinTableSourceContext tableOrJoinTableSource => VisitTableOrJoinTableSource(tableOrJoinTableSource),
                InlineAnalyticViewTableSourceContext => throw TreeHelper.NotSupportedFeature("Analytic View"),
                _ => throw new InvalidOperationException()
            };
        }

        public static QsiTableNode VisitTableOrJoinTableSource(TableOrJoinTableSourceContext context)
        {
            var node = VisitTableReference(context.tableReference());

            for (int i = 0; i < context.ChildCount - 1; i++)
            {
                var leftToken = i == 0
                    ? context.tableReference().Start
                    : context.tableJoinClause(i - 1).Start;

                var joinedNode = VisitTableJoinClause(node, leftToken, context.tableJoinClause(i));
                joinedNode.Left.Value = node;
                node = joinedNode;
            }

            return node;
        }

        public static OracleJoinedTableNode VisitTableJoinClause(QsiTableNode leftNode, IToken leftToken, TableJoinClauseContext context)
        {
            return context.children[0] switch
            {
                InnerCrossJoinClauseContext innerCrossJoin => VisitInnerCrossJoinClause(leftToken, innerCrossJoin),
                OuterJoinClauseContext outerJoin => VisitOuterJoinClause(leftNode, leftToken, outerJoin),
                CrossOuterApplyClauseContext crossOuterJoin => VisitCrossOuterApplyClause(leftToken, crossOuterJoin),
                _ => throw new InvalidOperationException()
            };
        }

        public static OracleJoinedTableNode VisitInnerCrossJoinClause(IToken leftToken, InnerCrossJoinClauseContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleJoinedTableNode>(leftToken, context.Stop);

            switch (context)
            {
                case InnerJoinClauseContext innerJoinClause:
                    node.JoinType = innerJoinClause.HasToken(INNER) ? "INNER JOIN" : "JOIN";
                    node.Right.Value = VisitTableReference(innerJoinClause.tableReference());

                    // USING (column..)
                    if (innerJoinClause.HasToken(USING))
                    {
                        var pivotColumnsNode = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(innerJoinClause.USING().Symbol, innerJoinClause.CLOSE_PAR_SYMBOL().Symbol);
                        pivotColumnsNode.Columns.AddRange(innerJoinClause.column().Select(IdentifierVisitor.VisitColumn));
                        node.PivotColumns.Value = pivotColumnsNode;
                    }
                    // ON condition
                    else
                    {
                        node.OnCondition.Value = ExpressionVisitor.VisitCondition(innerJoinClause.condition());
                    }

                    break;

                case CrossOrNatrualInnerJoinClauseContext crossOrNatrualInnerJoinClause:
                    if (crossOrNatrualInnerJoinClause.HasToken(CROSS))
                    {
                        node.JoinType = "CROSS JOIN";
                    }
                    else
                    {
                        node.JoinType = crossOrNatrualInnerJoinClause.HasToken(INNER) ? "NATURAL INNER JOIN" : "NATURAL JOIN";
                        node.IsNatural = true;
                    }

                    node.Right.Value = VisitTableReference(crossOrNatrualInnerJoinClause.tableReference());
                    break;

                default:
                    throw new InvalidOperationException();
            }

            return node;
        }

        public static OracleJoinedTableNode VisitOuterJoinClause(QsiTableNode leftNode, IToken leftToken, OuterJoinClauseContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleJoinedTableNode>(leftToken, context.Stop);

            SetTableNodePartition(leftNode, context.leftQpc);

            if (context.HasToken(NATURAL))
                node.IsNatural = true;

            node.JoinType = $"{(node.IsNatural ? "NATURAL" : string.Empty)} {string.Join(" ", context.outerJoinType().children.Select(c => c.GetText()))} JOIN";

            var rightNode = VisitTableReference(context.tableReference());
            SetTableNodePartition(rightNode, context.rightQpc);

            node.Right.Value = rightNode;

            // USING (column..)
            if (context.HasToken(USING))
            {
                var pivotColumnsNode = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.USING().Symbol, context.CLOSE_PAR_SYMBOL().Symbol);
                pivotColumnsNode.Columns.AddRange(context.column().Select(IdentifierVisitor.VisitColumn));
                node.PivotColumns.Value = pivotColumnsNode;
            }
            // ON condition
            else if (context.HasToken(ON))
            {
                node.OnCondition.Value = ExpressionVisitor.VisitCondition(context.condition());
            }

            return node;

            static void SetTableNodePartition(QsiTableNode tableNode, QueryPartitionClauseContext queryPartitionClause)
            {
                if (queryPartitionClause == null)
                    return;

                var partitionNode = ExpressionVisitor.VisitQueryPartitionClause(queryPartitionClause);

                if (tableNode is not IOracleTableNode partitionTableNode)
                    throw new QsiException(QsiError.SyntaxError, "Invalid query partition clause");

                partitionTableNode.Partition.Value = partitionNode;
            }
        }

        public static OracleJoinedTableNode VisitCrossOuterApplyClause(IToken leftToken, CrossOuterApplyClauseContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleJoinedTableNode>(leftToken, context.Stop);

            node.JoinType = context.HasToken(CROSS) ? "CROSS APPLY" : "OUTER APPLY";

            node.Right.Value = context.tableReference() is not null
                ? VisitTableReference(context.tableReference())
                : VisitCollectionExpression(context.collectionExpression());

            return node;
        }

        public static QsiTableNode VisitTableReference(TableReferenceContext context)
        {
            return context switch
            {
                QueryTableReferenceContext queryTableReference => VisitQueryTableReference(queryTableReference),
                _ => throw new NotImplementedException()
            };
        }

        public static QsiTableReferenceNode VisitTableReference(FullObjectPathContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleTableReferenceNode>(context);
            node.Identifier = IdentifierVisitor.VisitFullObjectPath(context);

            return node;
        }

        public static QsiTableNode VisitQueryTableReference(QueryTableReferenceContext context)
        {
            var node = VisitQueryTableExpression(context.queryTableExpression());

            if (context.flashbackQueryClause() is not null)
            {
                if (node is not OracleDerivedTableNode derivedTableNode)
                {
                    derivedTableNode = OracleTree.CreateWithSpan<OracleDerivedTableNode>(context);
                    derivedTableNode.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();
                    derivedTableNode.Source.Value = node;
                }

                derivedTableNode.FlashbackQueryClause.Value = TreeHelper.Fragment(context.flashbackQueryClause().GetInputText());
                node = derivedTableNode;
            }

            if (context.pivotClause() is not null)
            {
                if (node is not OracleDerivedTableNode derivedTableNode)
                {
                    derivedTableNode = OracleTree.CreateWithSpan<OracleDerivedTableNode>(context);
                    derivedTableNode.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();
                    derivedTableNode.Source.Value = node;
                }

                derivedTableNode.TableClauses.Value = TreeHelper.Fragment(context.pivotClause().GetInputText());
                node = derivedTableNode;
            }
            else if (context.unpivotClause() is not null)
            {
                if (node is not OracleDerivedTableNode derivedTableNode)
                {
                    derivedTableNode = OracleTree.CreateWithSpan<OracleDerivedTableNode>(context);
                    derivedTableNode.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();
                    derivedTableNode.Source.Value = node;
                }

                derivedTableNode.TableClauses.Value = TreeHelper.Fragment(context.unpivotClause().GetInputText());
                node = derivedTableNode;
            }
            else if (context.rowPatternClause() is not null)
            {
                if (node is not OracleDerivedTableNode derivedTableNode)
                {
                    derivedTableNode = OracleTree.CreateWithSpan<OracleDerivedTableNode>(context);
                    derivedTableNode.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();
                    derivedTableNode.Source.Value = node;
                }

                derivedTableNode.TableClauses.Value = TreeHelper.Fragment(context.rowPatternClause().GetInputText());
                node = derivedTableNode;
            }

            if (context.tAlias() is not null)
            {
                if (node is not OracleDerivedTableNode derivedTableNode)
                {
                    derivedTableNode = OracleTree.CreateWithSpan<OracleDerivedTableNode>(context);
                    derivedTableNode.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();
                    derivedTableNode.Source.Value = node;
                    node = derivedTableNode;
                }

                derivedTableNode.Alias.Value = new QsiAliasNode
                {
                    Name = IdentifierVisitor.VisitIdentifier(context.tAlias().identifier())
                };
            }

            if (node is IOracleTableNode oracleNode)
                oracleNode.IsOnly = context.HasToken(ONLY);

            return node;
        }

        public static QsiTableNode VisitQueryTableExpression(QueryTableExpressionContext context)
        {
            return context switch
            {
                ObjectPathTableExpressionContext objectPathTableExpression => VisitObjectPathTableExpression(objectPathTableExpression),
                SubqueryTableExpressionContext subqueryTableExpression => VisitSubqueryTableExpression(subqueryTableExpression),
                QueryTableCollectionExpressionContext queryTableExpressionContext => VisitTableCollectionExpression(queryTableExpressionContext.tableCollectionExpression()),
                FunctionTableExpressionContext _ => throw TreeHelper.NotSupportedFeature("Table function"),
                _ => throw new NotSupportedException()
            };
        }

        public static QsiTableNode VisitObjectPathTableExpression(ObjectPathTableExpressionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleTableReferenceNode>(context);

            node.Identifier = IdentifierVisitor.VisitFullObjectPath(context.fullObjectPath());

            if (context.partitionExtensionClause() is not null)
                node.Partition.Value = ExpressionVisitor.VisitPartitionExtensionClause(context.partitionExtensionClause());

            if (context.hierarchiesClause() is not null)
                node.Hierarchies.Value = ExpressionVisitor.VisitHierarchiesClause(context.hierarchiesClause());

            if (context.modifiedExternalTable() is not null)
                throw TreeHelper.NotSupportedFeature("External table");

            // sampleClause ignored

            return node;
        }

        public static QsiTableNode VisitSubqueryTableExpression(SubqueryTableExpressionContext context)
        {
            QsiTableNode node;

            if (context.HasToken(LATERAL))
            {
                var lateralNode = OracleTree.CreateWithSpan<OracleLateralTableNode>(context);
                lateralNode.Source.Value = VisitSubquery(context.subquery());
                node = lateralNode;
            }
            else
            {
                var derivedTable = OracleTree.CreateWithSpan<OracleDerivedTableNode>(context);
                derivedTable.Source.Value = VisitSubquery(context.subquery());
                derivedTable.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();
                node = derivedTable;
            }

            // subqueryRestrictionClause ignored

            return node;
        }

        public static QsiTableNode VisitDmlTableExpressionClause(DmlTableExpressionClauseContext context)
        {
            switch (context)
            {
                case DmlGeneralTableExpressionClauseContext context1:
                {
                    var node = new OracleTableReferenceNode
                    {
                        Identifier = context1.schema() is not null
                            ? IdentifierVisitor.CreateQualifiedIdentifier(context1.schema().identifier(), context1.table().identifier())
                            : IdentifierVisitor.CreateQualifiedIdentifier(context1.table().identifier())
                    };

                    if (context1.partitionExtensionClause() is not null)
                        node.Partition.Value = ExpressionVisitor.VisitPartitionExtensionClause(context1.partitionExtensionClause());

                    return node;
                }

                case DmlSubqueryExpressionClauseContext context2:
                {
                    var node = OracleTree.CreateWithSpan<OracleDerivedTableNode>(context2);

                    node.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();
                    node.Source.Value = VisitSubquery(context2.subquery());

                    // subqueryRestrictionClause ignored

                    return node;
                }

                case DmlTableCollectionExpressionClauseContext context3:
                    return VisitTableCollectionExpression(context3.tableCollectionExpression());

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
        }

        public static QsiTableNode VisitTableCollectionExpression(TableCollectionExpressionContext context)
        {
            throw TreeHelper.NotSupportedFeature("Nested table");
        }

        public static QsiTableNode VisitCollectionExpression(CollectionExpressionContext context)
        {
            throw TreeHelper.NotSupportedFeature("Nested table");
        }

        public static QsiTableDirectivesNode VisitWithClause(WithClauseContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiTableDirectivesNode>(context);
            node.Tables.AddRange(context._clauses.Select(VisitFactoringClause));
            return node;
        }

        public static OracleDerivedTableNode VisitFactoringClause(FactoringClauseContext context)
        {
            switch (context.children[0])
            {
                case SubqueryFactoringClauseContext subqueryFactoringClause:
                    var node = OracleTree.CreateWithSpan<OracleDerivedTableNode>(context);

                    node.Alias.Value = new QsiAliasNode
                    {
                        Name = IdentifierVisitor.VisitIdentifier(subqueryFactoringClause.identifier())
                    };

                    var columnList = subqueryFactoringClause.columnList();

                    if (columnList is not null)
                    {
                        node.Columns.Value = new QsiColumnsDeclarationNode();
                        node.Columns.Value.Columns.AddRange(IdentifierVisitor.VisitColumnList(columnList));
                    }
                    else
                    {
                        node.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();
                    }

                    node.Source.Value = VisitSubquery(subqueryFactoringClause.subquery());

                    // searchClause, cycleClause ignored
                    return node;

                case SubavFactoringClauseContext:
                    throw TreeHelper.NotSupportedFeature("Analytic View");
            }

            throw new NotSupportedException();
        }
    }
}

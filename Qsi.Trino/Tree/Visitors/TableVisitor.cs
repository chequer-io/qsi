using System;
using System.Linq;
using Antlr4.Runtime;
using Qsi.Data;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Trino.Internal;
using Qsi.Utilities;

namespace Qsi.Trino.Tree.Visitors
{
    using static SqlBaseParser;

    internal static class TableVisitor
    {
        public static QsiTableNode VisitQuery(QueryContext context)
        {
            var node = VisitQueryNoWith(context.queryNoWith());
            var with = context.with();

            if (with is null)
                return node;

            if (node is not QsiDerivedTableNode derivedTable)
            {
                derivedTable = new QsiDerivedTableNode();
                derivedTable.Source.Value = node;
                derivedTable.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();

                node = derivedTable;
            }
            else
            {
                TrinoTree.PutContextSpan(node, context);
            }

            derivedTable.Directives.Value = VisitWithClause(with);

            return node;
        }

        public static QsiTableNode VisitQueryNoWith(QueryNoWithContext context)
        {
            var table = VisitQueryTerm(context.queryTerm());

            if (table is not QsiDerivedTableNode node)
            {
                var derivedTableNode = TrinoTree.CreateWithSpan<QsiDerivedTableNode>(context);
                derivedTableNode.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();
                derivedTableNode.Source.Value = table;

                node = derivedTableNode;
            }
            else
            {
                TrinoTree.PutContextSpan(node, context);
            }

            // ORDER BY <sortItem> ..
            if (context.HasToken(ORDER))
            {
                SortItemContext[] sortItems = context.sortItem();

                var orderNode = TrinoTree.CreateWithSpan<QsiMultipleOrderExpressionNode>(
                    context.ORDER().Symbol,
                    sortItems[^1].Stop
                );

                orderNode.Orders.AddRange(sortItems.Select(ExpressionVisitor.VisitSortItem));

                node.Order.Value = orderNode;
            }

            IToken limitStartToken = null;
            IToken limitEndToken = null;
            QsiLimitExpressionNode limitNode = null;

            // OFFSET <rowCount> ROW
            if (context.HasToken(OFFSET))
            {
                limitNode = new QsiLimitExpressionNode();

                limitStartToken = context.OFFSET().Symbol;
                limitEndToken = context.offsetRow ?? context.offset.Stop;

                limitNode.Offset.Value = ExpressionVisitor.VisitRowCount(context.offset);
            }

            if (context.HasToken(LIMIT) && !context.limit.HasToken(ALL))
            {
                limitNode ??= new QsiLimitExpressionNode();

                limitStartToken ??= context.LIMIT().Symbol;
                limitEndToken = context.limit.Stop;

                limitNode.Limit.Value = ExpressionVisitor.VisitRowCount(context.limit.rowCount());
            }

            if (context.HasToken(FETCH))
            {
                limitNode ??= new QsiLimitExpressionNode();
                limitStartToken ??= context.FETCH().Symbol;

                if (context.HasToken(ONLY))
                    limitEndToken = context.ONLY().Symbol;
                else if (context.HasToken(WITH))
                    limitEndToken = context.TIES().Symbol;
                else
                    limitEndToken = context.fetchFirstRow;

                limitNode.Limit.Value = context.fetchFirst is null
                    ? TreeHelper.CreateLiteral(1)
                    : ExpressionVisitor.VisitRowCount(context.fetchFirst);
            }

            if (limitNode is not null)
            {
                node.Limit.Value = limitNode;
                TrinoTree.PutContextSpan(limitNode, limitStartToken, limitEndToken);
            }

            return node;
        }

        public static QsiTableNode VisitQueryTerm(QueryTermContext context)
        {
            switch (context)
            {
                case QueryTermDefaultContext queryTermDefault:
                    return VisitQueryPrimary(queryTermDefault.queryPrimary());

                case SetOperationContext setOperation:
                    throw new NotImplementedException();

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
        }

        public static QsiTableNode VisitQueryPrimary(QueryPrimaryContext context)
        {
            switch (context)
            {
                case QueryPrimaryDefaultContext queryPrimaryDefault:
                    return VisitQuerySpecification(queryPrimaryDefault.querySpecification());

                case TableContext table:
                    throw new NotImplementedException();

                case InlineTableContext inlineTable:
                    throw new NotImplementedException();

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
        }

        public static QsiDerivedTableNode VisitQuerySpecification(QuerySpecificationContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiDerivedTableNode>(context);

            // TODO: setQuantifier

            var columnsNode = new QsiColumnsDeclarationNode();
            columnsNode.Columns.AddRange(context.selectItem().Select(VisitSelectItem));
            node.Columns.Value = columnsNode;

            if (context.HasToken(FROM))
            {
                RelationContext[] relations = context.relation();

                var source = VisitRelation(relations[0]);

                if (relations.Length == 1)
                    node.Source.Value = source;
                else
                    throw new NotImplementedException("Comma Join");
            }

            return node;
        }

        public static QsiColumnNode VisitSelectItem(SelectItemContext context)
        {
            switch (context)
            {
                case SelectSingleContext selectSingle:
                    return VisitSelectSingle(selectSingle);

                case SelectAllContext selectAll:
                    return VisitSelectAll(selectAll);

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
        }

        public static QsiColumnNode VisitSelectSingle(SelectSingleContext context)
        {
            var node = ExpressionVisitor.VisitExpression(context.expression());
            QsiColumnNode columnNode;

            if (node is QsiColumnExpressionNode columnExpressionNode)
                columnNode = columnExpressionNode.Column.Value;
            else
            {
                var derivedColumnNode = new QsiDerivedColumnNode();
                derivedColumnNode.Expression.Value = node;
                columnNode = derivedColumnNode;
            }

            if (context.TryGetRuleContext<IdentifierContext>(out var identifierContext))
            {
                if (columnNode is not QsiDerivedColumnNode derivedColumnNode)
                {
                    derivedColumnNode = new QsiDerivedColumnNode();
                    derivedColumnNode.Column.Value = columnNode;
                    columnNode = derivedColumnNode;
                }

                derivedColumnNode.Alias.Value = new QsiAliasNode { Name = identifierContext.qi };
            }

            return columnNode;
        }

        public static QsiColumnNode VisitSelectAll(SelectAllContext context)
        {
            if (context.primaryExpression() is null)
                return new QsiAllColumnNode();

            throw new NotImplementedException("Select All with Primary expression");
        }

        public static QsiTableNode VisitRelation(RelationContext context)
        {
            switch (context)
            {
                case JoinRelationContext joinRelation:
                    return VisitJoinRelation(joinRelation);

                case RelationDefaultContext relationDefault:
                    return VisitSampledRelation(relationDefault.sampledRelation());

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
        }

        public static QsiTableNode VisitJoinRelation(JoinRelationContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiJoinedTableNode>(context);
            node.Left.Value = VisitRelation(context.left);

            if (context.HasToken(CROSS))
            {
                node.JoinType = "CROSS JOIN";
                node.Right.Value = VisitSampledRelation(context.right);
            }
            else if (context.HasToken(NATURAL))
            {
                node.IsNatural = true;
                node.JoinType = $"NATURAL {string.Join(" ", context.joinType().children.Select(c => c.GetText()))} JOIN";
                node.Right.Value = VisitSampledRelation(context.right);
            }
            else
            {
                node.JoinType = $"{string.Join(" ", context.joinType().children.Select(c => c.GetText()))} JOIN";
                node.Right.Value = VisitRelation(context.rightRelation);
            }

            return node;
        }

        public static QsiTableNode VisitSampledRelation(SampledRelationContext context)
        {
            if (context.HasToken(TABLESAMPLE))
                throw new NotImplementedException();

            return VisitPatternRecognition(context.patternRecognition());
        }

        public static QsiTableNode VisitPatternRecognition(PatternRecognitionContext context)
        {
            if (context.HasToken(MATCH_RECOGNIZE))
                throw new NotImplementedException();

            return VisitAliasedRelation(context.aliasedRelation());
        }

        public static QsiTableNode VisitAliasedRelation(AliasedRelationContext context)
        {
            var node = VisitRelationPrimary(context.relationPrimary());

            if (!context.TryGetRuleContext<IdentifierContext>(out var identifierContext))
                return node;

            if (node is not QsiDerivedTableNode derivedTableNode)
            {
                derivedTableNode = new QsiDerivedTableNode();
                derivedTableNode.Source.Value = node;
                derivedTableNode.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();
            }
            else
            {
                TrinoTree.PutContextSpan(derivedTableNode, context);
            }

            TreeHelper.CreateAliasedTableNode(derivedTableNode, identifierContext.qi);

            if (!context.TryGetRuleContext<ColumnAliasesContext>(out var columnAliasesContext))
                return derivedTableNode;

            var columnAliases = TrinoTree.CreateWithSpan<QsiColumnsDeclarationNode>(columnAliasesContext);

            foreach (var columnAlias in columnAliasesContext.identifier())
            {
                var sequentialNode = TrinoTree.CreateWithSpan<QsiSequentialColumnNode>(columnAlias);
                sequentialNode.ColumnType = QsiSequentialColumnType.Overwrite;
                sequentialNode.Alias.Value = new QsiAliasNode { Name = columnAlias.qi };

                columnAliases.Columns.Add(sequentialNode);
            }

            derivedTableNode.Columns.Value = columnAliases;
            return derivedTableNode;
        }

        public static QsiTableNode VisitRelationPrimary(RelationPrimaryContext context)
        {
            switch (context)
            {
                case TableNameContext tableName:
                    return VisitQualifiedName(tableName.qualifiedName());

                case SubqueryRelationContext subqueryRelation:
                    throw new NotImplementedException();

                case UnnestContext unnest:
                    throw new NotImplementedException();

                case LateralContext lateral:
                    throw new NotImplementedException();

                case ParenthesizedRelationContext parenthesizedRelation:
                    return VisitRelation(parenthesizedRelation.relation());

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
        }

        public static QsiTableDirectivesNode VisitWithClause(WithContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiTableDirectivesNode>(context);
            node.Tables.AddRange(context.namedQuery().Select(VisitNamedQuery));

            return node;
        }

        public static QsiDerivedTableNode VisitNamedQuery(NamedQueryContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiDerivedTableNode>(context);

            node.Alias.Value = new QsiAliasNode
            {
                Name = context.name.qi
            };

            var columnAliases = context.columnAliases();

            if (columnAliases is not null)
            {
                node.Columns.Value = new QsiColumnsDeclarationNode();

                node.Columns.Value.Columns.AddRange(
                    columnAliases.identifier().Select(i => new QsiColumnReferenceNode
                    {
                        Name = new QsiQualifiedIdentifier(i.qi)
                    })
                );
            }

            node.Source.Value = VisitQuery(context.query());

            return node;
        }

        public static QsiTableReferenceNode VisitQualifiedName(QualifiedNameContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiTableReferenceNode>(context);
            node.Identifier = context.qqi;

            return node;
        }
    }
}

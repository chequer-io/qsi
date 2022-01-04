using System.Collections.Generic;
using System.Linq;
using Qsi.Athena.Internal;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Athena.Tree.Visitors
{
    using static SqlBaseParser;

    internal static class TableVisitor
    {
        public static QsiTableNode VisitQuery(QueryContext context)
        {
            var with = context.with();
            var queryNoWith = context.queryNoWith();

            var queryNoWithNode = VisitQueryNoWith(queryNoWith);

            if (with is null)
                return queryNoWithNode;

            if (queryNoWithNode is not QsiDerivedTableNode derivedTable)
            {
                derivedTable = new QsiDerivedTableNode
                {
                    Source = { Value = queryNoWithNode },
                    Columns = { Value = TreeHelper.CreateAllColumnsDeclaration() }
                };

                queryNoWithNode = derivedTable;
            }
            else
            {
                AthenaTree.PutContextSpan(queryNoWithNode, context);
            }

            derivedTable.Directives.Value = VisitWith(with);

            return queryNoWithNode;
        }

        private static QsiTableNode VisitQueryNoWith(QueryNoWithContext context)
        {
            var queryTerm = context.queryTerm();
            var orderBy = context.orderBy();

            if (orderBy is not null)
            {
                var orderByNode = CommonVisitor.VisitOrderBy(orderBy);
            }
            
            return 
        }

        private static QsiTableNode VisitQueryTerm(QueryTermContext context)
        {
        }

        private static QsiLimitExpressionNode VisitLimitOffsetTerm(LimitOffsetTermContext context)
        {
            var node = AthenaTree.CreateWithSpan<QsiLimitExpressionNode>(context);
            
            var 
            
            node.Limit.Value = 

            return node;
        }

        private static QsiTableDirectivesNode VisitWith(WithContext context)
        {
            NamedQueryContext[] namedQueries = context.namedQuery();

            IEnumerable<QsiDerivedTableNode> namedQueryNodes = namedQueries.Select(VisitNamedQuery);

            var node = AthenaTree.CreateWithSpan<QsiTableDirectivesNode>(context);
            node.Tables.AddRange(namedQueryNodes);

            return node;
        }

        private static QsiDerivedTableNode VisitNamedQuery(NamedQueryContext context)
        {
            var name = context.name;
            var columnAliases = context.columnAliases();
            var query = context.query();

            var nameIdentifier = name.qi;

            var aliasNode = new QsiAliasNode
            {
                Name = nameIdentifier
            };

            var queryNode = VisitQuery(query);

            var node = AthenaTree.CreateWithSpan<QsiDerivedTableNode>(context);
            node.Alias.Value = aliasNode;

            if (columnAliases is not null)
            {
                var columnAliasesNode = VisitColumnAliases(columnAliases);
                node.Columns.Value = columnAliasesNode;
            }

            node.Source.Value = queryNode;

            return node;
        }

        private static QsiColumnsDeclarationNode VisitColumnAliases(ColumnAliasesContext context)
        {
            IdentifierContext[] identifiers = context.identifier();

            IEnumerable<QsiColumnReferenceNode> columnReferenceNodes = identifiers.Select(identifier => new QsiColumnReferenceNode
            {
                Name = new QsiQualifiedIdentifier(identifier.qi)
            });

            var node = new QsiColumnsDeclarationNode();
            node.Columns.AddRange(columnReferenceNodes);

            return node;
        }
    }
}

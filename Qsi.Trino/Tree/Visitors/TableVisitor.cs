using System;
using System.Linq;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Trino.Internal;

namespace Qsi.Trino.Tree.Visitors
{
    using static SqlBaseParser;

    internal static class TableVisitor
    {
        public static QsiTableNode VisitQuery(QueryContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiDerivedTableNode>(context);

            var with = context.with();

            if (with is not null)
                node.Directives.Value = VisitWithClause(with);

            node.Source.Value = VisitQueryNoWith(context.queryNoWith());

            return node;
        }

        public static QsiTableNode VisitQueryNoWith(QueryNoWithContext context)
        {
            throw new NotImplementedException();
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

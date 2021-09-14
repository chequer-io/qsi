using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.Oracle.Internal;
using Qsi.Tree;

namespace Qsi.Oracle.Tree.Visitors
{
    using static OracleParserInternal;

    internal static class IdentifierVisitor
    {
        public static QsiIdentifier VisitIdentifierFragment(IdentifierFragmentContext context)
        {
            var text = context.GetText();
            bool isEscaped = text.StartsWith('"');

            text = isEscaped ? text : text.ToUpperInvariant();

            return new QsiIdentifier(text, isEscaped);
        }

        public static QsiIdentifier VisitSimpleIdentifier(SimpleIdentifierContext context)
        {
            var text = context.GetText();
            bool isEscaped = text.StartsWith('"');

            text = isEscaped ? text : text.ToUpperInvariant();

            return new QsiIdentifier(text, isEscaped);
        }

        // [[database] .] object [@ dblink]
        public static QsiQualifiedIdentifier VisitFullObjectPath(FullObjectPathContext context)
        {
            var dbIdentifier = context.identifierFragment() is not null ?
                VisitIdentifierFragment(context.identifierFragment())
                : null;

            var objIdentifier = VisitIdentifier(context.identifier());

            return dbIdentifier is not null ?
                new QsiQualifiedIdentifier(dbIdentifier, objIdentifier) :
                new QsiQualifiedIdentifier(objIdentifier);
        }

        public static QsiIdentifier VisitIdentifier(IdentifierContext context)
        {
            if (context.dblink() is not null)
                throw new QsiException(QsiError.NotSupportedFeature, "dblink");

            return VisitIdentifierFragment(context.identifierFragment());
        }

        public static QsiAliasNode VisitAlias(AliasContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiAliasNode>(context);
            node.Name = VisitIdentifier(context.identifier());

            return node;
        }

        public static QsiAliasNode VisitAlias(TAliasContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiAliasNode>(context);
            node.Name = VisitIdentifier(context.identifier());

            return node;
        }

        public static QsiQualifiedIdentifier CreateQualifiedIdentifier(params IdentifierContext[] identifiers)
        {
            return new(identifiers.Select(VisitIdentifier));
        }

        public static IEnumerable<QsiColumnNode> VisitColumnList(ColumnListContext context)
        {
            foreach (var identifier in context.identifier())
            {
                var node = OracleTree.CreateWithSpan<QsiSequentialColumnNode>(identifier);
                var qsiIdentifier = VisitIdentifier(identifier);

                node.Alias.Value = new QsiAliasNode
                {
                    Name = qsiIdentifier
                };

                yield return node;
            }
        }

        public static QsiColumnReferenceNode VisitColumn(ColumnContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiColumnReferenceNode>(context);
            node.Name = CreateQualifiedIdentifier(context.identifier());

            return node;
        }

        public static QsiTableExpressionNode VisitNestedTable(NestedTableContext context)
        {
            var tableExprNode = new QsiTableExpressionNode();

            tableExprNode.Table.Value = new QsiTableReferenceNode
            {
                Identifier = new QsiQualifiedIdentifier(VisitIdentifier(context.identifier()))
            };

            return tableExprNode;
        }
    }
}

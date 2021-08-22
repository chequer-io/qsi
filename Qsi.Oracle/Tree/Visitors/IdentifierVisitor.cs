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
            return new QsiIdentifier(text, text.StartsWith('"'));
        }

        public static QsiIdentifier VisitSimpleIdentifier(SimpleIdentifierContext context)
        {
            var text = context.GetText();
            return new QsiIdentifier(text, text.StartsWith('"'));
        }

        // [[database] .] object [@ dblink]
        public static QsiQualifiedIdentifier VisitFullObjectPath(FullObjectPathContext context)
        {
            var dbIdentifier = context.identifierFragment() != null ?
                VisitIdentifierFragment(context.identifierFragment())
                : null;

            var objIdentifier = VisitIdentifier(context.identifier());

            return dbIdentifier != null ?
                new QsiQualifiedIdentifier(dbIdentifier, objIdentifier) :
                new QsiQualifiedIdentifier(objIdentifier);
        }

        public static QsiIdentifier VisitIdentifier(IdentifierContext context)
        {
            if (context.dblink() != null)
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
    }
}

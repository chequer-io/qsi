using System;
using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.Shared.Extensions;
using Qsi.Utilities;
using static Qsi.MySql.Internal.MySqlParserInternal;

namespace Qsi.MySql.Tree
{
    internal static class IdentifierVisitor
    {
        public static QsiQualifiedIdentifier VisitTableRef(TableRefContext context)
        {
            switch (context.children[0])
            {
                case QualifiedIdentifierContext qualifiedIdentifier:
                    return VisitQualifiedIdentifier(qualifiedIdentifier);

                case DotIdentifierContext dotIdentifier:
                    return new QsiQualifiedIdentifier(QsiIdentifier.Empty, VisitDotIdentifier(dotIdentifier));

                default:
                    throw TreeHelper.NotSupportedTree(context.children[0]);
            }
        }

        public static QsiQualifiedIdentifier VisitQualifiedIdentifier(QualifiedIdentifierContext context)
        {
            var identifier = VisitIdentifier((IdentifierContext)context.children[0]);

            if (context.ChildCount == 1)
                return new QsiQualifiedIdentifier(identifier);

            return new QsiQualifiedIdentifier(identifier, VisitDotIdentifier((DotIdentifierContext)context.children[1]));
        }

        public static QsiQualifiedIdentifier VisitTableRefWithWildcard(TableRefWithWildcardContext context)
        {
            var identifier = VisitIdentifier(context.identifier());
            var dotIdentifier = context.dotIdentifier() != null ? VisitDotIdentifier(context.dotIdentifier()) : null;
            var hasWildcard = context.HasToken(MULT_OPERATOR);

            var buffer = new QsiIdentifier[(dotIdentifier != null ? 2 : 1) + (hasWildcard ? 1 : 0)];

            buffer[0] = identifier;

            if (dotIdentifier != null)
                buffer[1] = dotIdentifier;

            if (hasWildcard)
                buffer[^1] = QsiIdentifier.Wildcard;

            return new QsiQualifiedIdentifier(buffer);
        }

        // Special rule that should also match all keywords if they are directly preceded by a dot.
        public static QsiIdentifier VisitDotIdentifier(DotIdentifierContext context)
        {
            var text = context.identifier().GetText();
            return new QsiIdentifier(text, IdentifierUtility.IsEscaped(text));
        }

        public static QsiIdentifier VisitIdentifier(IdentifierContext context)
        {
            switch (context.children[0])
            {
                case PureIdentifierContext pureIdentifier:
                    return VisitPureIdentifier(pureIdentifier);

                case IdentifierKeywordContext identifierKeyword:
                    return VisitIdentifierKeyword(identifierKeyword);

                default:
                    throw TreeHelper.NotSupportedTree(context.children[0]);
            }
        }

        public static QsiIdentifier VisitPureIdentifier(PureIdentifierContext context)
        {
            return new(context.GetText(), !context.HasToken(IDENTIFIER));
        }

        public static QsiIdentifier VisitIdentifierKeyword(IdentifierKeywordContext context)
        {
            return new(context.GetText(), false);
        }

        public static QsiIdentifier VisitTextStringLiteral(TextStringLiteralContext context)
        {
            return new(context.value.Text, true);
        }

        public static QsiQualifiedIdentifier VisitSimpleIdentifier(SimpleIdentifierContext context)
        {
            IEnumerable<QsiIdentifier> identifiers = context.children
                .Select(child =>
                {
                    if (child is DotIdentifierContext dotIdentifier)
                        return VisitDotIdentifier(dotIdentifier);

                    return VisitIdentifier((IdentifierContext)child);
                });

            return new QsiQualifiedIdentifier(identifiers);
        }

        public static QsiQualifiedIdentifier VisitFieldIdentifier(FieldIdentifierContext context)
        {
            switch (context.children[0])
            {
                case DotIdentifierContext dotIdentifier:
                    return new QsiQualifiedIdentifier(VisitDotIdentifier(dotIdentifier));

                case QualifiedIdentifierContext qualifiedIdentifier:
                    var identifier = VisitQualifiedIdentifier(qualifiedIdentifier);

                    if (context.ChildCount == 2)
                    {
                        var dotIdentifier = VisitDotIdentifier((DotIdentifierContext)context.children[1]);
                        identifier = new QsiQualifiedIdentifier(identifier.Append(dotIdentifier));
                    }

                    return identifier;

                default:
                    throw TreeHelper.NotSupportedTree(context.children[0]);
            }
        }

        public static QsiQualifiedIdentifier VisitInsertIdentifier(InsertIdentifierContext context)
        {
            if (context.children[0] is ColumnRefContext columnRef)
                return VisitColumnRef(columnRef);

            return VisitTableWild((TableWildContext)context.children[0]);
        }

        public static QsiQualifiedIdentifier VisitColumnRef(ColumnRefContext context)
        {
            return VisitFieldIdentifier(context.fieldIdentifier());
        }

        private static QsiQualifiedIdentifier VisitTableWild(TableWildContext context)
        {
            return new(
                context.identifier()
                    .Select(VisitIdentifier)
                    .Append(QsiIdentifier.Wildcard)
            );
        }

        public static QsiQualifiedIdentifier VisitViewName(ViewNameContext context)
        {
            switch (context.children[0])
            {
                case QualifiedIdentifierContext qualifiedIdentifier:
                    return VisitQualifiedIdentifier(qualifiedIdentifier);

                case DotIdentifierContext dotIdentifier:
                    return new QsiQualifiedIdentifier(VisitDotIdentifier(dotIdentifier));

                default:
                    throw TreeHelper.NotSupportedTree(context.children[0]);
            }
        }
    }
}

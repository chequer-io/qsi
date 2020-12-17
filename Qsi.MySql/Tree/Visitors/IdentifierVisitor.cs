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
            var identifier = VisitIdentifier(context.identifier());
            var dotIdentifier = context.dotIdentifier() != null ? VisitDotIdentifier(context.dotIdentifier()) : null;

            if (dotIdentifier == null)
                return new QsiQualifiedIdentifier(identifier);

            return new QsiQualifiedIdentifier(identifier, dotIdentifier);
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
                buffer[^1] = new QsiIdentifier("*", false);

            return new QsiQualifiedIdentifier(buffer);
        }

        // Special rule that should also match all keywords if they are directly preceded by a dot.
        public static QsiIdentifier VisitDotIdentifier(DotIdentifierContext context)
        {
            var text = context.identifier().GetText().TrimStart('.');
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

        private static QsiIdentifier VisitPureIdentifier(PureIdentifierContext context)
        {
            return new(context.GetText(), !context.HasToken(IDENTIFIER));
        }

        public static QsiIdentifier VisitIdentifierKeyword(IdentifierKeywordContext context)
        {
            return new(context.GetText(), false);
        }
    }
}

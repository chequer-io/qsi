using Antlr4.Runtime.Tree;
using Qsi.Data;
using Qsi.Utilities;
using static Qsi.MySql.Internal.MySqlParser;

namespace Qsi.MySql.Tree
{
    internal static class IdentifierVisitor
    {
        public static QsiQualifiedIdentifier Visit(IParseTree tree)
        {
            switch (tree)
            {
                case FullIdContext fullIdContext:
                    return VisitFullId(fullIdContext);

                case FullColumnNameContext fullColumnNameContext:
                    return VisitFullColumnName(fullColumnNameContext);

                case UidContext uidContext:
                    return new QsiQualifiedIdentifier(VisitUid(uidContext));

                case DottedIdContext dottedIdContext:
                    return new QsiQualifiedIdentifier(VisitDottedId(dottedIdContext));

                case ITerminalNode terminalNode:
                    return new QsiQualifiedIdentifier(VisitTerminalNode(terminalNode));
            }

            throw TreeHelper.NotSupportedTree(tree);
        }

        public static QsiQualifiedIdentifier VisitFullId(FullIdContext context)
        {
            var identifiers = new QsiIdentifier[context.ChildCount];

            for (int i = 0; i < identifiers.Length; i++)
            {
                switch (context.children[i])
                {
                    case UidContext uidContext:
                    {
                        identifiers[i] = VisitUid(uidContext);
                        break;
                    }

                    case DottedIdContext dottedIdContext:
                    {
                        identifiers[i] = VisitDottedId(dottedIdContext);
                        break;
                    }

                    default:
                    {
                        throw TreeHelper.NotSupportedTree(context.children[i]);
                    }
                }
            }

            return new QsiQualifiedIdentifier(identifiers);
        }

        public static QsiQualifiedIdentifier VisitFullColumnName(FullColumnNameContext context)
        {
            DottedIdContext[] dottedIds = context.dottedId();
            var identifiers = new QsiIdentifier[dottedIds.Length + 1];

            identifiers[0] = VisitUid(context.uid());

            for (int i = 0; i < dottedIds.Length; i++)
                identifiers[i + 1] = VisitDottedId(dottedIds[i]);

            return new QsiQualifiedIdentifier(identifiers);
        }

        public static QsiIdentifier VisitUid(UidContext context)
        {
            switch (context.children[0])
            {
                case SimpleIdContext simpleId:
                    return VisitSimpleId(simpleId);

                case ITerminalNode terminalNode:
                    return VisitTerminalNode(terminalNode);
            }

            return new QsiIdentifier(context.GetText(), false);
        }

        public static QsiIdentifier VisitSimpleId(SimpleIdContext context)
        {
            switch (context.children[0])
            {
                case EngineNameContext engineName:
                    return VisitEngineName(engineName);

                case ITerminalNode terminalNode:
                    return VisitTerminalNode(terminalNode);
            }

            return new QsiIdentifier(context.GetText(), false);
        }

        private static QsiIdentifier VisitEngineName(EngineNameContext context)
        {
            return VisitTerminalNode((ITerminalNode)context.children[0]);
        }

        public static QsiIdentifier VisitDottedId(DottedIdContext context)
        {
            var uid = context.uid();

            if (uid != null)
                return VisitUid(uid);

            if (context.children[0] is ITerminalNode terminalNode)
                return VisitTerminalNode(terminalNode);

            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiIdentifier VisitTerminalNode(ITerminalNode terminalNode)
        {
            switch (terminalNode.Symbol.Type)
            {
                case DOT_ID:
                    return new QsiIdentifier(terminalNode.GetText().Substring(1), false);

                case STRING_LITERAL:
                case REVERSE_QUOTE_ID:
                case CHARSET_REVERSE_QOUTE_STRING:
                    return new QsiIdentifier(terminalNode.GetText(), true);
            }

            return new QsiIdentifier(terminalNode.GetText(), false);
        }
    }
}

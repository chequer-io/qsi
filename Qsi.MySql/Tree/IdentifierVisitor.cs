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
                    return Visit(fullIdContext);

                case UidContext uidContext:
                    return new QsiQualifiedIdentifier(Visit(uidContext));

                case DottedIdContext dottedIdContext:
                    return new QsiQualifiedIdentifier(Visit(dottedIdContext));

                case ITerminalNode terminalNode:
                    return new QsiQualifiedIdentifier(Visit(terminalNode));
            }

            throw TreeHelper.NotSupportedTree(tree);
        }

        public static QsiQualifiedIdentifier Visit(FullIdContext context)
        {
            var identifiers = new QsiIdentifier[context.ChildCount];

            for (int i = 0; i < identifiers.Length; i++)
            {
                switch (context.children[i])
                {
                    case UidContext uidContext:
                    {
                        identifiers[i] = Visit(uidContext);
                        break;
                    }

                    case DottedIdContext dottedIdContext:
                    {
                        identifiers[i] = Visit(dottedIdContext);
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

        public static QsiIdentifier Visit(UidContext context)
        {
            if (context.children[0] is ITerminalNode terminalNode)
                return Visit(terminalNode);

            return new QsiIdentifier(context.GetText(), false);
        }

        public static QsiIdentifier Visit(DottedIdContext context)
        {
            var uid = context.uid();

            if (uid != null)
                return Visit(uid);

            if (context.children[0] is ITerminalNode terminalNode)
                return Visit(terminalNode);

            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiIdentifier Visit(ITerminalNode terminalNode)
        {
            switch (terminalNode.Symbol.Type)
            {
                case DOT_ID:
                    return new QsiIdentifier(terminalNode.GetText().Substring(1), false);

                case REVERSE_QUOTE_ID:
                case CHARSET_REVERSE_QOUTE_STRING:
                    return new QsiIdentifier(terminalNode.GetText(), true);
            }

            return new QsiIdentifier(terminalNode.GetText(), false);
        }
    }
}

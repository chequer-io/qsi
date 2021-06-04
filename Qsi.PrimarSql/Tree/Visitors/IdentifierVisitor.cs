using System.Linq;
using Antlr4.Runtime.Tree;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;
using static PrimarSql.Internal.PrimarSqlParser;

namespace Qsi.PrimarSql.Tree
{
    internal static class IdentifierVisitor
    {
        public static QsiQualifiedIdentifier Visit(IParseTree tree)
        {
            switch (tree)
            {
                case FullIdContext fullIdContext:
                    return VisitFullId(fullIdContext);

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

        public static PrimarSqlColumnReferenceNode VisitFullColumnName(FullColumnNameContext context)
        {
            return TreeHelper.Create<PrimarSqlColumnReferenceNode>(n =>
            {
                n.Name = new QsiQualifiedIdentifier(VisitUid(context.uid()));
                n.Accessors.AddRange(context.columnDottedId().Select(VisitColumnDottedId));
            });
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
                case ITerminalNode terminalNode:
                    return VisitTerminalNode(terminalNode);
            }

            return new QsiIdentifier(context.GetText(), false);
        }

        public static QsiExpressionNode VisitColumnDottedId(ColumnDottedIdContext context)
        {
            if (context.columnIndex() != null)
                return VisitColumnIndex(context.columnIndex());

            return new QsiFieldExpressionNode
            {
                Identifier = new QsiQualifiedIdentifier(VisitDottedId(context.dottedId()))
            };
        }

        public static PrimarSqlIndexerExpressionNode VisitColumnIndex(ColumnIndexContext context)
        {
            return TreeHelper.Create<PrimarSqlIndexerExpressionNode>(n =>
            {
                n.Indexer.SetValue(ExpressionVisitor.VisitLiteral(context.decimalLiteral()));
            });
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
                case STRING_LITERAL:
                case DOUBLE_QUOTE_ID:
                case REVERSE_QUOTE_ID:
                    return new QsiIdentifier(terminalNode.GetText(), true);
            }

            return new QsiIdentifier(terminalNode.GetText(), false);
        }
    }
}

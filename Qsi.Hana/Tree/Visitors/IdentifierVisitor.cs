using System.Linq;
using Antlr4.Runtime.Tree;
using Qsi.Data;
using static Qsi.Hana.Internal.HanaParserInternal;

namespace Qsi.Hana.Tree.Visitors
{
    internal static class IdentifierVisitor
    {
        public static QsiIdentifier VisitIdentifier(IdentifierContext context)
        {
            var identifier = context.children[0];
            var escaped = identifier is ITerminalNode { Symbol: { Type: QUOTED_IDENTIFIER } };

            return new QsiIdentifier(identifier.GetText(), escaped);
        }

        public static QsiIdentifier VisitColumnName(ColumnNameContext context)
        {
            return VisitIdentifier(context.identifier());
        }

        public static QsiQualifiedIdentifier VisitFieldName(FieldNameContext context)
        {
            return new(context.identifier().Select(VisitIdentifier));
        }

        public static QsiQualifiedIdentifier VisitTableName(TableNameContext context)
        {
            return new(context.identifier().Select(VisitIdentifier));
        }

        public static QsiQualifiedIdentifier VisitFunctionName(FunctionNameContext context)
        {
            return new(context.identifier().Select(VisitIdentifier));
        }
    }
}

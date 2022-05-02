using System.Linq;
using Qsi.Data;
using static Qsi.PostgreSql.Internal.PostgreSqlParserInternal;

namespace Qsi.PostgreSql.Tree.Visitors;

internal static class IdentifierVisitor
{
    public static QsiIdentifier VisitIdentifier(IdentifierContext context)
    {
        return context.id;
    }

    public static QsiIdentifier VisitIdentifier(ColumnLabelIdentifierContext context)
    {
        return context.id;
    }

    public static QsiIdentifier VisitIdentifier(ColumnIdentifierContext context)
    {
        return context.id;
    }

    public static QsiIdentifier VisitIdentifier(TypeFunctionIdentifierContext context)
    {
        return context.id;
    }

    public static QsiQualifiedIdentifier VisitFunctionName(FunctionNameContext context)
    {
        if (context.typeFunctionIdentifier() != null)
        {
            var identifier = VisitIdentifier(context.typeFunctionIdentifier());
            return new QsiQualifiedIdentifier(identifier);
        }

        return VisitQualifiedIdentifier(context.qualifiedIdentifier());
    }
    
    public static QsiQualifiedIdentifier VisitQualifiedIdentifier(QualifiedIdentifierContext context)
    {
        var identifier = VisitIdentifier(context.columnIdentifier());

        if (context.indirection() == null)
        {
            return new QsiQualifiedIdentifier(identifier);
        }

        var identifiers = context
            .indirection()
            .columnLabelIdentifier()
            .Select(VisitIdentifier)
            .Prepend(identifier);

        return new QsiQualifiedIdentifier(identifiers);
    }
}

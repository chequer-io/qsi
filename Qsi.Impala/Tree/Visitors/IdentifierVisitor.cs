using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;
using Qsi.Data;
using Qsi.Impala.Internal;
using Qsi.Utilities;

namespace Qsi.Impala.Tree.Visitors;

using static ImpalaParserInternal;

internal static class IdentifierVisitor
{
    public static IEnumerable<QsiIdentifier> VisitIdentList(Ident_listContext context)
    {
        return context.ident_or_default().Select(IdentifierVisitor.VisitIdentOrDefault);
    }

    public static QsiIdentifier VisitStringLiteral(ITerminalNode node)
    {
        TreeHelper.VerifyTokenType(node.Symbol, STRING_LITERAL);

        return new QsiIdentifier(node.GetText()[1..^1], false);
    }

    public static QsiIdentifier VisitIdentOrDefault(Ident_or_defaultContext context)
    {
        if (context.children[0] is ITerminalNode { Symbol: { Type: IDENT } } ident)
            return VisitIdent(ident);

        return VisitDefault(context.KW_DEFAULT());
    }

    private static QsiIdentifier VisitIdent(ITerminalNode node)
    {
        var text = node.GetText();
        return new QsiIdentifier(text, text.StartsWith('`'));
    }

    public static QsiIdentifier VisitDefault(ITerminalNode node)
    {
        TreeHelper.VerifyTokenType(node.Symbol, KW_DEFAULT);

        return new QsiIdentifier(node.GetText(), false);
    }

    public static QsiQualifiedIdentifier VisitDottedPath(Dotted_pathContext context)
    {
        return new(context.ident_or_default().Select(VisitIdentOrDefault));
    }

    public static QsiQualifiedIdentifier VisitSlotRef(Slot_refContext context)
    {
        return VisitDottedPath(context.dotted_path());
    }

    public static QsiQualifiedIdentifier VisitTableName(Table_nameContext context)
    {
        return new(context.ident_or_default().Select(VisitIdentOrDefault));
    }

    public static QsiQualifiedIdentifier VisitColumnName(Column_nameContext context)
    {
        return new(context.ident_or_default().Select(VisitIdentOrDefault));
    }

    public static QsiQualifiedIdentifier VisitFunctionName(Column_nameContext context)
    {
        if (context.children[0] is Dotted_pathContext dottedPath)
            return VisitDottedPath(dottedPath);

        return new QsiQualifiedIdentifier(new QsiIdentifier(context.children[0].GetText(), false));
    }
}
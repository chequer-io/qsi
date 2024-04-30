using Qsi.Analyzers.Context;
using Qsi.Data;
using Qsi.Services;
using Qsi.Utilities;

namespace Qsi.Analyzers.Extensions;

public static class QsiLanguageServiceExtension
{
    public static bool MatchIdentifier(
        this IQsiLanguageService languageService,
        QsiQualifiedIdentifier a,
        QsiQualifiedIdentifier b)
    {
        if (a.Level != b.Level)
            return false;

        for (int i = 0; i < a.Level; i++)
        {
            if (!languageService.MatchIdentifier(a[i], b[i]))
                return false;
        }

        return true;
    }

    public static bool MatchIdentifier(
        this IQsiLanguageService languageService,
        IAnalyzerContext context,
        QsiTableStructure table,
        QsiQualifiedIdentifier identifier)
    {
        if (!table.HasIdentifier)
            return false;

        // * case - Explicit access
        // ┌──────────────────────────────────────────────────────────┐
        // │ SELECT sakila.actor.column FROM sakila.actor             │
        // │        ▔▔▔▔▔▔^▔▔▔▔▔      ==     ▔▔▔▔▔▔^▔▔▔▔▔             │
        // │         └-> identifier(2)        └-> table.Identifier(2) │
        // └──────────────────────────────────────────────────────────┘ 

        if (MatchIdentifier(languageService, table.Identifier, identifier))
            return true;

        // * case - 2 Level implicit access
        // ┌──────────────────────────────────────────────────────────┐
        // │ SELECT actor.column FROM sakila.actor                    │
        // │        ▔▔▔▔▔      <       ▔▔▔▔▔^▔▔▔▔▔                    │
        // │         └-> identifier(1)  └-> table.Identifier(2)       │
        // └──────────────────────────────────────────────────────────┘ 

        // * case - 3 Level implicit access
        // ┌──────────────────────────────────────────────────────────┐
        // │ SELECT sakila.actor.column FROM db.sakila.actor          │
        // │        ▔▔▔▔▔▔^▔▔▔▔▔       <     ▔▔^▔▔▔▔▔▔^▔▔▔▔▔          │
        // │         └-> identifier(2)        └-> table.Identifier(3) │
        // └──────────────────────────────────────────────────────────┘ 

        if (context.AnalyzerOptions.UseExplicitRelationAccess)
            return false;

        if (!QsiUtility.IsReferenceType(table.Type))
            return false;

        if (table.Identifier.Level <= identifier.Level)
            return false;

        QsiIdentifier[] partialIdentifiers = table.Identifier[^identifier.Level..];
        var partialIdentifier = new QsiQualifiedIdentifier(partialIdentifiers);

        return MatchIdentifier(languageService, partialIdentifier, identifier);
    }
}

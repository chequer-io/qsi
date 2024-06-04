using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Qsi.SingleStore.Internal;

// library/base/symbol-info.cpp
internal static partial class SingleStoreSymbolInfo
{
    private record Keyword(string Identifier, bool IsReserved);

    private static readonly ConcurrentDictionary<SingleStoreVersion, IReadOnlySet<string>> _keywords = new();
    private static readonly ConcurrentDictionary<SingleStoreVersion, IReadOnlySet<string>> _reservedKeywords = new();

    // MySQLSymbolInfo::keywordsForVersion
    public static IReadOnlySet<string> keywordsForVersion(SingleStoreVersion version)
    {
        if (!_keywords.ContainsKey(version))
        {
            Keyword[] keywords = version switch
            {
                SingleStoreVersion.MySQL56 => _keywords56,
                SingleStoreVersion.MySQL57 => _keywords57,
                SingleStoreVersion.MySQL80 => _keywords80,
                _ => Array.Empty<Keyword>()
            };

            _keywords[version] = keywords
                .Select(k => k.Identifier)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            _reservedKeywords[version] = keywords
                .Where(k => k.IsReserved)
                .Select(k => k.Identifier)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        return _keywords[version];
    }

    //MySQLSymbolInfo::isReservedKeyword
    public static bool isReservedKeyword(string identifier, SingleStoreVersion version)
    {
        keywordsForVersion(version);
        return _reservedKeywords[version].Contains(identifier);
    }

    //MySQLSymbolInfo::isReservedKeyword
    public static bool isKeyword(string identifier, SingleStoreVersion version)
    {
        keywordsForVersion(version);
        return _keywords[version].Contains(identifier);
    }

    // MySQLSymbolInfo::numberToVersion
    public static SingleStoreVersion numberToVersion(int version)
    {
        int major = version / 10000;
        int minor = (version / 100) % 100;

        switch (major)
        {
            case < 5:
            case > 8:
                return SingleStoreVersion.Unknown;

            case 8:
                return SingleStoreVersion.MySQL80;
        }

        if (major != 5)
            return SingleStoreVersion.Unknown;

        switch (minor)
        {
            case 6:
                return SingleStoreVersion.MySQL56;

            case 7:
                return SingleStoreVersion.MySQL57;

            default:
                return SingleStoreVersion.Unknown;
        }
    }
}

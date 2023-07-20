using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Qsi.MySql.Internal;

// library/base/symbol-info.cpp
internal static partial class MySqlSymbolInfo
{
    private record Keyword(string Identifier, bool IsReserved);

    private static readonly ConcurrentDictionary<MySqlVersion, IReadOnlySet<string>> _keywords = new();
    private static readonly ConcurrentDictionary<MySqlVersion, IReadOnlySet<string>> _reservedKeywords = new();

    // MySQLSymbolInfo::keywordsForVersion
    public static IReadOnlySet<string> keywordsForVersion(MySqlVersion version)
    {
        if (!_keywords.ContainsKey(version))
        {
            Keyword[] keywords = version switch
            {
                MySqlVersion.MySQL56 => _keywords56,
                MySqlVersion.MySQL57 => _keywords57,
                MySqlVersion.MySQL80 => _keywords80,
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
    public static bool isReservedKeyword(string identifier, MySqlVersion version)
    {
        keywordsForVersion(version);
        return _reservedKeywords[version].Contains(identifier);
    }

    //MySQLSymbolInfo::isReservedKeyword
    public static bool isKeyword(string identifier, MySqlVersion version)
    {
        keywordsForVersion(version);
        return _keywords[version].Contains(identifier);
    }

    // MySQLSymbolInfo::numberToVersion
    public static MySqlVersion numberToVersion(int version)
    {
        int major = version / 10000;
        int minor = (version / 100) % 100;

        switch (major)
        {
            case < 5:
            case > 8:
                return MySqlVersion.Unknown;

            case 8:
                return MySqlVersion.MySQL80;
        }

        if (major != 5)
            return MySqlVersion.Unknown;

        switch (minor)
        {
            case 6:
                return MySqlVersion.MySQL56;

            case 7:
                return MySqlVersion.MySQL57;

            default:
                return MySqlVersion.Unknown;
        }
    }
}
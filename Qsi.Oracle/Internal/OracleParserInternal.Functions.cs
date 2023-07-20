using System;
using System.Collections.Generic;
using System.Linq;

namespace Qsi.Oracle.Internal;

internal partial class OracleParserInternal
{
    private readonly Dictionary<string, string[]> _tAliasInvalidRules = new(StringComparer.OrdinalIgnoreCase)
    {
        // Fix Problem (InnerCrossJoinClause)
        // Query:   SELECT * FROM actor INNER   JOIN   actor ...
        // Invalid: --KW-- * -KW- TABLE ALIAS JOINTYPE TABLE
        // Fix:     --KW-- * -KW- TABLE ---JOINTYPE--- TABLE
        ["INNER"] = new[] { "JOIN" },

        // Fix Problem (OuterJoinClause)
        // Query:   SELECT * FROM actor NATURAL FULL OUTER JOIN actor ...
        // Invalid: --KW-- * -KW- TABLE -ALIAS- ----JOINTYPE--- TABLE
        // Fix:     --KW-- * -KW- TABLE --------JOINTYPE------- TABLE
        ["NATURAL"] = new[] { "FULL", "LEFT", "RIGHT" }
    };

    private bool CheckTAlias()
    {
        var token = CurrentToken;
        var nextToken = TokenStream.LT(2);

        if (_tAliasInvalidRules.TryGetValue(token.Text, out var keywords))
            return !keywords.Any(keyword => nextToken.Text.Equals(keyword, StringComparison.OrdinalIgnoreCase));

        return true;
    }
}
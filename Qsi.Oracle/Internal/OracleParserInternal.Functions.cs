using System;

namespace Qsi.Oracle.Internal
{
    internal partial class OracleParserInternal
    {
        public bool checkTAlias()
        {
            var token = CurrentToken;
            var nextToken = TokenStream.LT(2);

            return !token.Text.Equals("INNER", StringComparison.OrdinalIgnoreCase) || !nextToken.Text.Equals("JOIN", StringComparison.OrdinalIgnoreCase);
        }
    }
}

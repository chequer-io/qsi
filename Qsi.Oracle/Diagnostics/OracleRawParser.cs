using Qsi.Diagnostics;
using Qsi.JSql.Diagnostics;

namespace Qsi.Oracle.Diagnostics
{
    public class OracleRawParser : JSqlRawParser
    {
        public override IRawTree Parse(string input)
        {
            return base.Parse(OracleCompat.Normalize(input));
        }
    }
}

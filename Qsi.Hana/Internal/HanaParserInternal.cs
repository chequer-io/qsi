using System;

namespace Qsi.Hana.Internal
{
    internal partial class HanaParserInternal
    {
        public bool IsRegexFlag()
        {
            const string flags = "imsx";

            return
                CurrentToken?.Text?.Length == 1 &&
                flags.Contains(CurrentToken.Text[0], StringComparison.OrdinalIgnoreCase);
        }
    }
}

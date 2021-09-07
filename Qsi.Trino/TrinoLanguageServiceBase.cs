using Qsi.Analyzers;
using Qsi.Parsing;
using Qsi.Parsing.Common;
using Qsi.Services;

namespace Qsi.Trino
{
    public abstract class TrinoLanguageServiceBase : QsiLanguageServiceBase
    {
        public override QsiAnalyzerOptions CreateAnalyzerOptions()
        {
            return new();
        }

        public override IQsiTreeParser CreateTreeParser()
        {
            return new TrinoParser();
        }

        public override IQsiTreeDeparser CreateTreeDeparser()
        {
            return new TrinoDeparser();
        }

        public override IQsiScriptParser CreateScriptParser()
        {
            return new CommonScriptParser();
        }
    }
}

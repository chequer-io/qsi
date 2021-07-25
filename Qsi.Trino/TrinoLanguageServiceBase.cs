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
            throw new System.NotImplementedException();
        }

        public override IQsiTreeDeparser CreateTreeDeparser()
        {
            throw new System.NotImplementedException();
        }

        public override IQsiScriptParser CreateScriptParser()
        {
            return new CommonScriptParser();
        }
    }
}

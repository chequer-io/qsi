using Qsi.Analyzers;
using Qsi.Parsing;
using Qsi.Parsing.Common;
using Qsi.Services;

namespace Qsi.Impala
{
    public abstract class ImpalaLanguageServiceBase : QsiLanguageServiceBase
    {
        public override IQsiTreeParser CreateTreeParser()
        {
            return new ImpalaParser();
        }

        public override IQsiTreeDeparser CreateTreeDeparser()
        {
            return new ImpalaDeparser();
        }

        public override IQsiScriptParser CreateScriptParser()
        {
            return new ImpalaScriptParser();
        }

        public override QsiAnalyzerOptions CreateAnalyzerOptions()
        {
            return new();
        }
    }
}

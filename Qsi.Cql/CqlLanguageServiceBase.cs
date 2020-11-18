using Qsi.Analyzers;
using Qsi.Parsing;
using Qsi.Services;

namespace Qsi.Cassandra
{
    public abstract class CqlLanguageServiceBase : QsiLanguageServiceBase
    {
        public override IQsiTreeParser CreateTreeParser()
        {
            return new CqlParser();
        }

        public override IQsiTreeDeparser CreateTreeDeparser()
        {
            return new CqlDeparser();
        }

        public override IQsiScriptParser CreateScriptParser()
        {
            return new CqlScriptParser();
        }

        public override QsiAnalyzerOptions CreateAnalyzerOptions()
        {
            return null;
        }
    }
}

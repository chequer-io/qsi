using Qsi.Analyzers;
using Qsi.Parsing;
using Qsi.Services;

namespace Qsi.PhoenixSql
{
    public abstract class PhoenixSqlLanguageServiceBase : QsiLanguageServiceBase
    {
        public override IQsiTreeParser CreateTreeParser()
        {
            return new PhoenixSqlParser();
        }

        public override IQsiTreeDeparser CreateTreeDeparser()
        {
            throw new System.NotImplementedException();
        }

        public override IQsiScriptParser CreateScriptParser()
        {
            return new PhoenixSqlScriptParser();
        }

        public override QsiAnalyzerOptions CreateAnalyzerOptions()
        {
            return new QsiAnalyzerOptions();
        }
    }
}

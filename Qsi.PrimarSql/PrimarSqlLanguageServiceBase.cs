using Qsi.Analyzers;
using Qsi.Parsing;
using Qsi.Services;

namespace Qsi.PrimarSql
{
    public abstract class PrimarSqlLanguageServiceBase : QsiLanguageServiceBase
    {
        public override IQsiTreeParser CreateTreeParser()
        {
            return new PrimarSqlParser();
        }
        
        public override IQsiTreeDeparser CreateTreeDeparser()
        {
            return new PrimarSqlDeparser();
        }
        
        public override IQsiScriptParser CreateScriptParser()
        {
            return new PrimarSqlScriptParser();
        }
        
        public override QsiAnalyzerOptions CreateAnalyzerOptions()
        {
            return new QsiAnalyzerOptions
            {
                AllowEmptyColumnsInSelect = false
            };
        }
    }
}

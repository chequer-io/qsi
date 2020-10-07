using Qsi.Analyzers;
using Qsi.Parsing;
using Qsi.Services;

namespace Qsi.MySql
{
    public abstract class MySqlLanguageServiceBase : QsiLanguageServiceBase
    {
        public override IQsiTreeParser CreateTreeParser()
        {
            return new MySqlParser();
        }

        public override IQsiTreeDeparser CreateTreeDeparser()
        {
            throw new System.NotImplementedException();
        }

        public override IQsiScriptParser CreateScriptParser()
        {
            return new MySqlScriptParser();
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

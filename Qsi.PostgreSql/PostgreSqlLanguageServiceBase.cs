using Qsi.Analyzers;
using Qsi.Parsing;
using Qsi.Services;

namespace Qsi.PostgreSql
{
    public abstract class PostgreSqlLanguageServiceBase : QsiLanguageServiceBase
    {
        public override IQsiTreeParser CreateTreeParser()
        {
            return new PostgreSqlParser();
        }

        public override IQsiScriptParser CreateScriptParser()
        {
            return new PostgreSqlScriptParser();
        }

        public override QsiAnalyzerOptions CreateAnalyzerOptions()
        {
            return new QsiAnalyzerOptions
            {
                AllowEmptyColumnsInSelect = true
            };
        }
    }
}

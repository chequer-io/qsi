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
    }
}

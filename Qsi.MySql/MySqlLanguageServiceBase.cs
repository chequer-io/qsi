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

        public override IQsiScriptParser CreateScriptParser()
        {
            return new MySqlScriptParser();
        }
    }
}

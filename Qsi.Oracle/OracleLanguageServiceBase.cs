using Qsi.Compiler;
using Qsi.JSql;
using Qsi.Parsing;

namespace Qsi.Oracle
{
    public abstract class OracleLanguageServiceBase : JSqlLanguageServiceBase
    {
        public override IQsiTreeParser CreateTreeParser()
        {
            return new OracleParser();
        }

        public override IQsiScriptParser CreateScriptParser()
        {
            return new OracleScriptParser();
        }

        public override QsiTableCompileOptions CreateCompileOptions()
        {
            return new QsiTableCompileOptions
            {
                AllowNoAliasInDerivedTable = true,
                UseAutoFixRecursiveQuery = true
            };
        }
    }
}

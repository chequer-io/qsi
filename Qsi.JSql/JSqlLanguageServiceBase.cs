using Qsi.Compiler;
using Qsi.Parsing;
using Qsi.Services;

namespace Qsi.JSql
{
    public abstract class JSqlLanguageServiceBase : QsiLanguageServiceBase
    {
        public override IQsiTreeParser CreateTreeParser()
        {
            return new JSqlParser();
        }

        public override IQsiScriptParser CreateScriptParser()
        {
            return new JSqlScriptParser();
        }

        public override QsiTableCompileOptions CreateCompileOptions()
        {
            return new QsiTableCompileOptions
            {
                UseAutoFixRecursiveQuery = true
            };
        }
    }
}

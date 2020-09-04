using Qsi.Compiler;
using Qsi.Parsing;
using Qsi.Services;

namespace Qsi.SqlServer
{
    public abstract class SqlServerLanguageServiceBase : QsiLanguageServiceBase
    {
        public override IQsiTreeParser CreateTreeParser()
        {
            return new SqlServerParser();
        }

        public override IQsiScriptParser CreateScriptParser()
        {
            return new CommonScriptParser();
        }

        public override QsiTableCompileOptions CreateCompileOptions()
        {
            return new QsiTableCompileOptions
            {
                AllowEmptyColumnsInSelect = false
            };
        }
    }
}

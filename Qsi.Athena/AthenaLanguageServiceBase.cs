using Qsi.Parsing;
using Qsi.Services;

namespace Qsi.Athena
{
    public abstract class AthenaLanguageSerivceBase : QsiLanguageServiceBase
    {
        public override IQsiTreeParser CreateTreeParser()
        {
            return new AthenaParser();
        }

        public override IQsiScriptParser CreateScriptParser()
        {
            return new AthenaScriptParser();
        }

        public override IQsiTreeDeparser CreateTreeDeparser()
        {
            return new AthenaDeparser();
        }
    }
}

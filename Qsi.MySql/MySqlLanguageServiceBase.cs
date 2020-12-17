using System;
using Qsi.Analyzers;
using Qsi.Parsing;
using Qsi.Services;

namespace Qsi.MySql
{
    public abstract class MySqlLanguageServiceBase : QsiLanguageServiceBase
    {
        public abstract Version Version { get; }

        public override IQsiTreeParser CreateTreeParser()
        {
            return new MySqlParser(Version);
        }

        public override IQsiTreeDeparser CreateTreeDeparser()
        {
            return new MySqlDeparser();
        }

        public override IQsiScriptParser CreateScriptParser()
        {
            return new MySqlScriptParser();
        }

        public override QsiAnalyzerOptions CreateAnalyzerOptions()
        {
            return new()
            {
                AllowEmptyColumnsInSelect = false
            };
        }
    }
}

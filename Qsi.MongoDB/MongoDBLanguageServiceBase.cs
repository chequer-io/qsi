using System;
using Qsi.Analyzers;
using Qsi.Parsing;
using Qsi.Services;

namespace Qsi.MongoDB
{
    public abstract class MongoDBLanguageServiceBase : QsiLanguageServiceBase
    {
        public override IQsiScriptParser CreateScriptParser()
        {
            return new MongoDBScriptParser();
        }

        public override QsiAnalyzerOptions CreateAnalyzerOptions()
        {
            throw new NotSupportedException();
        }

        public override IQsiRepositoryProvider CreateRepositoryProvider()
        {
            throw new NotSupportedException();
        }

        public override IQsiTreeDeparser CreateTreeDeparser()
        {
            throw new NotSupportedException();
        }

        public override IQsiTreeParser CreateTreeParser()
        {
            throw new NotSupportedException();
        }
    }
}

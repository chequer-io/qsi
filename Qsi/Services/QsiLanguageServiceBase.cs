using Qsi.Parsing;

namespace Qsi.Services
{
    public abstract class QsiLanguageServiceBase : IQsiLanguageService
    {
        public abstract IQsiParser CreateParser();

        public abstract IQsiScriptParser CreateScriptParser();

        public abstract IQsiReferenceResolver CreateResolver();
    }
}

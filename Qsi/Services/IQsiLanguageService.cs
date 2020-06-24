using Qsi.Parsing;

namespace Qsi.Services
{
    public interface IQsiLanguageService
    {
        IQsiParser CreateParser();

        IQsiScriptParser CreateScriptParser();

        IQsiReferenceResolver CreateResolver();
    }
}

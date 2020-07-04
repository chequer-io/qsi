using Qsi.Data;
using Qsi.Parsing;

namespace Qsi.Services
{
    public interface IQsiLanguageService
    {
        IQsiTreeParser CreateTreeParser();

        IQsiScriptParser CreateScriptParser();

        IQsiReferenceResolver CreateResolver();

        bool MatchIdentifier(QsiIdentifier x, QsiIdentifier y);
    }
}
